// <copyright file="PointShopService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserPointShopItemInventory;
    using SteamStore.Constants;
    using SteamStore.Data;
    using SteamStore.Models;
    using SteamStore.Repositories;
    using SteamStore.Repositories.Interfaces;
    using SteamStore.Services.Interfaces;

    public class PointShopService : IPointShopService
    {
        private const int InitialIndexOfTransaction = 0;
        private const int IncrementingValue = 1;
        private const int InitialIndexAllItems = 0;
        private const int InitialIndexUserItems = 0;
        private const string FILTERTYPEALL = "All";

        // private readonly IPointShopRepository repository;
        public IPointShopItemServiceProxy pointShopItemServiceProxy { get; set; }

        public IUserPointShopItemInventoryServiceProxy userPointShopItemInventoryServiceProxy { get; set; }

        public IUserServiceProxy userServiceProxy { get; set; }

        public User user { get; set; }

        public PointShopService(IPointShopItemServiceProxy pointShopItemServiceProxy, IUserPointShopItemInventoryServiceProxy userPointShopItemInventoryServiceProxy, IUserServiceProxy userServiceProxy, User user)
        {
            this.pointShopItemServiceProxy = pointShopItemServiceProxy;
            this.userPointShopItemInventoryServiceProxy = userPointShopItemInventoryServiceProxy;
            this.userServiceProxy = userServiceProxy;
            this.user = user;
        }

        public User GetCurrentUser()
        {
            return this.user;
        }

        public async Task<List<PointShopItem>> GetAllItems()
        {
            try
            {
                var allItems = await this.pointShopItemServiceProxy.GetPointShopItemsAsync();
                return allItems.PointShopItems
                    .Select(PointShopItemMapper.MapToPointShopItem)
                    .ToList();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<Collection<PointShopItem>> GetUserItems()
        {
            try
            {
                var userItems = await this.userPointShopItemInventoryServiceProxy.GetUserInventoryAsync(this.user.UserId);
                var allItems = await this.pointShopItemServiceProxy.GetPointShopItemsAsync();
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

        public async Task PurchaseItem(PointShopItem item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "Item cannot be null");
                }

                if (this.user == null)
                {
                    throw new InvalidOperationException("User is not initialized");
                }

                if (this.user.PointsBalance < item.PointPrice)
                {
                    throw new InvalidOperationException("User does not have enough points to purchase this item");
                }

                var purchaseRequest = new PurchasePointShopItemRequest
                {
                    UserId = this.user.UserId,
                    PointShopItemId = item.ItemIdentifier,
                };

                await this.userPointShopItemInventoryServiceProxy.PurchaseItemAsync(purchaseRequest);

                this.user.PointsBalance -= (float)item.PointPrice;

                // Update the user's points balance in the database
                var updateUserRequest = new UpdateUserRequest
                {
                    UserName = this.user.UserName,
                    Email = this.user.Email,
                    WalletBalance = this.user.WalletBalance,
                    PointsBalance = this.user.PointsBalance,
                    Role = (RoleEnum)this.user.UserRole,
                };

                await this.userServiceProxy.UpdateUserAsync(this.user.UserId, updateUserRequest);

            }
            catch (Exception exception)
            {
                throw new Exception($"Error purchasing item: {exception.Message}", exception);
            }
        }

        public async Task ActivateItem(PointShopItem item)
        {
            try
            {
                //this.repository.ActivateItem(item);
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "Item cannot be null");
                }
                if (this.user == null)
                {
                    throw new InvalidOperationException("User is not initialized");
                }

                var activateRequest = new UpdateUserPointShopItemInventoryRequest
                {
                    UserId = this.user.UserId,
                    PointShopItemId = item.ItemIdentifier,
                    IsActive = true,
                };

                await this.userPointShopItemInventoryServiceProxy.UpdateItemStatusAsync(activateRequest);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error activating item: {exception.Message}", exception);
            }
        }

        public async Task DeactivateItem(PointShopItem item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "Item cannot be null");
                }
                if (this.user == null)
                {
                    throw new InvalidOperationException("User is not initialized");
                }

                var activateRequest = new UpdateUserPointShopItemInventoryRequest
                {
                    UserId = this.user.UserId,
                    PointShopItemId = item.ItemIdentifier,
                    IsActive = false,
                };

                await this.userPointShopItemInventoryServiceProxy.UpdateItemStatusAsync(activateRequest);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error deactivating item: {exception.Message}", exception);
            }
        }

        public async Task<List<PointShopItem>> GetFilteredItems(string filterType, string searchText, double minimumPrice, double maximumPrice)
        {
            try
            {
                var allItems = await this.GetAllItems();
                var userItems = await this.GetUserItems();
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
                if (!string.IsNullOrEmpty(filterType) && filterType != FILTERTYPEALL)
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

        public bool CanUserPurchaseItem(User user, PointShopItem selectedItem, IEnumerable<PointShopItem> userItems)
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

        public async Task<List<PointShopItem>> GetAvailableItems(User user)
        {
            var allItems = await this.GetAllItems();
            var userItems = await this.GetUserItems();

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

        public bool TryPurchaseItem(PointShopItem selectedItem, ObservableCollection<PointShopTransaction> transactionHistory, User user, out PointShopTransaction newTransaction)
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
                        selectedItem.ItemType);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PointShopItem> ToggleActivationForItem(int itemId, ObservableCollection<PointShopItem> userItems)
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
                await this.DeactivateItem(item);
                return item;
            }
            else
            {
                await this.ActivateItem(item);
                return item;
            }
        }

        //public void ResetUserInventory()
        //{
        //    this.repository.ResetUserInventory();
        //}
    }
}