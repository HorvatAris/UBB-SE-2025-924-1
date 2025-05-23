// <copyright file="PointShopService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Constants;
    using SteamHub.ApiContract.Context.Repositories;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.PointShopItem;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserPointShopItemInventory;
    using SteamHub.ApiContract.Proxies;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;

    public class PointShopService : IPointShopService
    {
        private const int InitialIndexOfTransaction = 0;
        private const int IncrementingValue = 1;
        private const int InitialIndexAllItems = 0;
        private const int InitialIndexUserItems = 0;
        private const string FilterTypeAll = "All";

        public PointShopService(IPointShopItemRepository pointShopItemRepository, IUserPointShopItemInventoryRepository userPointShopItemInventoryRepository, IUserRepository userRepository)
        {
            this.PointShopItemRepository = pointShopItemRepository;
            this.UserPointShopItemInventoryRepository = userPointShopItemInventoryRepository;
            this.UserRepository = userRepository;
        }

        public IPointShopItemRepository PointShopItemRepository { get; set; }

        public IUserPointShopItemInventoryRepository UserPointShopItemInventoryRepository { get; set; }

        public IUserRepository UserRepository { get; set; }

        public async Task<List<PointShopItem>> GetAllItemsAsync()
        {
            try
            {
                var allItems = await this.PointShopItemRepository.GetPointShopItemsAsync();
                return allItems.PointShopItems
                    .Select(PointShopItemMapper.MapToPointShopItem)
                    .ToList();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<Collection<PointShopItem>> GetUserItemsAsync(int userId)
        {
            try
            {
                var userItems = await this.UserPointShopItemInventoryRepository.GetUserInventoryAsync(userId);
                var allItems = await this.PointShopItemRepository.GetPointShopItemsAsync();
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

        public async Task PurchaseItemAsync(PurchasePointShopItemRequest itemRequest)
        {
            try
            {
                PointShopItemResponse item = await this.PointShopItemRepository.GetPointShopItemByIdAsync(itemRequest.PointShopItemId);
                UserResponse user = await this.UserRepository.GetUserByIdAsync(itemRequest.UserId);

                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "Item cannot be null");
                }

                if (user == null)
                {
                    throw new InvalidOperationException("User is not initialized");
                }

                if (user.PointsBalance < item.PointPrice)
                {
                    throw new InvalidOperationException("User does not have enough points to purchase this item");
                }

                var purchaseRequest = new PurchasePointShopItemRequest
                {
                    UserId = user.UserId,
                    PointShopItemId = item.PointShopItemId,
                };

                await this.UserPointShopItemInventoryRepository.PurchaseItemAsync(purchaseRequest);

                user.PointsBalance -= (float)item.PointPrice;

                // Update the user's points balance in the database
                var updateUserRequest = new UpdateUserRequest
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    WalletBalance = user.WalletBalance,
                    PointsBalance = user.PointsBalance,
                    Role = (RoleEnum)user.Role,
                };

                await this.UserRepository.UpdateUserAsync(user.UserId, updateUserRequest);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error purchasing item: {exception.Message}", exception);
            }
        }

        public async Task ActivateItemAsync(UpdateUserPointShopItemInventoryRequest activateRequest)
        {
            try
            {
                await this.UserPointShopItemInventoryRepository.UpdateItemStatusAsync(activateRequest);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error activating item: {exception.Message}", exception);
            }
        }

        public async Task DeactivateItemAsync(UpdateUserPointShopItemInventoryRequest deactivateRequest)
        {
            try
            {
                await this.UserPointShopItemInventoryRepository.UpdateItemStatusAsync(deactivateRequest);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error deactivating item: {exception.Message}", exception);
            }
        }

        public async Task<List<PointShopItem>> GetFilteredItemsAsync(string filterType, string searchText, double minimumPrice, double maximumPrice)
        {
            try
            {
                var allItems = await this.GetAllItemsAsync();
                var userItems = await this.GetUserItemsAsync(0);
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

        public async Task<List<PointShopItem>> GetAvailableItemsAsync(IUserDetails user)
        {
            var allItems = await this.GetAllItemsAsync();
            var userItems = await this.GetUserItemsAsync(0);

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
                        0);
                }

                return true;
            }
            catch
            {
                return false;
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
                    UserId = 0,
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
                    UserId = 0,
                    PointShopItemId = itemId,
                    IsActive = false,
                };

                await this.ActivateItemAsync(activateRequest);
                return item;
            }
        }

        public IUserDetails GetCurrentUser()
        {
            throw new NotImplementedException();
        }
    }
}