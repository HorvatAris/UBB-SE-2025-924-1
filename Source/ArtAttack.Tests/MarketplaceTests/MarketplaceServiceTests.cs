using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Steampunks.Services.MarketplaceService;
using Steampunks.Repository.Marketplace;
using Steampunks.Domain.Entities;
using Steampunks.DataLink;

namespace Steampunks_Tests.MarketplaceTests
{
    [TestFixture]
    public class MarketplaceServiceTests
    {
        private Mock<IMarketplaceRepository> mockRepository;
        private User testUser;
        private MarketplaceService service;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new Mock<IMarketplaceRepository>();
            testUser = new User("newUser");
            mockRepository.Setup(d => d.GetCurrentUser()).Returns(testUser);

            service = new MarketplaceService(mockRepository.Object);
        }

        [Test]
        public void Constructor_NullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketplaceService(null));
        }

        [Test]
        public void GetCurrentUser_ReturnsCurrentUser()
        {
            var result = service.GetCurrentUser();
            Assert.AreEqual(testUser, result);
        }

        [Test]
        public void SetCurrentUser_ValidUser_SetsCurrentUser()
        {
            var newUser = new User("newUser");
            service.SetCurrentUser(newUser);
            Assert.AreEqual(newUser, service.GetCurrentUser());
        }

        [Test]
        public void SetCurrentUser_NullUser_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => service.SetCurrentUser(null));
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsUserList()
        {
            var expected = new List<User> { new User("newUser"), new User("newUser2") };
            mockRepository.Setup(d => d.GetAllUsersAsync()).ReturnsAsync(expected);

            var result = await service.GetAllUsersAsync();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task GetAllListingsAsync_ReturnsItemList()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var items = new List<Item> {
                new Item("New Item1", game, 5.99f, "Item Desc"),
                new Item("New Item2", game, 5.99f, "Item Desc")
            };
            mockRepository.Setup(r => r.GetAllListedItemsAsync()).ReturnsAsync(items);

            var result = await service.GetAllListingsAsync();
            Assert.AreEqual(items, result);
        }

        [Test]
        public void GetListingsByGameAsync_NullGame_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => service.GetListingsByGameAsync(null));
        }

        [Test]
        public async Task GetListingsByGameAsync_ReturnsListings()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var listings = new List<Item> { new Item("New Item", game, 5.99f, "Item Desc") };
            mockRepository.Setup(r => r.GetListedItemsByGameAsync(game)).ReturnsAsync(listings);

            var result = await service.GetListingsByGameAsync(game);
            Assert.AreEqual(listings, result);
        }

        [Test]
        public void AddListingAsync_NullGame_ThrowsArgumentNullException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            Assert.ThrowsAsync<ArgumentNullException>(() => service.AddListingAsync(null, new Item("New Item", game, 5.99f, "Item Desc")));
        }

        [Test]
        public void AddListingAsync_NullItem_ThrowsArgumentNullException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            Assert.ThrowsAsync<ArgumentNullException>(() => service.AddListingAsync(game, null));
        }

        [Test]
        public async Task AddListingAsync_ValidInputs_CallsRepository()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var item = new Item("New Item", game, 5.99f, "Item Desc") { IsListed = false };
            mockRepository.Setup(r => r.MakeItemListableAsync(game, item)).Returns(Task.CompletedTask);

            await service.AddListingAsync(game, item);
            mockRepository.Verify(r => r.MakeItemListableAsync(game, item), Times.Once);
        }

        [Test]
        public void RemoveListingAsync_NullGame_ThrowsArgumentNullException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            var item = new Item("New Item", game, 5.99f, "Item Desc");
            Assert.ThrowsAsync<ArgumentNullException>(() => service.RemoveListingAsync(null, item));
        }

        [Test]
        public void RemoveListingAsync_NullItem_ThrowsArgumentNullException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            Assert.ThrowsAsync<ArgumentNullException>(() => service.RemoveListingAsync(game, null));
        }

        [Test]
        public async Task RemoveListingAsync_Valid_CallsRepository()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var item = new Item("New Item", game, 5.99f, "Item Desc");
            mockRepository.Setup(r => r.MakeItemListableAsync(game, item)).Returns(Task.CompletedTask);

            await service.RemoveListingAsync(game, item);
            mockRepository.Verify(r => r.MakeItemListableAsync(game, item), Times.Once);
        }

        [Test]
        public void UpdateListingAsync_NullGame_ThrowsArgumentNullException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListingAsync(null, new Item("New Item", game, 5.99f, "Item Desc")));
        }

        [Test]
        public void UpdateListingAsync_NullItem_ThrowsArgumentNullException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListingAsync(game, null));
        }

        [Test]
        public async Task UpdateListingAsync_Valid_CallsRepository()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var item = new Item("New Item", game, 5.99f, "Item Desc");
            mockRepository.Setup(r => r.UpdateItemPriceAsync(game, item)).Returns(Task.CompletedTask);

            await service.UpdateListingAsync(game, item);
            mockRepository.Verify(r => r.UpdateItemPriceAsync(game, item), Times.Once);
        }

        [Test]
        public void BuyItemAsync_NullItem_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => service.BuyItemAsync(null));
        }

        [Test]
        public void BuyItemAsync_UnlistedItem_ThrowsInvalidOperationException()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var item = new Item("New Item", game, 5.99f, "Item Desc") { IsListed = false };
            Assert.ThrowsAsync<InvalidOperationException>(() => service.BuyItemAsync(item));
        }

        [Test]
        public async Task BuyItemAsync_Valid_ReturnsTrue()
        {
            var game = new Game("Test Game", 9.99f, "RPG", "Game GameDescription");
            game.SetGameId(101);
            var item = new Item("New Item", game, 5.99f, "Item Desc") { IsListed = true };
            mockRepository.Setup(r => r.BuyItemAsync(item, testUser)).ReturnsAsync(true);

            var result = await service.BuyItemAsync(item);
            Assert.IsTrue(result);
        }
    }
}
