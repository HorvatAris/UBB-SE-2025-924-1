using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Steampunks.Domain.Entities;
using Steampunks.Repository.Inventory;
using Steampunks.Services.InventoryService.InventoryService;

namespace Steampunks.Tests.InventoryTests
{
    [TestFixture]
    public class InventoryServiceTests
    {
        // 1. GetItemsFromInventoryAsync tests

        [Test]
        public void GetItemsFromInventoryAsync_NullGame_ThrowsArgumentNullException()
        {
            // Arrange
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            // Act & Assert: One assert checking for exception.
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await service.GetItemsFromInventoryAsync(null));
        }

        [Test]
        public async Task GetItemsFromInventoryAsync_ValidGame_ReturnsExpectedItemCount()
        {
            // Arrange
            var game = new Game("Test Game", 9.99f, "Adventure", "Test Desc");
            game.SetGameId(1);
            var expectedItems = new List<Item>
            {
                new Item("Sword", game, 20f, "Sharp sword")
            };
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.GetItemsFromInventoryAsync(game)).ReturnsAsync(expectedItems);
            var service = new InventoryService(mockRepo.Object);

            // Act
            var items = await service.GetItemsFromInventoryAsync(game);

            // Assert: One assert checking the item count.
            Assert.AreEqual(1, items.Count);
        }

        // 2. GetAllItemsFromInventoryAsync tests

        [Test]
        public async Task GetAllItemsFromInventoryAsync_ValidUser_ReturnsExpectedItemCount()
        {
            // Arrange
            var user = new User("TestUser");
            user.SetUserId(1);
            var expectedItems = new List<Item>
            {
                new Item("Shield", new Game("Game", 9.99f, "RPG", "Desc"), 15f, "Sturdy shield")
            };
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.GetAllItemsFromInventoryAsync(user)).ReturnsAsync(expectedItems);
            var service = new InventoryService(mockRepo.Object);

            // Act
            var items = await service.GetAllItemsFromInventoryAsync(user);

            // Assert: One assert checking the item count.
            Assert.AreEqual(1, items.Count);
        }

        // 3. AddItemToInventoryAsync tests

        [Test]
        public async Task AddItemToInventoryAsync_ValidParameters_CallsRepositoryOnce()
        {
            // Arrange
            var game = new Game("Test Game", 9.99f, "RPG", "Desc");
            game.SetGameId(1);
            var item = new Item("Item", game, 5f, "Desc");
            item.SetItemId(1);
            var user = new User("TestUser");
            user.SetUserId(1);
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.AddItemToInventoryAsync(game, item, user))
                    .Returns(Task.CompletedTask);
            var service = new InventoryService(mockRepo.Object);

            // Act
            await service.AddItemToInventoryAsync(game, item, user);

            // Assert: One assert verifying the repository call.
            mockRepo.Verify(r => r.AddItemToInventoryAsync(game, item, user), Times.Once);
        }

        // 4. RemoveItemFromInventoryAsync tests

        [Test]
        public async Task RemoveItemFromInventoryAsync_ValidParameters_CallsRepositoryOnce()
        {
            // Arrange
            var game = new Game("Test Game", 9.99f, "RPG", "Desc");
            game.SetGameId(1);
            var item = new Item("Item", game, 5f, "Desc");
            item.SetItemId(1);
            var user = new User("TestUser");
            user.SetUserId(1);
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.RemoveItemFromInventoryAsync(game, item, user))
                    .Returns(Task.CompletedTask);
            var service = new InventoryService(mockRepo.Object);

            // Act
            await service.RemoveItemFromInventoryAsync(game, item, user);

            // Assert: One assert verifying the repository call.
            mockRepo.Verify(r => r.RemoveItemFromInventoryAsync(game, item, user), Times.Once);
        }

        // 5. GetUserInventoryAsync tests

        [Test]
        public void GetUserInventoryAsync_InvalidUserId_ThrowsArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            // Act & Assert: One assert checking for exception.
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.GetUserInventoryAsync(0));
        }

        // 6. GetAllUsersAsync tests

        [Test]
        public async Task GetAllUsersAsync_ReturnsExpectedUserCount()
        {
            // Arrange
            User expectedUser = new User("User1");
            expectedUser.SetUserId(1);
            var expectedUsers = new List<User> { expectedUser };
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(expectedUsers);
            var service = new InventoryService(mockRepo.Object);

            // Act
            var users = await service.GetAllUsersAsync();

            // Assert: One assert checking the user count.
            Assert.AreEqual(1, users.Count);
        }

        // 7. SellItemAsync tests

        [Test]
        public async Task SellItemAsync_ValidItem_ReturnsTrue()
        {
            // Arrange
            var game = new Game("Test Game", 9.99f, "Adventure", "Test Desc");
            game.SetGameId(1);
            var item = new Item("Sword", game, 20f, "Sharp sword");
            item.SetItemId(1);
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.SellItemAsync(item)).ReturnsAsync(true);
            var service = new InventoryService(mockRepo.Object);

            // Act
            var result = await service.SellItemAsync(item);

            // Assert: One assert checking that the operation succeeded.
            Assert.IsTrue(result);
        }

        // 8. FilterInventoryItems tests

        [Test]
        public void FilterInventoryItems_NullItems_ThrowsArgumentNullException()
        {
            // Arrange
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);
            var selectedGame = new Game("Test Game", 9.99f, "Adventure", "Test Desc");
            selectedGame.SetGameId(1);

            // Act & Assert: One assert checking for exception.
            Assert.Throws<ArgumentNullException>(() =>
                service.FilterInventoryItems(null, selectedGame, "sword"));
        }

        [Test]
        public void FilterInventoryItems_ValidItems_ReturnsExpectedFilteredCount()
        {
            // Arrange
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);
            var game = new Game("Test Game", 9.99f, "Adventure", "Test Desc");
            game.SetGameId(1);
            var otherGame = new Game("Other Game", 9.99f, "Adventure", "Test Desc");
            otherGame.SetGameId(2);

            var items = new List<Item>
            {
                new Item("Sword", game, 20f, "A sharp sword") { IsListed = false },
                new Item("Shield", game, 15f, "A sturdy shield") { IsListed = false },
                new Item("Potion", otherGame, 5f, "Healing potion") { IsListed = true }
            };

            // Act
            var filtered = service.FilterInventoryItems(items, game, "sword");

            // Assert: One assert checking that only one item remains after filtering.
            Assert.AreEqual(1, filtered.Count);
        }

        // 9. GetAvailableGames tests

        [Test]
        public void GetAvailableGames_ValidItems_ReturnsAllGamesOptionFirst()
        {
            // Arrange
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);
            var game1 = new Game("Game1", 9.99f, "Action", "Desc");
            game1.SetGameId(1);
            var game2 = new Game("Game2", 19.99f, "Adventure", "Desc");
            game2.SetGameId(2);

            var items = new List<Item>
            {
                new Item("Sword", game1, 20f, "A sharp sword") { IsListed = false },
                new Item("Shield", game2, 15f, "A sturdy shield") { IsListed = false }
            };

            // Act
            var availableGames = service.GetAvailableGames(items);

            // Assert: One assert checking that the first game's title is the special "All Games" option.
            Assert.AreEqual("All Games", availableGames[0].Title);
        }

        // 10. GetUserFilteredInventoryAsync tests

        [Test]
        public async Task GetUserFilteredInventoryAsync_ValidParameters_ReturnsExpectedFilteredCount()
        {
            // Arrange
            var game = new Game("Test Game", 9.99f, "Adventure", "Test Desc");
            game.SetGameId(1);
            var item1 = new Item("Sword", game, 20f, "A sharp sword") { IsListed = false };
            var item2 = new Item("Shield", game, 15f, "Sturdy shield") { IsListed = false };
            var expectedItems = new List<Item> { item1, item2 };
            var user = new User("TestUser");
            user.SetUserId(1);

            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.GetUserInventoryAsync(user.UserId)).ReturnsAsync(expectedItems);
            var service = new InventoryService(mockRepo.Object);

            // Act
            var filtered = await service.GetUserFilteredInventoryAsync(user.UserId, game, "sword");

            // Assert: One assert checking that only one item matches the search text.
            Assert.AreEqual(1, filtered.Count);
        }
    }
}

