using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class MarketplaceServiceProxy : IMarketplaceService
    {
        public IUserDetails User { get; set; }
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public MarketplaceServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        public async Task AddListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game.GameId, item.ItemId);
        }

        public async Task<bool> BuyItemAsync(Item item, int userId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Marketplace/BuyItem/{userId}", item);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while buying the item: {ex.Message}", ex);
            }
        }

        public async Task<List<Item>> GetAllListingsAsync()
        {
            try 
            {
                var response = await _httpClient.GetAsync("api/Marketplace/Listings");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Item>>(content, _options) ?? new List<Item>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching listings: {ex.Message}", ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Marketplace/Users");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<User>>(content, _options) ?? new List<User>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching users: {ex.Message}", ex);
            }
        }

        public async Task<List<Item>> GetListingsByGameAsync(int gameId, int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Marketplace/Listings/{userId}/Game/{gameId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Item>>(content, _options) ?? new List<Item>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching listings for game ID {gameId}: {ex.Message}", ex);
            }
        }

        public async Task RemoveListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game.GameId, item.ItemId);
        }

        public async Task UpdateListingAsync(int gameId, int itemId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/Marketplace/UpdateListing/{gameId}/{itemId}", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the listing for game ID {gameId} and item ID {itemId}: {ex.Message}", ex);
            }
        }
        public async Task SwitchListingStatusAsync(int gameId, int itemId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/Marketplace/SwitchListingStatus/{gameId}/{itemId}", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while switching the listing status for game ID {gameId} and item ID {itemId}: {ex.Message}", ex);
            }
        }
    }
}
