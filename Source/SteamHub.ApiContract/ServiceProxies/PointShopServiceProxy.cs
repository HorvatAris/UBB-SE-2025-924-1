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

        public async Task ActivateItemAsync(PointShopItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }

            if (this.User == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            var request = new UpdateUserPointShopItemInventoryRequest
            {
                UserId = this.User.UserId,
                PointShopItemId = item.ItemIdentifier,
                IsActive = true
            };

            try
            {
                var response = await _httpClient.PutAsJsonAsync("/api/UserPointShopItemInventory/update", request);
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

        public async Task DeactivateItemAsync(PointShopItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }

            if (this.User == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            var request = new UpdateUserPointShopItemInventoryRequest
            {
                UserId = this.User.UserId,
                PointShopItemId = item.ItemIdentifier,
                IsActive = false
            };

            try
            {
                var response = await _httpClient.PutAsJsonAsync("/api/UserPointShopItemInventory/update", request);
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
                var response = await _httpClient.GetAsync("/api/PointShopItems");
                response.EnsureSuccessStatusCode();
                var itemsResponse = await response.Content.ReadFromJsonAsync<GetPointShopItemsResponse>(_options);

                // Map PointShopItemResponse to PointShopItem using PointShopItemMapper
                var pointShopItems = itemsResponse?.PointShopItems?
                    .Select(item => PointShopItemMapper.MapToPointShopItem(item))
                    .ToList();

                // Ensure a non-null list is returned
                return pointShopItems ?? new List<PointShopItem>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<List<PointShopItem>> GetAvailableItemsAsync(IUserDetails user)
        {
            var allItems = await this.GetAllItemsAsync();
            var userItems = await this.GetUserItemsAsync();

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
                var userItems = await this.GetUserItemsAsync();
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

        public async Task<Collection<PointShopItem>> GetUserItemsAsync()
        {
            try
            {
                var responseUserItems = await _httpClient.GetAsync($"/api/UserPointShopItemInventory/{this.User.UserId}");
                responseUserItems.EnsureSuccessStatusCode();
                var userItems = await responseUserItems.Content.ReadFromJsonAsync<GetUserPointShopItemInventoryResponse>(_options);

                if (userItems == null)
                {
                    throw new InvalidOperationException("Invalid response from GetUserItemsAsync");
                }

                var responseAllItems = await _httpClient.GetAsync("/api/PointShopItems");
                responseAllItems.EnsureSuccessStatusCode();

                var allItems = await responseAllItems.Content.ReadFromJsonAsync<GetPointShopItemsResponse>(_options);

                if (allItems == null)
                {
                    throw new InvalidOperationException("Invalid response from GetAllItemsAsync");
                }

                var userPointShopItems = userItems.UserPointShopItemsInventory
                        .Select(userItem =>
                        {
                            var pointShopItem = allItems.PointShopItems
                                .FirstOrDefault(item => item.PointShopItemId == userItem.PointShopItemId);

                            if (pointShopItem != null)
                            {
                                var mappedItem = PointShopItemMapper.MapToPointShopItem(pointShopItem);
                                mappedItem.IsActive = userItem.IsActive; // Update IsActive status
                                return mappedItem;
                            }

                            return null;
                        })
                        .Where(item => item != null)
                        .ToList();
                return new Collection<PointShopItem>(userPointShopItems);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving user items: {exception.Message}", exception);
            }
        }

        public async Task PurchaseItemAsync(PointShopItem item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "Item cannot be null");
                }

                if (this.User == null)
                {
                    throw new InvalidOperationException("User is not initialized");
                }

                if (this.User.PointsBalance < item.PointPrice)
                {
                    throw new InvalidOperationException("User does not have enough points to purchase this item");
                }

                var purchaseRequest = new PurchasePointShopItemRequest
                {
                    UserId = this.User.UserId,
                    PointShopItemId = item.ItemIdentifier,
                };

                var response = await _httpClient.PostAsJsonAsync("/api/UserPointShopItemInventory/purchase", purchaseRequest);
                response.EnsureSuccessStatusCode();

                this.User.PointsBalance -= (float)item.PointPrice;

                // Update the user's points balance in the database
                var updateUserRequest = new UpdateUserRequest
                {
                    UserName = this.User.UserName,
                    Email = this.User.Email,
                    WalletBalance = this.User.WalletBalance,
                    PointsBalance = this.User.PointsBalance,
                    Role = (RoleEnum)this.User.UserRole,
                };

                var responseUser = await _httpClient.PutAsJsonAsync($"/api/Users/{this.User.UserId}", updateUserRequest);
                responseUser.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
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
                await this.DeactivateItemAsync(item);
                return item;
            }
            else
            {
                await this.ActivateItemAsync(item);
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
