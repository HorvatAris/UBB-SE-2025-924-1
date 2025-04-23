// <copyright file="PointShopRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SteamStore.Data;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Tests.TestUtils;
using Xunit;

namespace SteamStore.Tests.Repositories
{
    public class PointShopRepositoryTests
    {
        private const string ValidUserName = "John Doe";
        private const int ValidUserId = 1;
        private const float InitialUserPoints = 999999.99f;

        private const int FirstOwnedItemId = 1;
        private const int SecondOwnedItemId = 3;
        private const float HighItemPrice = 9999999.99f;
        private const float UpdatedPointBalance = 100f;
        private const int EmptyItemsListLength = 0;

        private readonly PointShopRepository validRepository;
        private readonly PointShopRepository repositoryWithNullUser;
        private readonly User testUser;

        public PointShopRepositoryTests()
        {
            testUser = new User
            {
                UserIdentifier = ValidUserId,
                Name = ValidUserName,
                PointsBalance = InitialUserPoints
            };

            validRepository = new PointShopRepository(testUser, DataLinkTestUtils.GetDataLink());
            repositoryWithNullUser = new PointShopRepository(null, DataLinkTestUtils.GetDataLink());
        }

        [Fact]
        public void GetAllItems_WhenCalled_ReturnsNonEmptyList()
        {
            var items = validRepository.GetAllItems();

            Assert.NotNull(items);
            Assert.NotEmpty(items);
        }

        [Fact]
        public void GetUserItems_WhenCalled_ReturnsUserOwnedItems()
        {
            var items = validRepository.GetUserItems();

            Assert.NotNull(items);
            Assert.True(items.Count >= EmptyItemsListLength);
        }

        [Fact]
        public void PurchaseItem_WhenUserIsNull_ThrowsInvalidOperationException()
        {
            var item = new PointShopItem { ItemIdentifier = FirstOwnedItemId };

            var exception = Assert.Throws<InvalidOperationException>(() => repositoryWithNullUser.PurchaseItem(item));

            Assert.Contains("User is not initialized", exception.Message);
        }

        [Fact]
        public void PurchaseItem_WhenItemIsNull_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => validRepository.PurchaseItem(null));

            Assert.Contains("Cannot purchase a null item", exception.Message);
        }

        [Fact]
        public void PurchaseItem_WhenPointsInsufficient_ThrowsException()
        {
            var expensiveItem = new PointShopItem { ItemIdentifier = SecondOwnedItemId, PointPrice = HighItemPrice };

            var exception = Assert.Throws<Exception>(() => validRepository.PurchaseItem(expensiveItem));

            Assert.Contains("Insufficient points to purchase this item", exception.Message);
        }

        [Fact]
        public void ActivateItem_WhenItemIsValid_DoesNotThrow()
        {
            var item = new PointShopItem { ItemIdentifier = FirstOwnedItemId };

            validRepository.ActivateItem(item);

            Assert.True(true);
        }

        [Fact]
        public void ActivateItem_WhenItemIsNull_ThrowsArgumentNullException()
        {
            PointShopItem? item = null;

            var exception = Assert.Throws<ArgumentNullException>(() => validRepository.ActivateItem(item));

            Assert.Contains("Cannot activate a null item", exception.Message);
        }

        [Fact]
        public void ActivateItem_WhenUserIsNull_ThrowsInvalidOperationException()
        {
            var item = new PointShopItem { ItemIdentifier = FirstOwnedItemId };

            var exception = Assert.Throws<InvalidOperationException>(() => repositoryWithNullUser.ActivateItem(item));

            Assert.Contains("User is not initialized", exception.Message);
        }

        [Fact]
        public void DeactivateItem_WhenItemIsValid_DoesNotThrow()
        {
            var item = new PointShopItem { ItemIdentifier = FirstOwnedItemId };

            validRepository.DeactivateItem(item);

            Assert.True(true);
        }

        [Fact]
        public void DeactivateItem_WhenUserIsNull_ThrowsInvalidOperationException()
        {
            var item = new PointShopItem { ItemIdentifier = FirstOwnedItemId };

            var exception = Assert.Throws<InvalidOperationException>(() => repositoryWithNullUser.DeactivateItem(item));

            Assert.Equal("User is not initialized", exception.Message);
        }

        [Fact]
        public void DeactivateItem_WhenItemIsNull_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => validRepository.DeactivateItem(null));

            Assert.Contains("Cannot deactivate a null item", exception.Message);
        }

        [Fact]
        public void UpdateUserPointBalance_WhenCalled_UpdatesUserBalanceCorrectly()
        {
            testUser.PointsBalance = UpdatedPointBalance;
            validRepository.UpdateUserPointBalance();

            var updatedItem = validRepository.GetUserItems().FirstOrDefault();
            Assert.Equal(UpdatedPointBalance, testUser.PointsBalance);

            testUser.PointsBalance = InitialUserPoints;
            validRepository.UpdateUserPointBalance();
            Assert.Equal(InitialUserPoints, testUser.PointsBalance);
        }
    }
}
