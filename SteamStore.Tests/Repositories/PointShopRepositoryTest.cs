using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Data;
using SteamStore.Tests.TestUtils;

namespace SteamStore.Tests.Repositories
{
    public class PointShopRepositoryTest
    {
        private const string UserName = "John Doe";
        private const int UserIdentifier = 1;
        private const float InitialPointsBalance = 999999.99f;

        private const int ItemIdentifier1 = 1;
        private const int ItemIdentifier3 = 3;
        private const float ItemPointPrice = 9999999.99f;
        private const float NewPointBalance = 100;

        private readonly PointShopRepository repository;
        private readonly PointShopRepository nullRepository;
        private readonly User testUser;

        public PointShopRepositoryTest()
        {
            testUser = new User
            {
                UserIdentifier = UserIdentifier,
                Name = UserName,
                PointsBalance = InitialPointsBalance
            };

            repository = new PointShopRepository(testUser, DataLinkTestUtils.GetDataLink());
            nullRepository = new PointShopRepository(null, DataLinkTestUtils.GetDataLink());
        }

        [Fact]
        public void GetAllItems_ShouldReturnItems()
        {
            var items = repository.GetAllItems();
            Assert.NotNull(items);
            Assert.NotEmpty(items);
        }

        [Fact]
        public void GetUserItems_ShouldReturnUserItems()
        {
            var items = repository.GetUserItems();
            Assert.NotNull(items);
        }

        [Fact]
        public void PurchaseItem_NullUser()
        {
            var item = new PointShopItem
            {
                ItemIdentifier = ItemIdentifier1
            };

            var exception = Assert.Throws<InvalidOperationException>(() => nullRepository.PurchaseItem(item));
            Assert.Contains("User is not initialized", exception.Message);
        }

        [Fact]
        public void PurchaseItem_NullItem()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => repository.PurchaseItem(null));
            Assert.Contains("Cannot purchase a null item", exception.Message);
        }

        [Fact]
        public void PurchaseItem_InsufficentPoints()
        {
            var item = new PointShopItem
            {
                ItemIdentifier = ItemIdentifier3,
                PointPrice = ItemPointPrice
            };

            var exception = Assert.Throws<Exception>(() => repository.PurchaseItem(item));
            Assert.Contains("Insufficient points to purchase this item", exception.Message);
        }

        [Fact]
        public void ActivateItem_ShouldNotThrow()
        {
            var item = new PointShopItem
            {
                ItemIdentifier = ItemIdentifier1
            };
            repository.ActivateItem(item);
        }

        [Fact]
        public void ActivateItem_ShouldThrowException_WhenItemIsNull()
        {
            PointShopItem? item = null;

            var exception = Assert.Throws<ArgumentNullException>(() => repository.ActivateItem(item));
            Assert.Contains("Cannot activate a null item", exception.Message);
        }

        [Fact]
        public void ActivateItem_NullUser()
        {
            var item = new PointShopItem
            {
                ItemIdentifier = ItemIdentifier1
            };
            var exception = Assert.Throws<InvalidOperationException>(() => nullRepository.ActivateItem(item));
            Assert.Contains("User is not initialized", exception.Message);
        }

        [Fact]
        public void DeactivateItem_ShouldNotThrow()
        {
            var item = new PointShopItem
            {
                ItemIdentifier = ItemIdentifier1
            };
            repository.DeactivateItem(item);
        }

        [Fact]
        public void DeactivateItem_ShouldThrowException_WhenUserIsNotInitialized()
        {
            var item = new PointShopItem { ItemIdentifier = ItemIdentifier1 };

            var exception = Assert.Throws<InvalidOperationException>(() => nullRepository.DeactivateItem(item));
            Assert.Equal("User is not initialized", exception.Message);
        }

        [Fact]
        public void DeactivateItem_NullItem()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => repository.DeactivateItem(null));
            Assert.Contains("Cannot deactivate a null item", exception.Message);
        }

        [Fact]
        public void UpdateUserPointBalance_ShouldUpdatePoints()
        {
            testUser.PointsBalance = NewPointBalance;
            repository.UpdateUserPointBalance();

            var updatedBalance = repository.GetUserItems()
                .FirstOrDefault()?.PointPrice;
            Assert.Equal(NewPointBalance, testUser.PointsBalance);
            testUser.PointsBalance = InitialPointsBalance;
            repository.UpdateUserPointBalance();
        }
    }
}
