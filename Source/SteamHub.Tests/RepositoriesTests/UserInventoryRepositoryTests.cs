using GameEntity = SteamHub.Api.Entities.Game;

using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.UserInventory;
using SteamHub.ApiContract.Repositories;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Context;

namespace SteamHub.Tests.RepositoriesTests
{
    public class UserInventoryRepositoryTests
    {
        private readonly DataContext _context;
        private readonly UserInventoryRepository _repository;

        public UserInventoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options, null);
            _repository = new UserInventoryRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var user = new User
            {
                UserId = 1,
                UserName = "test_user",
                Email = "user@example.com",
                WalletBalance = 100.0f,
                PointsBalance = 500.0f,
                RoleId = RoleEnum.User
            };

            var game = new GameEntity
            {
                GameId = 1,
                Name = "Mock Game",
                Description = "A great mock game",
                Price = 59.99m
            };

            var item = new Item
            {
                ItemId = 1,
                ItemName = "Mock Item",
                CorrespondingGameId = 1,
                Game = game,
                Price = 19.99f,
                IsListed = true,
                Description = "A mock item for testing",
                ImagePath = "/images/mockitem.jpg",
                ItemTradeDetails = new List<ItemTradeDetail>()
            };

            var userInventory = new UserInventory
            {
                UserId = 1,
                ItemId = 1,
                GameId = 1,
                AcquiredDate = DateTime.Now,
                IsActive = true,
                User = user,
                Item = item,
                Game = game
            };

            _context.Users.Add(user);
            _context.Games.Add(game);
            _context.Items.Add(item);
            _context.UserInventories.Add(userInventory);
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddItemToUserInventoryAsync_WithValidData_AddsItemToInventory()
        {
            // Arrange
            var request = new ItemFromInventoryRequest
            {
                UserId = 1,
                ItemId = 2,  // This item should be added to the inventory
                GameId = 1
            };

            var item = new Item
            {
                ItemId = 2,
                ItemName = "New Mock Item",
                CorrespondingGameId = 1,
                Game = await _context.Games.FindAsync(1),
                Price = 29.99f,
                IsListed = true,
                Description = "A new mock item for testing",
                ImagePath = "/images/newmockitem.jpg",
                ItemTradeDetails = new List<ItemTradeDetail>()
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AddItemToUserInventoryAsync(request);

            // Assert
            var userInventory = await _context.UserInventories
                .FirstOrDefaultAsync(userInventoryElement => userInventoryElement.UserId == 1 && userInventoryElement.ItemId == 2);
            Assert.NotNull(userInventory);
            Assert.True(userInventory.IsActive);
        }

        [Fact]
        public async Task AddItemToUserInventoryAsync_WithInvalidUser_ThrowsArgumentException()
        {
            // Arrange
            var request = new ItemFromInventoryRequest
            {
                UserId = 999,  // Non-existent user
                ItemId = 1,
                GameId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.AddItemToUserInventoryAsync(request));
        }

        [Fact]
        public async Task AddItemToUserInventoryAsync_WithInvalidItem_ThrowsArgumentException()
        {
            // Arrange
            var request = new ItemFromInventoryRequest
            {
                UserId = 1,
                ItemId = 999,  // Non-existent item
                GameId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.AddItemToUserInventoryAsync(request));
        }

        [Fact]
        public async Task AddItemToUserInventoryAsync_WithInvalidGame_ThrowsArgumentException()
        {
            // Arrange
            var request = new ItemFromInventoryRequest
            {
                UserId = 1,
                ItemId = 1,
                GameId = 999  // Non-existent game
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.AddItemToUserInventoryAsync(request));
        }

        [Fact]
        public async Task GetItemFromUserInventoryAsync_WithValidData_ReturnsCorrectItem()
        {
            // Act
            var response = await _repository.GetItemFromUserInventoryAsync(1, 1);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(1, response.ItemId);
            Assert.Equal("Mock Item", response.ItemName);
            Assert.True(response.IsListed);
        }

        [Fact]
        public async Task GetItemFromUserInventoryAsync_WithInvalidUser_ReturnsNull()
        {
            // Act
            var response = await _repository.GetItemFromUserInventoryAsync(999, 1);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetItemFromUserInventoryAsync_WithInvalidItem_ReturnsNull()
        {
            // Act
            var response = await _repository.GetItemFromUserInventoryAsync(1, 999);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetUserInventoryAsync_WithValidUser_ReturnsCorrectInventory()
        {
            // Act
            var response = await _repository.GetUserInventoryAsync(1);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Items);
        }

        [Fact]
        public async Task GetUserInventoryAsync_WithValidUser_ReturnsItemWithCorrectId()
        {
            var response = await _repository.GetUserInventoryAsync(1);
            var item = response.Items.First();

            Assert.Equal(1, item.ItemId);
        }


        [Fact]
        public async Task RemoveItemFromUserInventoryAsync_WithValidData_RemovesItemFromInventory()
        {
            // Arrange
            var request = new ItemFromInventoryRequest
            {
                UserId = 1,
                ItemId = 1,
                GameId = 1
            };

            // Act
            await _repository.RemoveItemFromUserInventoryAsync(request);

            // Assert
            var userInventory = await _context.UserInventories
                .FirstOrDefaultAsync(userInventoryItem => userInventoryItem.UserId == 1 && userInventoryItem.ItemId == 1 && userInventoryItem.GameId == 1);
            Assert.Null(userInventory);
        }

        [Fact]
        public async Task RemoveItemFromUserInventoryAsync_WithInvalidData_ThrowsArgumentException()
        {
            // Arrange
            var request = new ItemFromInventoryRequest
            {
                UserId = 1,
                ItemId = 999,  // Non-existent item
                GameId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.RemoveItemFromUserInventoryAsync(request));
        }
    }
}
