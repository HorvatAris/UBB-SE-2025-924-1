using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Models.ItemTradeDetails;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Services;
using System.Collections.ObjectModel;
using SteamHub.ApiContract.Models.UserInventory;
using System.Net.Http.Json;
using System.Diagnostics;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class TradeServiceProxy : ITradeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public TradeServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.user = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        private IUserDetails user;

        public async Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser)
        {
            try
            {
                if (isSourceUser)
                {
                    trade.AcceptBySourceUser();
                }
                else
                {
                    trade.AcceptByDestinationUser();
                }

                await this.UpdateItemTradeAsync(trade);

                // If both users have accepted, complete the trade
                if (trade.AcceptedByDestinationUser)
                {
                    await this.CompleteTradeAsync(trade);
                }
            }
            catch (Exception tradeAcceptionException)
            {
                System.Diagnostics.Debug.WriteLine($"Error accepting trade: {tradeAcceptionException.Message}");
                throw;
            }
        }

        public async Task AddItemTradeAsync(ItemTrade trade)
        {
            if (trade == null)
                throw new ArgumentNullException(nameof(trade));

            if (trade.SourceUser == null || trade.DestinationUser == null)
                throw new ArgumentException("Source and destination users must be specified");
            
            if (trade.GameOfTrade == null)
                throw new ArgumentException("Game must be specified");

            // Ensure trade has initial status
            if (string.IsNullOrEmpty(trade.TradeStatus))
                trade.TradeStatus = "Pending";

            // Use PostAsJsonAsync to send the Trade object as JSON in the request body
            var response = await _httpClient.PostAsJsonAsync($"/api/Trade", trade);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add trade. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }

        public async Task CompleteTradeAsync(ItemTrade trade)
        {
            try
            {
                // Transfer source user items to destination user
                var tradeRequest = new TransferItemTradeRequest
                {
                    SourceUserId = trade.SourceUser.UserId,
                    DestinationUserId = trade.DestinationUser.UserId,
                    GameId = trade.GameOfTrade.GameId
                };

                foreach (var item in trade.SourceUserItems)
                {
                    tradeRequest.ItemId = item.ItemId;
                    await this.TransferItemAsync(tradeRequest);
                }


                // Transfer destination user items to source user
                tradeRequest.SourceUserId = trade.DestinationUser.UserId;
                tradeRequest.DestinationUserId = trade.SourceUser.UserId;
                foreach (var item in trade.DestinationUserItems)
                {
                    tradeRequest.ItemId = item.ItemId;
                    await this.TransferItemAsync(tradeRequest);
                }

                trade.MarkTradeAsCompleted();
                await this.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeCompletingException)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing trade: {tradeCompletingException.Message}");
                throw;
            }
        }

        public async Task CreateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.AddItemTradeAsync(trade);
            }
            catch (Exception tradeCreationException)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating trade: {tradeCreationException.Message}");
                throw;
            }
        }

        public async Task DeclineTradeRequest(ItemTrade trade)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"/api/Trade/Decline", trade);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error declining trade: {ex.Message}", ex);
            }
        }

        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Trade/Active/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code

                var result = await response.Content.ReadFromJsonAsync<List<ItemTrade>>(_options);

                return result ?? new List<ItemTrade>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<ItemTrade>();
            }
        }

        public IUserDetails GetCurrentUser()
        {
            return this.user;
        }

        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Trade/History/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code

                var result = await response.Content.ReadFromJsonAsync<List<ItemTrade>>(_options);

                return result ?? new List<ItemTrade>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<ItemTrade>();
            }
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Trade/Inventory/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code

                var result = await response.Content.ReadFromJsonAsync<List<Item>>(_options);

                return result ?? new List<Item>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<Item>();
            }
        }

        public async Task MarkTradeAsCompletedAsync(int tradeId)
        {
            try
            {
                var content = new StringContent(string.Empty);
                var response = await _httpClient.PatchAsync($"/api/Trade/Complete/{tradeId}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error completing trade: {ex.Message}", ex);
            }
        }

        public async Task TransferItemAsync(TransferItemTradeRequest tradeRequest)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"/api/Trade/TransferItem", tradeRequest);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error transfering item: {ex.Message}", ex);
            }
        }

        public async Task UpdateItemTradeAsync(ItemTrade trade)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"/api/Trade/Update", trade);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating trade: {ex.Message}", ex);
            }
        }

        public async Task UpdateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeUpdateException)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating trade: {tradeUpdateException.Message}");
                throw;
            }
        }
    }
}
