using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.PointShopItem;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Services;
using System.Net.Http.Json;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;
using SteamHub.ApiContract.Constants;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class PointShopServiceProxy : IPointShopService
    {
        private const int InitialIndexOfTransaction = 0;
        private const int IncrementingValue = 1;
        private const int InitialIndexAllItems = 0;
        private const int InitialIndexUserItems = 0;
        private const string FilterTypeAll = "All";

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public PointShopServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        public IUserDetails User { get; set; }

        public IUserDetails GetCurrentUser()
        {
            return this.User;
        }

        public async Task ActivateItemAsync(UpdateUserPointShopItemInventoryRequest request)
        {
            if (this.User == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/PointShop/Activate", request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error activating item: {exception.Message}", exception);
            }
        }

        public bool CanUserPurchaseItem(IUserDetails user, PointShopItem selectedItem, IEnumerable<PointShopItem> userItems)
        {
            if (user == null || selectedItem == null)
            {
                return false;
            }

            bool isAlreadyOwned = false;
            foreach (var item in userItems)
            {
                if (item.ItemIdentifier == selectedItem.ItemIdentifier)
                {
                    isAlreadyOwned = true;
                    break;
                }
            }

            bool hasEnoughPoints = user.PointsBalance >= selectedItem.PointPrice;

            return !isAlreadyOwned && hasEnoughPoints;
        }

        public async Task DeactivateItemAsync(UpdateUserPointShopItemInventoryRequest request)
        {
            if (this.User == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/PointShop/Deactivate", request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error activating item: {exception.Message}", exception);
            }
        }

        public async Task<List<PointShopItem>> GetAllItemsAsync()
        {
            try
            {
                var itemsResponse = await _httpClient.GetAsync("/api/PointShop");
                itemsResponse.EnsureSuccessStatusCode();
                var itemsResult = await itemsResponse.Content.ReadFromJsonAsync<List<PointShopItem>>(_options);

                // Ensure a non-null list is returned
                return itemsResult ?? new List<PointShopItem>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<List<PointShopItem>> GetAvailableItemsAsync(IUserDetails user)
        {
            var allItems = await this.GetAllItemsAsync();
            var userItems = await this.GetUserItemsAsync(this.User.UserId);

            var availableItems = new List<PointShopItem>();

            for (int indexForAllItems = InitialIndexAllItems; indexForAllItems < allItems.Count; indexForAllItems++)
            {
                bool isGameOwned = false;

                for (int indexForUsersItems = InitialIndexUserItems; indexForUsersItems < userItems.Count; indexForUsersItems++)
                {
                    if (allItems[indexForAllItems].ItemIdentifier == userItems[indexForUsersItems].ItemIdentifier)
                    {
                        isGameOwned = true;
                        break;
                    }
                }

                if (!isGameOwned)
                {
                    availableItems.Add(allItems[indexForAllItems]);
                }
            }

            return availableItems;
        }

        public async Task<List<PointShopItem>> GetFilteredItemsAsync(string filterType, string searchText, double minimumPrice, double maximumPrice)
        {
            try
            {
                var allItems = await this.GetAllItemsAsync();
                var userItems = await this.GetUserItemsAsync(this.User.UserId);
                var availableItems = new List<PointShopItem>();

                // Exclude items already owned by the user
                foreach (var item in allItems)
                {
                    bool isOwned = false;
                    foreach (var userItem in userItems)
                    {
                        if (userItem.ItemIdentifier == item.ItemIdentifier)
                        {
                            isOwned = true;
                            break;
                        }
                    }

                    if (!isOwned)
                    {
                        availableItems.Add(item);
                    }
                }

                // Apply type filter
                if (!string.IsNullOrEmpty(filterType) && filterType != FilterTypeAll)
                {
                    var filteredByType = new List<PointShopItem>();
                    foreach (var item in availableItems)
                    {
                        if (item.ItemType.Equals(filterType, StringComparison.OrdinalIgnoreCase))
                        {
                            filteredByType.Add(item);
                        }
                    }

                    availableItems = filteredByType;
                }

                // Apply price filter
                var filteredByPrice = new List<PointShopItem>();
                foreach (var item in availableItems)
                {
                    if (item.PointPrice >= minimumPrice && item.PointPrice <= maximumPrice)
                    {
                        filteredByPrice.Add(item);
                    }
                }

                availableItems = filteredByPrice;

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var filteredBySearch = new List<PointShopItem>();
                    foreach (var item in availableItems)
                    {
                        if ((item.Name != null && item.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (item.Description != null && item.Description.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            filteredBySearch.Add(item);
                        }
                    }

                    availableItems = filteredBySearch;
                }

                return availableItems;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFilteredItems: {exception.Message}");
                return new List<PointShopItem>();
            }
        }

        public async Task<Collection<PointShopItem>> GetUserItemsAsync(int userId)
        {
            try
            {
                var userItemsResponse = await _httpClient.GetAsync($"/api/PointShop/User/{userId}");
                userItemsResponse.EnsureSuccessStatusCode();
                var userItems = await userItemsResponse.Content.ReadFromJsonAsync<Collection<PointShopItem>>(_options);

                return userItems ?? new Collection<PointShopItem>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving user items: {exception.Message}", exception);
            }
        }

        public async Task PurchaseItemAsync(PurchasePointShopItemRequest purchaseRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/PointShop/Purchase", purchaseRequest);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error purchasing item: {exception.Message}", exception);
            }
        }

        public async Task<PointShopItem> ToggleActivationForItemAsync(int itemId, ObservableCollection<PointShopItem> userItems)
        {
            PointShopItem item = null;

            foreach (var userItem in userItems)
            {
                if (userItem.ItemIdentifier == itemId)
                {
                    item = userItem;
                    break;
                }
            }

            if (item == null)
            {
                return item;
            }

            if (item.IsActive)
            {
                var deactivateRequest = new UpdateUserPointShopItemInventoryRequest
                {
                    UserId = this.User.UserId,
                    PointShopItemId = itemId,
                    IsActive = false,
                };

                await this.DeactivateItemAsync(deactivateRequest);
                return item;
            }
            else
            {
                var activateRequest = new UpdateUserPointShopItemInventoryRequest
                {
                    UserId = this.User.UserId,
                    PointShopItemId = itemId,
                    IsActive = true,
                };

                await this.ActivateItemAsync(activateRequest);
                return item;
            }
        }

        public bool TryPurchaseItem(PointShopItem selectedItem, ObservableCollection<PointShopTransaction> transactionHistory, IUserDetails user, out PointShopTransaction newTransaction)
        {
            newTransaction = null;

            if (selectedItem == null || user == null)
            {
                return false;
            }

            // Purchase item
            try
            {
                // Check if transaction already exists
                bool transactionExists = false;
                for (int idexOfTransaction = InitialIndexOfTransaction; idexOfTransaction < transactionHistory.Count; idexOfTransaction++)
                {
                    var currentTransaction = transactionHistory[idexOfTransaction];
                    if (currentTransaction.ItemName == selectedItem.Name &&
                        Math.Abs(currentTransaction.PointsSpent - selectedItem.PointPrice) < PointShopConstants.MINMALDIFFERENCEVALUECOMPARISON)
                    {
                        transactionExists = true;
                        break;
                    }
                }

                if (!transactionExists)
                {
                    newTransaction = new PointShopTransaction(
                        transactionHistory.Count + IncrementingValue,
                        selectedItem.Name,
                        selectedItem.PointPrice,
                        selectedItem.ItemType,
                        User.UserId);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
