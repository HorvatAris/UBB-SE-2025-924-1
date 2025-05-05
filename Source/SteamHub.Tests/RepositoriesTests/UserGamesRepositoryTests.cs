using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamHub.Api.Context.Repositories;
using SteamHub.ApiContract.Models.UsersGames;
using Xunit;
using SteamHub.ApiContract.Models.Game;

namespace SteamHub.Tests.RepositoriesTests
{
    public class UsersGamesRepositoryTests : IDisposable
    {
        private readonly DataContext _mockContext;
        private readonly UsersGamesRepository _repository;

        public UsersGamesRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockContext = new DataContext(options, null);
            _repository = new UsersGamesRepository(_mockContext);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Users
            var users = new List<User>
    {
        new User
        {
            UserId = 1,
            UserName = "Alice",
            Email = "alice@example.com",
            RoleId = RoleEnum.User,
            WalletBalance = (float)100.0m,
            PointsBalance = 500
        },
        new User
        {
            UserId = 2,
            UserName = "Bob",
            Email = "bob@example.com",
            RoleId = RoleEnum.Developer,
            WalletBalance = (float) 200.0m,
            PointsBalance = 1000
        }
    };
            _mockContext.Users.AddRange(users);

            // Tag
            var tag = new Tag
            {
                TagId = 1,
                TagName = "Action",
                Games = new List<Game>()
            };
            _mockContext.Tags.Add(tag);

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
                Tags = new HashSet<Tag> { tag },
                Rating = 4.5m,
                NumberOfRecentPurchases = 100,
                TrailerPath = "/trailers/mock.mp4",
                GameplayPath = "/gameplay/mock.mp4",
                Discount = 10.0m,
                PublisherUserId = 1,
                Publisher = users[0],
                StoreTransactions = new List<StoreTransaction>(),
                Items = new List<Item>()
            };
            _mockContext.Games.Add(game);

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
                ImagePath = "/images/item.jpg"
            };
            _mockContext.Items.Add(item);

            // ItemTrade
            var itemTrade = new ItemTrade
            {
                TradeId = 1,
                SourceUserId = 1,
                DestinationUserId = 1,
                SourceUser = users[0],
                DestinationUser = users[0],
                GameOfTradeId = 1,
                GameOfTrade = game,
                TradeDate = DateTime.UtcNow,
                TradeDescription = "Trade description",
                TradeStatus = TradeStatus.Pending,
                AcceptedBySourceUser = false,
                AcceptedByDestinationUser = false
            };
            _mockContext.ItemTrades.Add(itemTrade);

            // UserInventory
            var userInventory = new UserInventory
            {
                UserId = 1,
                ItemId = 1,
                GameId = 1,
                AcquiredDate = DateTime.Now,
                IsActive = true,
                User = users[0],
                Item = item,
                Game = game
            };
            _mockContext.UserInventories.Add(userInventory);

            // UsersGames
            var usersGames = new UsersGames
            {
                UserId = 1,
                GameId = 1,
                IsInWishlist = true,
                IsPurchased = false,
                IsInCart = false,
                User = users[0],
                Game = game
            };
            _mockContext.UsersGames.Add(usersGames);

            _mockContext.SaveChanges();
        }


        public void Dispose()
        {
            _mockContext.Dispose();
        }

        [Fact]
        public async Task AddToCartAsync_WithNewGame_AddsToCart()
        {
            var request = new UserGameRequest { UserId = 1, GameId = 1 };

            await _repository.AddToCartAsync(request);

            var entry = await _mockContext.UsersGames
                .FirstOrDefaultAsync(x => x.UserId == 1 && x.GameId == 1);

            Assert.NotNull(entry);
            Assert.True(entry.IsInCart);
        }

        [Fact]
        public async Task AddToWishlistAsync_WithNewGame_AddsToWishlist()
        {
            var request = new UserGameRequest { UserId = 1, GameId = 1 };

            await _repository.AddToWishlistAsync(request);

            var entry = await _mockContext.UsersGames
                .FirstOrDefaultAsync(userGame => userGame.UserId == 1 && userGame.GameId == 1);

            Assert.NotNull(entry);
            Assert.True(entry.IsInWishlist);
        }

        [Fact]
        public async Task PurchaseGameAsync_WithValidRequest_MarksGameAsPurchased()
        {
            var request = new UserGameRequest { UserId = 1, GameId = 1 };

            await _repository.PurchaseGameAsync(request);

            var entry = await _mockContext.UsersGames
                .FirstOrDefaultAsync(userGame => userGame.UserId == 1 && userGame.GameId == 1);

            Assert.NotNull(entry);
            Assert.True(entry.IsPurchased);
        }

        [Fact]
        public async Task PurchaseGameAsync_WithValidRequest_RemovesGameFromCart()
        {
            var request = new UserGameRequest { UserId = 1, GameId = 1 };

            await _repository.PurchaseGameAsync(request);

            var entry = await _mockContext.UsersGames
                .FirstOrDefaultAsync(userGame => userGame.UserId == 1 && userGame.GameId == 1);

            Assert.NotNull(entry);
            Assert.False(entry.IsInCart);
        }

        [Fact]
        public async Task PurchaseGameAsync_WithValidRequest_RemovesGameFromWishlist()
        {
            var request = new UserGameRequest { UserId = 1, GameId = 1 };

            await _repository.PurchaseGameAsync(request);

            var entry = await _mockContext.UsersGames
                .FirstOrDefaultAsync(userGame => userGame.UserId == 1 && userGame.GameId == 1);

            Assert.NotNull(entry);
            Assert.False(entry.IsInWishlist);
        }



        [Fact]
        public async Task RemoveFromCartAsync_WithValidRequest_ItemIsInitiallyInCart()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInCart = true;
            await _mockContext.SaveChangesAsync();

            var updated = await _mockContext.UsersGames.FirstAsync();

            Assert.True(updated.IsInCart);  // Assert that the item is initially in the cart
        }

        [Fact]
        public async Task RemoveFromCartAsync_WithValidRequest_RemovesItemFromCart()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInCart = true;
            await _mockContext.SaveChangesAsync();

            var request = new UserGameRequest { UserId = 1, GameId = 1 };
            await _repository.RemoveFromCartAsync(request);

            var updated = await _mockContext.UsersGames.FirstAsync();

            Assert.False(updated.IsInCart);  // Assert that the item is removed from the cart
        }


        [Fact]
        public async Task RemoveFromWishlistAsync_WithValidRequest_ItemIsInitiallyInWishlist()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInWishlist = true;
            await _mockContext.SaveChangesAsync();

            var updated = await _mockContext.UsersGames.FirstAsync();

            Assert.True(updated.IsInWishlist);  // Assert that the item is initially in the wishlist
        }

        [Fact]
        public async Task RemoveFromWishlistAsync_WithValidRequest_RemovesItemFromWishlist()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInWishlist = true;
            await _mockContext.SaveChangesAsync();

            var request = new UserGameRequest { UserId = 1, GameId = 1 };
            await _repository.RemoveFromWishlistAsync(request);

            var updated = await _mockContext.UsersGames.FirstAsync();

            Assert.False(updated.IsInWishlist);  // Assert that the item is removed from the wishlist
        }


        [Fact]
        public async Task GetUserCartAsync_WithValidUser_ReturnsCorrectNumberOfGamesInCart()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInCart = true;
            await _mockContext.SaveChangesAsync();

            var result = await _repository.GetUserCartAsync(1);

            Assert.Single(result.UserGames);  // Assert that there is only one game in the cart
        }


        [Fact]
        public async Task GetUserCartAsync_WithValidUser_ReturnsGamesMarkedAsInCart()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInCart = true;
            await _mockContext.SaveChangesAsync();

            var result = await _repository.GetUserCartAsync(1);

            Assert.True(result.UserGames.First().IsInCart);  // Assert that the game in the cart is marked as "InCart"
        }

        [Fact]
        public async Task GetUserWishlistAsync_WithValidUser_ReturnsCorrectNumberOfGamesInWishlist()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInWishlist = true;
            await _mockContext.SaveChangesAsync();

            var result = await _repository.GetUserWishlistAsync(1);

            Assert.Single(result.UserGames);  // Assert that there is only one game in the wishlist
        }

        [Fact]
        public async Task GetUserWishlistAsync_WithValidUser_ReturnsGamesMarkedAsInWishlist()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsInWishlist = true;
            await _mockContext.SaveChangesAsync();

            var result = await _repository.GetUserWishlistAsync(1);

            Assert.True(result.UserGames.First().IsInWishlist);  // Assert that the game in the wishlist is marked as "InWishlist"
        }


        [Fact]
        public async Task GetUserPurchasedGamesAsync_WithValidUser_ReturnsCorrectNumberOfPurchasedGames()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsPurchased = true;
            await _mockContext.SaveChangesAsync();

            var result = await _repository.GetUserPurchasedGamesAsync(1);

            Assert.Single(result.UserGames);  // Assert that there is only one purchased game
        }

        [Fact]
        public async Task GetUserPurchasedGamesAsync_WithValidUser_ReturnsPurchasedGames()
        {
            var entry = await _mockContext.UsersGames.FirstAsync();
            entry.IsPurchased = true;
            await _mockContext.SaveChangesAsync();

            var result = await _repository.GetUserPurchasedGamesAsync(1);

            Assert.True(result.UserGames.First().IsPurchased);  // Assert that the returned game is marked as "Purchased"
        }

        [Fact]
        public async Task GetUserGamesAsync_WithValidUser_ReturnsAtLeastOneGame()
        {
            var result = await _repository.GetUserGamesAsync(1);

            Assert.Single(result.UserGames);  // Assert that there is at least one game returned
        }

        [Fact]
        public async Task GetUserGamesAsync_WithValidUser_ReturnsCorrectGameId()
        {
            var result = await _repository.GetUserGamesAsync(1);

            Assert.Equal(1, result.UserGames.First().GameId);  // Assert that the game ID is 1
        }



    }
}
