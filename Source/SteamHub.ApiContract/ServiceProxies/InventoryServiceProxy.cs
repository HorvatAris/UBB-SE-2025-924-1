using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Utils;
using SteamHub.ApiContract.Models.ItemTrade;
using System.Net.Http.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class InventoryServiceProxy : IInventoryService
    {
        private readonly InventoryValidator inventoryValidator;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public IUserDetails User { get; set; }
        public InventoryServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.User = user;
        }

        public async Task AddItemToInventoryAsync(Game game, Item item, int userId)
        {
            var request = new AddItemToInventoryRequest
            {
                Game = game,
                Item = item
            };

            var response = await _httpClient.PostAsJsonAsync($"/api/Inventory/AddItemToInventory/{userId}", request);
            response.EnsureSuccessStatusCode();
        }


        public List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            IEnumerable<Item> filtered = items;

            // Filter out listed items (only show unlisted ones)
            filtered = filtered.Where(item => !item.IsListed);

            // Filter by selected game if it's not null and not the "All Games" option
            if (selectedGame != null && selectedGame.GameTitle != "All Games")
            {
                filtered = filtered.Where(item =>
                    string.Equals(item.GameName, selectedGame.GameTitle, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by search text if provided
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                filtered = filtered.Where(item =>
                    (!string.IsNullOrEmpty(item.ItemName) &&
                     item.ItemName.Trim().Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(item.Description) &&
                     item.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }

            return filtered.ToList();
        }

        public async Task<List<Item>> GetAllItemsFromInventoryAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/Inventory/All/{userId}");
            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<List<Item>>();
            return items ?? new List<Item>();
        }

        public IUserDetails GetAllUsers()
        {
            return this.User;
        }

        public async Task<IUserDetails> GetAllUsersAsync()
        {
            return await Task.FromResult(this.User);
        }

        public async Task<List<Game>> GetAvailableGamesAsync(List<Item> items, int userId)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/Inventory/AvailableGames/{userId}",
                items
            );
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Status: " + response.StatusCode);
                Console.WriteLine("Error content: " + errorContent);
                throw new Exception("API call failed: " + errorContent);
            }

            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<Game>>();
            return games ?? new List<Game>();
        }


        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game, int userId)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/Inventory/ItemsFromGame/{userId}",
                game
            );

            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<List<Item>>();
            return items ?? new List<Item>();
        }


        public async Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText)
        {
            var allItems = await this.GetUserInventoryAsync(userId);
            return this.FilterInventoryItems(allItems, selectedGame, searchText);
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be positive.", nameof(userId));
            }

            var response = await _httpClient.GetAsync($"/api/Inventory/UserInventory/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get user inventory: {error}");
            }

            var items = await response.Content.ReadFromJsonAsync<List<Item>>();
            var finalList = new List<Item>();
            foreach (var item in items)
            {
                item.GameName = item.Game.GameTitle;
                finalList.Add(item);
            }


            return finalList ?? new List<Item>();
        }


        public async Task<bool> SellItemAsync(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var response = await _httpClient.PatchAsJsonAsync("/api/Inventory/SellItem", item);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to sell item: {errorContent}");
            }
        }

    }
}
