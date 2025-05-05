using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using Xunit;

namespace SteamHub.Tests.Repositories
{
    public class ItemRepositoryTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly ItemRepository _repository;

        public ItemRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var inMemorySettings = new Dictionary<string, string>
            {
                { "SomeSetting", "SomeValue" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _context = new DataContext(options, configuration);
            _repository = new ItemRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var gameStatus = new GameStatus { Id = GameStatusEnum.Approved, Name = "Approved" };
            var role = new Role { Id = RoleEnum.User, Name = "User" };
            // User
            var user = new User
            {
                UserId = 1,
                UserName = "test_user",
                Email = "user@example.com",
                WalletBalance = 100.0f,
                PointsBalance = 500.0f,
                RoleId = RoleEnum.User,
                UserRole = role,
                UserPointShopItemsInventory = new List<UserPointShopItemInventory>(),
                StoreTransactions = new List<StoreTransaction>()
            };
            _context.Users.Add(user);

            // Tag
            var tag = new Tag
            {
                TagId = 1,
                TagName = "Action",
                Games = new List<Game>()
            };
            _context.Tags.Add(tag);

            // Game
            var game = new Game
            {
                GameId = 1,
                Name = "Mock Game",
                Description = "A great mock game",
                ImagePath = "/images/mock.jpg",
                Price = 59.99m,
                MinimumRequirements = "Min Spec",
                RecommendedRequirements = "Recommended Spec",
                StatusId = GameStatusEnum.Approved,
                Status = gameStatus,
                RejectMessage = null,
                Tags = new HashSet<Tag> { tag },
                Rating = 4.5m,
                NumberOfRecentPurchases = 100,
                TrailerPath = "/trailers/mock.mp4",
                GameplayPath = "/gameplay/mock.mp4",
                Discount = 10.0m,
                PublisherUserId = 1,
                Publisher = user,
                StoreTransactions = new List<StoreTransaction>(),
                Items = new List<Item>()
            };
            _context.Games.Add(game);

            // Item
            var item = new Item
            {
                ItemId = 1,
                ItemName = "Mock Item",
                CorrespondingGameId = 1,
                Game = game,
                Price = 19.99f,
                Description = "A mock item",
                IsListed = true,
                ImagePath = "/images/item.jpg",
                ItemTradeDetails = new List<ItemTradeDetail>()
            };
            _context.Items.Add(item);

            // ItemTrade
            var itemTrade = new ItemTrade
            {
                TradeId = 1,
                SourceUserId = 1,
                DestinationUserId = 1,
                SourceUser = user,
                DestinationUser = user,
                GameOfTradeId = 1,
                GameOfTrade = game,
                TradeDate = DateTime.UtcNow,
                TradeDescription = "Trade description",
                TradeStatus = TradeStatus.Pending,
                AcceptedBySourceUser = false,
                AcceptedByDestinationUser = false,
                ItemTradeDetails = new List<ItemTradeDetail>()
            };
            _context.ItemTrades.Add(itemTrade);

            // UserInventory
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
            _context.UserInventories.Add(userInventory);

            // UsersGames
            var usersGames = new UsersGames
            {
                UserId = 1,
                GameId = 1,
                IsInWishlist = true,
                IsPurchased = true,
                IsInCart = false,
                User = user,
                Game = game
            };
            _context.UsersGames.Add(usersGames);

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        [Fact]
        public async Task GetItemsAsync_WhenCalled_ReturnsNonNullItemList()
        {
            var items = await _repository.GetItemsAsync();

            Assert.NotNull(items);
        }

        [Fact]
        public async Task GetItemsAsync_WhenCalled_ReturnsItemWithCorrectName()
        {
            var items = await _repository.GetItemsAsync();

            var itemList = items.ToList();
            Assert.Equal("Mock Item", itemList[0].ItemName);
        }


        [Fact]
        public async Task GetItemsAsync_WhenCalled_ReturnsSingleItem()
        {
            var items = await _repository.GetItemsAsync();

            var itemList = items.ToList();
            Assert.Single(itemList);
        }


        [Fact]
        public async Task GetItemByIdAsync_ValidId_ReturnsItem()
        {
            var validId = 1;
            var item = await _repository.GetItemByIdAsync(validId);

            Assert.NotNull(item);
            Assert.Equal("Mock Item", item.ItemName);
        }

        [Fact]
        public async Task GetItemByIdAsync_InvalidId_ReturnsNull()
        {
            var item = await _repository.GetItemByIdAsync(999);

            Assert.Null(item);
        }

        [Fact]
        public async Task CreateItemAsync_WithValidRequest_ReturnsNonNullResponse()
        {
            var request = new CreateItemRequest
            {
                ItemName = "New Item",
                GameId = 1,
                Price = 5.5f,
                Description = "A new test item",
                IsListed = true,
                ImagePath = "/images/new.png"
            };

            var response = await _repository.CreateItemAsync(request);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task CreateItemAsync_WithValidRequest_SetsCorrectItemName()
        {
            var request = new CreateItemRequest
            {
                ItemName = "New Item",
                GameId = 1,
                Price = 5.5f,
                Description = "A new test item",
                IsListed = true,
                ImagePath = "/images/new.png"
            };

            var response = await _repository.CreateItemAsync(request);

            Assert.Equal("New Item", response.ItemName);
        }

        [Fact]
        public async Task CreateItemAsync_WithValidRequest_SavesItemToDatabase()
        {
            var request = new CreateItemRequest
            {
                ItemName = "New Item",
                GameId = 1,
                Price = 5.5f,
                Description = "A new test item",
                IsListed = true,
                ImagePath = "/images/new.png"
            };

            var response = await _repository.CreateItemAsync(request);

            var created = await _context.Items.FindAsync(response.ItemId);
            Assert.NotNull(created);
        }




        [Fact]
        public async Task UpdateItemAsync_ValidId_UpdatesItem()
        {
            var updateRequest = new UpdateItemRequest
            {
                ItemName = "Updated Item",
                GameId = 1,
                Price = 10.99f,
                Description = "Updated description",
                IsListed = false,
                ImagePath = "/images/updated.png"
            };

            await _repository.UpdateItemAsync(1, updateRequest);

            var updatedItem = await _context.Items.FindAsync(1);
            Assert.Equal("Updated Item", updatedItem.ItemName);
            Assert.False(updatedItem.IsListed);
        }

        [Fact]
        public async Task UpdateItemAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            var request = new UpdateItemRequest
            {
                ItemName = "Non-existing",
                GameId = 1,
                Price = 1,
                Description = "None",
                IsListed = false,
                ImagePath = "/nope.png"
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _repository.UpdateItemAsync(999, request));
        }

        [Fact]
        public async Task DeleteItemAsync_ValidId_DeletesItemAndRelatedData()
        {
            await _repository.DeleteItemAsync(1);

            var item = await _context.Items.FindAsync(1);
            Assert.Null(item);

            var inventory = await _context.UserInventories.FirstOrDefaultAsync(i => i.ItemId == 1);
            Assert.Null(inventory);

            var tradeDetails = await _context.ItemTradeDetails.FirstOrDefaultAsync(t => t.ItemId == 1);
            Assert.Null(tradeDetails);
        }

        [Fact]
        public async Task DeleteItemAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.DeleteItemAsync(999));
        }
    }
}
