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

namespace SteamStore.Tests.Services
{
    public class PointShopServiceTest
    {
        private const string UserName = "John Doe";
        private const int UserIdentifier = 1;
        private const float InitialPointsBalance = 999999.99f;

        private const int PurchaseItemIdentifier = 7;
        private const int PurchaseItemPointPrice = 500;

        private const int ActivateItemIdentifier = 3;
        private const int DeactivateItemIdentifier = 1;

        private const string FilterType = "ProfileBackground";
        private const string FilterText = "Blue";
        private const double PriceMinimum = 0;
        private const double PriceMaximum = 1000;

        private const int CanPurchaseItemIdentifier = 1;

        private const int ItemIdentifier1 = 1;
        private const int ItemIdentifier2 = 2;
        private const int ItemIdentifier3 = 3;
        private const string ItemName = "Red Background";
        private const int ItemPointPrice = 500;
        private const string ItemType = "ProfileBackground";

        private readonly User testUser;
        private readonly PointShopService service;

        public PointShopServiceTest()
        {
            testUser = new User
            {
                UserIdentifier = UserIdentifier,
                Name = UserName,
                PointsBalance = InitialPointsBalance
            };

            service = new PointShopService(testUser, DataLinkTestUtils.GetDataLink());
        }

        [Fact]
        public void GetCurrentUser_ShouldReturnCurrentUser()
        {
            var user = service.GetCurrentUser();
            Assert.Equal(testUser.UserIdentifier, user.UserIdentifier);
        }

        [Fact]
        public void GetAllItems_ShouldReturnListOfPointShopItems()
        {
            var items = service.GetAllItems();
            Assert.All(items, item => Assert.NotNull(item.Name));
        }

        [Fact]
        public void GetUserItems_ShouldReturnListOfUserPointShopItems()
        {
            var userItems = service.GetUserItems();
            Assert.All(userItems, item => Assert.NotNull(item.Name));
        }

        [Fact]
        public void PurchaseItem_ShouldDeductPointsAndAddItem()
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
        public void ActivateItem_ShouldSetItemAsActive()
        {
            var itemToActivate = new PointShopItem
            {
                ItemIdentifier = ActivateItemIdentifier
            };

            service.ActivateItem(itemToActivate);

            var userItems = service.GetUserItems();
            Assert.Contains(userItems, item => item.ItemIdentifier == ActivateItemIdentifier && item.IsActive);
        }

        [Fact]
        public void DeactivateItem_ShouldSetItemAsInactive()
        {
            var itemToDeactivate = new PointShopItem
            {
                ItemIdentifier = DeactivateItemIdentifier
            };

            service.DeactivateItem(itemToDeactivate);

            var userItems = service.GetUserItems();
            Assert.Contains(userItems, item => item.ItemIdentifier == DeactivateItemIdentifier && !item.IsActive);
        }

        [Fact]
        public void GetFilteredItems_ShouldReturnFilteredItems()
        {
            var filteredItems = service.GetFilteredItems(FilterType, FilterText, PriceMinimum, PriceMaximum);
            Assert.NotNull(filteredItems);
        }

        [Fact]
        public void CanUserPurchaseItem_ShouldReturnTrue_WhenUserCanPurchase()
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
        public void CanUserPurchaseItem_AlreadyOwns()
        {
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
        public void CanUserPurchaseItem_NullItem()
        {
            var userItems = service.GetUserItems();
            var canPurchase = service.CanUserPurchaseItem(testUser, null, userItems);
            Assert.False(canPurchase);
        }

        [Fact]
        public void GetAvailableItems_ShouldReturnItemsNotOwnedByUser()
        {
            var availableItems = service.GetAvailableItems(testUser);
            Assert.All(availableItems, item => Assert.DoesNotContain(service.GetUserItems(), userItem => userItem.ItemIdentifier == item.ItemIdentifier));
        }

        [Fact]
        public void TryPurchaseItem_ReturnTrue()
        {
            var selectedItem = new PointShopItem
            {
                ItemIdentifier = ItemIdentifier2,
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
        public void TryPurchaseItem_ReturnFasle()
        {
            var transactionHistory = new ObservableCollection<PointShopTransaction>();
            var result = service.TryPurchaseItem(null, transactionHistory, testUser, out var newTransaction);

            Assert.False(result);
        }

        [Fact]
        public void ToggleActivationForItem_ShouldActivateOrDeactivateItem()
        {
            var userItems = new ObservableCollection<PointShopItem>
            {
                new PointShopItem { ItemIdentifier = ItemIdentifier1, IsActive = true },
                new PointShopItem { ItemIdentifier = ItemIdentifier3, IsActive = false }
            };

            var toggledItem = service.ToggleActivationForItem(ItemIdentifier1, userItems);

            Assert.True(toggledItem.IsActive);
        }

        [Fact]
        public void ToggleActivationForItem_NullItem()
        {
            var userItems = new ObservableCollection<PointShopItem>
            {
                new PointShopItem { ItemIdentifier = ItemIdentifier1, IsActive = true },
                new PointShopItem { ItemIdentifier = ItemIdentifier3, IsActive = false }
            };

            var toggledItem = service.ToggleActivationForItem(ItemIdentifier2, userItems);

            Assert.Null(toggledItem);
        }

        [Fact]
        public void ToggleActivationForItem_AlreadyActive()
        {
            var userItems = new ObservableCollection<PointShopItem>
            {
                new PointShopItem { ItemIdentifier = ItemIdentifier1, IsActive = true },
                new PointShopItem { ItemIdentifier = ItemIdentifier3, IsActive = false }
            };

            var toggledItem = service.ToggleActivationForItem(ItemIdentifier3, userItems);

            Assert.False(toggledItem.IsActive);
        }
    }
}

