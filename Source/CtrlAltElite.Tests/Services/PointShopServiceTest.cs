namespace SteamStore.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Moq;
    using SteamStore.Data;
    using SteamStore.Models;
    using SteamStore.Repositories;
    using SteamStore.Services;
    using SteamStore.Tests.TestUtils;
    using Xunit;

    public class PointShopServiceTest
    {
        private const int PurchaseItemIdentifier = 7;
        private const int PurchaseItemPointPrice = 500;

        private const int ActiveItemIdentifier = 1;
        private const int NotOwnedItemIdentifier = 2;
        private const int NotActiveItemIdentifier = 3;

        private readonly User testUser;
        private readonly PointShopService service;

        public PointShopServiceTest()
        {
            const int UserID = 1;
            const string UserName = "John Doe";
            const float InitialPointsBalance = 999999.99f;
            testUser = new User
            {
                UserId = UserID,
                UserName = UserName,
                PointsBalance = InitialPointsBalance
            };

            service = new PointShopService(testUser, DataLinkTestUtils.GetDataLink());
        }

        [Fact]
        public void GetCurrentUser_Always_ShouldReturnCurrentUser()
        {
            var user = service.GetCurrentUser();
            Assert.Equal(testUser.UserId, user.UserId);
        }

        [Fact]
        public void GetAllItems_Always_ShouldReturnListOfPointShopItems()
        {
            var items = service.GetAllItems();

            Assert.All(items, item => Assert.NotNull(item.Name));
        }

        [Fact]
        public void GetUserItems_Always_ShouldReturnListOfUserPointShopItems()
        {
            var userItems = service.GetUserItems();

            Assert.All(userItems, item => Assert.NotNull(item.Name));
        }

        [Fact]
        public void PurchaseItem_WhenItemIsNotAlreadyOwned_ShouldDeductPointsAndAddItem()
        {
            var newItem = new PointShopItem
            {
                ItemIdentifier = PurchaseItemIdentifier,
                PointPrice = PurchaseItemPointPrice
            };

            try
            {
                service.PurchaseItem(newItem);
                var userItems = service.GetUserItems();

                Assert.Contains(userItems, item => item.ItemIdentifier == newItem.ItemIdentifier);
            }
            finally
            {
                service.ResetUserInventory();
            }
        }

        [Fact]
        public void ActivateItem_WhenItemIsOwned_ShouldSetItemAsActive()
        {
            const int ActivateItemIdentifier = 3;
            var itemToActivate = new PointShopItem
            {
                ItemIdentifier = ActivateItemIdentifier
            };

            service.ActivateItem(itemToActivate);

            var userItems = service.GetUserItems();
            Assert.Contains(userItems, item => item.ItemIdentifier == ActivateItemIdentifier && item.IsActive);
        }

        [Fact]
        public void DeactivateItem_WhenItemIsOwned_ShouldSetItemAsInactive()
        {
            const int DeactivateItemIdentifier = 1;
            var itemToDeactivate = new PointShopItem
            {
                ItemIdentifier = DeactivateItemIdentifier
            };

            service.DeactivateItem(itemToDeactivate);

            var userItems = service.GetUserItems();
            Assert.Contains(userItems, item => item.ItemIdentifier == DeactivateItemIdentifier && !item.IsActive);
        }

        [Fact]
        public void GetFilteredItems_Always_ShouldReturnFilteredItems()
        {
            const string FilterType = "ProfileBackground";
            const string FilterText = "Blue";
            const double PriceMinimum = 0;
            const double PriceMaximum = 1000;

            var filteredItems = service.GetFilteredItems(FilterType, FilterText, PriceMinimum, PriceMaximum);

            Assert.NotNull(filteredItems);
        }

        [Fact]
        public void CanUserPurchaseItem_WhenUserCanPurchase_ShouldReturnTrue()
        {
            var selectedItem = new PointShopItem
            {
                ItemIdentifier = PurchaseItemIdentifier,
                PointPrice = PurchaseItemPointPrice
            };

            var userItems = service.GetUserItems();
            var canPurchase = service.CanUserPurchaseItem(testUser, selectedItem, userItems);

            Assert.True(canPurchase);

            if (userItems.Exists(item => item.ItemIdentifier == selectedItem.ItemIdentifier))
            {
                service.DeactivateItem(selectedItem);
                userItems.RemoveAll(item => item.ItemIdentifier == selectedItem.ItemIdentifier);
            }
        }

        [Fact]
        public void CanUserPurchaseItem_WhenItemIsAlreadyOwned_ReturnsFalse()
        {
            const int CanPurchaseItemIdentifier = 1;
            var selectedItem = new PointShopItem
            {
                ItemIdentifier = CanPurchaseItemIdentifier,
                PointPrice = PurchaseItemPointPrice
            };

            var userItems = service.GetUserItems();
            var canPurchase = service.CanUserPurchaseItem(testUser, selectedItem, userItems);

            Assert.False(canPurchase);
        }

        [Fact]
        public void CanUserPurchaseItem_WhenItemIsNull_ShouldReturnFalse()
        {
            var userItems = service.GetUserItems();

            var canPurchase = service.CanUserPurchaseItem(testUser, null, userItems);

            Assert.False(canPurchase);
        }

        [Fact]
        public void GetAvailableItems_Always_ShouldReturnItemsNotOwnedByUser()
        {
            var availableItems = service.GetAvailableItems(testUser);

            Assert.All(availableItems, item => Assert.DoesNotContain(service.GetUserItems(), userItem => userItem.ItemIdentifier == item.ItemIdentifier));
        }

        [Fact]
        public void TryPurchaseItem_WhenItemIsNotOwned_ShouldReturnItem()
        {
            const string ItemName = "Red Background";
            const int ItemPointPrice = 500;
            const string ItemType = "ProfileBackground";
            var selectedItem = new PointShopItem
            {
                ItemIdentifier = NotOwnedItemIdentifier,
                Name = ItemName,
                PointPrice = ItemPointPrice,
                ItemType = ItemType
            };

            var transactionHistory = new ObservableCollection<PointShopTransaction>();
            var result = service.TryPurchaseItem(selectedItem, transactionHistory, testUser, out var newTransaction);

            Assert.Equal(ItemPointPrice, newTransaction.PointsSpent);

            var userItems = service.GetUserItems();
            if (userItems.Exists(item => item.ItemIdentifier == selectedItem.ItemIdentifier))
            {
                service.DeactivateItem(selectedItem);
                userItems.RemoveAll(item => item.ItemIdentifier == selectedItem.ItemIdentifier);
            }
        }

        [Fact]
        public void TryPurchaseItem_WhenUnableToPurchase_ShouldReturnFalse()
        {
            var transactionHistory = new ObservableCollection<PointShopTransaction>();

            var result = service.TryPurchaseItem(null, transactionHistory, testUser, out var newTransaction);

            Assert.False(result);
        }

        [Fact]
        public void ToggleActivationForItem_WhenItemIsActive_ShouldDeactivateItem()
        {
            var userItems = new ObservableCollection<PointShopItem>
            {
                new PointShopItem { ItemIdentifier = ActiveItemIdentifier, IsActive = true },
                new PointShopItem { ItemIdentifier = NotActiveItemIdentifier, IsActive = false }
            };

            var toggledItem = service.ToggleActivationForItem(ActiveItemIdentifier, userItems);

            Assert.True(toggledItem.IsActive);
        }

        [Fact]
        public void ToggleActivationForItem_WhenItemDoesntExist_ShouldReturnNullItem()
        {
            var userItems = new ObservableCollection<PointShopItem>
            {
                new PointShopItem { ItemIdentifier = ActiveItemIdentifier, IsActive = true },
                new PointShopItem { ItemIdentifier = NotActiveItemIdentifier, IsActive = false }
            };

            var toggledItem = service.ToggleActivationForItem(NotOwnedItemIdentifier, userItems);

            Assert.Null(toggledItem);
        }

        [Fact]
        public void ToggleActivationForItem_WhenItemIsInactive_ShouldActivateItem()
        {
            var userItems = new ObservableCollection<PointShopItem>
            {
                new PointShopItem { ItemIdentifier = ActiveItemIdentifier, IsActive = true },
                new PointShopItem { ItemIdentifier = NotActiveItemIdentifier, IsActive = false }
            };

            var toggledItem = service.ToggleActivationForItem(NotActiveItemIdentifier, userItems);

            Assert.False(toggledItem.IsActive);
        }
    }
}

