using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.PointShopItem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Xunit;
using SteamHub.ApiContract.Models.Game;

namespace SteamHub.Tests.RepositoriesTests
{
    public class PointShopItemRepositoryTests
    {
        private readonly DataContext _context;
        private readonly PointShopItemRepository _repository;

        public PointShopItemRepositoryTests()
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
            _repository = new PointShopItemRepository(_context);

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

            // ItemTradeDetail
            var itemTradeDetail = new ItemTradeDetail
            {
                TradeId = 1,
                ItemId = 1,
                IsSourceUserItem = true,
                ItemTrade = itemTrade,
                Item = item
            };
            _context.ItemTradeDetails.Add(itemTradeDetail);
            item.ItemTradeDetails.Add(itemTradeDetail);

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


            _context.PointShopItems.AddRange(
                new PointShopItem { PointShopItemId = 1, Name = "CSGO", Description = "ex", ImagePath = "csgo.png", PointPrice = 100, ItemType = "ex" },
                new PointShopItem { PointShopItemId = 2, Name = "Sword", Description = "Sharp sword", ImagePath = "sword.png", PointPrice = 200, ItemType = "Weapon" }
            );
            // StoreTransaction
            var storeTransaction = new StoreTransaction
            {
                StoreTransactionId = 1,
                UserId = 1,
                GameId = 1,
                Date = DateTime.Now,
                Amount = 59.99f,
                WithMoney = true,
                User = user,
                Game = game
            };
            _context.StoreTransactions.Add(storeTransaction);


            _context.SaveChanges();

        }

        [Fact]
        public async Task GetPointShopItemsAsync_WhenGenerallyCalled_ReturnsAllItems()
        {
            var result = await _repository.GetPointShopItemsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.PointShopItems.Count);
        }

        [Fact]
        public async Task GetPointShopItemByIdAsync_WhenCalledWithExistingId_ReturnsNonNullItem()
        {
            var result = await _repository.GetPointShopItemByIdAsync(1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPointShopItemByIdAsync_WhenCalledWithExistingId_ReturnsItemWithCorrectName()
        {
            var result = await _repository.GetPointShopItemByIdAsync(1);

            Assert.Equal("CSGO", result.Name);
        }



        [Fact]
        public async Task GetPointShopItemByIdAsync_WhenCalledWithInvalidId_ReturnsNull()
        {
            var result = await _repository.GetPointShopItemByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreatePointShopItemAsync_WhenCalledWithValidRequest_CreatesItem()
        {
            var request = new CreatePointShopItemRequest
            {
                Name = "ex1",
                Description = "ex1",
                ImagePath = "ex1.png",
                PointPrice = 300,
                ItemType = "ex1"
            };

            var response = await _repository.CreatePointShopItemAsync(request);
            var created = await _context.PointShopItems.FindAsync(response.PointShopItemId);

            Assert.NotNull(created);
            Assert.Equal("ex1", created.Name);
        }

        [Fact]
        public async Task UpdatePointShopItemAsync_WhenCalledWithValidRequest_UpdatesItem()
        {
            var request = new UpdatePointShopItemRequest
            {
                Name = "ex2",
                Description = "ex",
                ImagePath = "ex2.png",
                PointPrice = 120,
                ItemType = "ex"
            };

            var idOfItemToBeUpdated = 1;

            await _repository.UpdatePointShopItemAsync(idOfItemToBeUpdated, request);

            var updated = await _context.PointShopItems.FindAsync(1);
            Assert.Equal("ex2", updated.Name);
        }

        [Fact]
        public async Task UpdatePointShopItemAsync_WhenCalledWithInvalidId_ThrowsException()
        {
            var invalidId = 999;
            var request = new UpdatePointShopItemRequest
            {
                Name = "Ghost",
                Description = "Invisible",
                ImagePath = "ghost.png",
                PointPrice = 50,
                ItemType = "Effect"
            };

            await Assert.ThrowsAsync<Exception>(() => _repository.UpdatePointShopItemAsync(invalidId, request));
        }

        [Fact]
        public async Task DeletePointShopItemAsync_WhenCalledWithValidId_DeletesItem()
        {
            var invalidIdValue = 2;
            await _repository.DeletePointShopItemAsync(invalidIdValue);

            var deleted = await _context.PointShopItems.FindAsync(2);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeletePointShopItemAsync_WhenCalledWithInvalidId_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _repository.DeletePointShopItemAsync(999));
        }
    }
}
