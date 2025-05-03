namespace SteamStore.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.Api.Context;
    using SteamHub.Api.Entities;
    using SteamHub.Api.Context.Repositories;
    using SteamHub.ApiContract.Models.Game;
    using Microsoft.EntityFrameworkCore;
    using Xunit;
    using Microsoft.Extensions.Configuration;

    public class GameRepositoryTest : IDisposable
    {
        private readonly DataContext _mockContext;
        private readonly GameRepository _repository;

        public GameRepositoryTest()
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

            _mockContext = new DataContext(options, configuration);
            _repository = new GameRepository(_mockContext);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Clear existing data
            _mockContext.RemoveRange(_mockContext.Games);
            _mockContext.RemoveRange(_mockContext.Users);
            _mockContext.RemoveRange(_mockContext.Tags);
            _mockContext.SaveChanges();

            // Create enum entities (won't be tracked by context)
            var approvedStatus = new GameStatus { Id = GameStatusEnum.Approved, Name = "Approved" };
            var userRole = new Role { Id = RoleEnum.User, Name = "User" };
            var developerRole = new Role { Id = RoleEnum.Developer, Name = "Developer" };

            // Create test user
            var user = new User
            {
                UserId = 1,
                UserName = "test_user",
                Email = "user@example.com",
                WalletBalance = 100.0f,
                PointsBalance = 500.0f,
                RoleId = RoleEnum.User,
                UserPointShopItemsInventory = new List<UserPointShopItemInventory>(),
                StoreTransactions = new List<StoreTransaction>()
            };

            var developer = new User
            {
                UserId = 2,
                UserName = "test_dev",
                Email = "dev@example.com",
                WalletBalance = 500.0f,
                PointsBalance = 1000.0f,
                RoleId = RoleEnum.Developer,
                UserPointShopItemsInventory = new List<UserPointShopItemInventory>(),
                StoreTransactions = new List<StoreTransaction>()
            };

            _mockContext.Users.AddRange(user, developer);

            // Create test tag
            var actionTag = new Tag{ TagId = 1, TagName = "Action", Games = new List<Game>()};
            var adventureTag = new Tag { TagId = 2, TagName = "Adventure", Games = new List<Game>() };
            var puzzleTag = new Tag { TagId = 3, TagName = "Puzzle", Games = new List<Game>() };


            _mockContext.Tags.Add(actionTag);

            // Create test game
            var game = new Game
            {
                GameId = 1,
                Name = "Test Game",
                Description = "Test Description",
                ImagePath = "/images/test.jpg",
                Price = 59.99m,
                MinimumRequirements = "Min Spec",
                RecommendedRequirements = "Recommended Spec",
                StatusId = GameStatusEnum.Approved,
                RejectMessage = null,
                Tags = new HashSet<Tag> { actionTag },
                Rating = 4.5m,
                NumberOfRecentPurchases = 100,
                TrailerPath = "/trailers/test.mp4",
                GameplayPath = "/gameplay/test.mp4",
                Discount = 10.0m,
                PublisherUserId = 2,
                Publisher = developer,
                StoreTransactions = new List<StoreTransaction>(),
                Items = new List<Item>()
            };
            actionTag.Games.Add(game);

            _mockContext.Games.Add(game);

            // Create store transaction
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
            user.StoreTransactions.Add(storeTransaction);
            game.StoreTransactions.Add(storeTransaction);

            _mockContext.StoreTransactions.Add(storeTransaction);
            _mockContext.SaveChanges();
        }

        public void Dispose()
        {
            _mockContext.Dispose();
        }

        [Fact]
        public async Task CreateGameAsync_WithValidRequest_CreatesGame()
        {
            var request = new CreateGameRequest
            {
                Name = "New Test Game",
                Description = "New Test Description",
                Price = 49.99m,
                PublisherUserIdentifier = 2,
                MinimumRequirements = "Min",
                RecommendedRequirements = "Rec",
                ImagePath = "path.jpg",
                TrailerPath = "trailer.mp4",
                GameplayPath = "gameplay.mp4",
                Discount = 0.1m,
                Rating = 4.5m,
                NumberOfRecentPurchases = 100
            };

            var result = await _repository.CreateGameAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            var gameInDb = await _mockContext.Games.FirstOrDefaultAsync(g => g.GameId == result.Identifier);
            Assert.NotNull(gameInDb);
            Assert.Equal(request.Description, gameInDb.Description);
        }

        [Fact]
        public async Task GetGameByIdAsync_WithExistingId_ReturnsGame()
        {
            // Arrange - Seed the test data
            SeedTestData(); 

            // Act
            var result = await _repository.GetGameByIdAsync(1);

            
            Assert.NotNull(result);
            Assert.Equal("Test Game", result.Name); 
            Assert.Equal(GameStatusEnum.Approved, result.Status); 
            Assert.Null(result.RejectMessage);  
            Assert.Equal(59.99m, result.Price); 
        }

        [Fact]
        public async Task GetGameByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetGameByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGamesAsync_WithNoFilters_ReturnsAllGames()
        {
            // Arrange
            var parameters = new GetGamesRequest();

            // Act
            var result = await _repository.GetGamesAsync(parameters);

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetGamesAsync_WithStatusFilter_ReturnsFilteredGames()
        {
            // Arrange
            var parameters = new GetGamesRequest { StatusIs = GameStatusEnum.Approved };

            // Act
            var result = await _repository.GetGamesAsync(parameters);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Game 2", result[0].Name);
        }

        [Fact]
        public async Task GetGamesAsync_WithPublisherFilter_ReturnsFilteredGames()
        {
            // Arrange
            var parameters = new GetGamesRequest { PublisherIdentifierIs = 2 };

            // Act
            var result = await _repository.GetGamesAsync(parameters);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, g => Assert.Equal(2, g.PublisherUserIdentifier));
        }

        [Fact]
        public async Task UpdateGameAsync_WithValidRequest_UpdatesGame()
        {
            // Arrange
            var request = new UpdateGameRequest
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 9.99m,
                Status = GameStatusEnum.Approved
            };

            // Act
            await _repository.UpdateGameAsync(1, request);

            // Assert
            var updatedGame = await _mockContext.Games.FindAsync(1);
            Assert.Equal("Updated Name", updatedGame.Name);
            Assert.Equal("Updated Description", updatedGame.Description);
            Assert.Equal(9.99m, updatedGame.Price);
            Assert.Equal(GameStatusEnum.Approved, updatedGame.StatusId);
        }

        [Fact]
        public async Task DeleteGameAsync_WithExistingId_RemovesGame()
        {
            // Act
            await _repository.DeleteGameAsync(1);

            // Assert
            var game = await _mockContext.Games.FindAsync(1);
            Assert.Null(game);
        }

        [Fact]
        public async Task PatchGameTagsAsync_WithInsertType_AddsTags()
        {
            // Arrange
            var request = new PatchGameTagsRequest
            {
                Type = GameTagsPatchType.Insert,
                TagIds = new HashSet<int> { 1, 2 }
            };

            // Act
            await _repository.PatchGameTagsAsync(1, request);

            // Assert
            var game = await _mockContext.Games
                .Include(g => g.Tags)
                .FirstOrDefaultAsync(g => g.GameId == 1);
            Assert.Equal(2, game.Tags.Count);
            Assert.Contains(game.Tags, t => t.TagId == 1);
            Assert.Contains(game.Tags, t => t.TagId == 2);
        }

        [Fact]
        public async Task PatchGameTagsAsync_WithDeleteType_RemovesTags()
        {
            // Arrange
            // First add some tags
            var game = await _mockContext.Games
                .Include(g => g.Tags)
                .FirstOrDefaultAsync(g => g.GameId == 1);
            var tags = await _mockContext.Tags.Where(t => t.TagId == 1 || t.TagId == 2).ToListAsync();
            foreach (var tag in tags)
            {
                game.Tags.Add(tag);
            }
            await _mockContext.SaveChangesAsync();

            var request = new PatchGameTagsRequest
            {
                Type = GameTagsPatchType.Delete,
                TagIds = new HashSet<int> { 1 }
            };

            // Act
            await _repository.PatchGameTagsAsync(1, request);

            // Assert
            game = await _mockContext.Games
                .Include(g => g.Tags)
                .FirstOrDefaultAsync(g => g.GameId == 1);
            Assert.Single(game.Tags);
            Assert.DoesNotContain(game.Tags, t => t.TagId == 1);
        }

        [Fact]
        public async Task PatchGameTagsAsync_WithReplaceType_ReplacesTags()
        {
            // Arrange
            // First add some tags
            var game = await _mockContext.Games
                .Include(g => g.Tags)
                .FirstOrDefaultAsync(g => g.GameId == 1);
            var tags = await _mockContext.Tags.Where(t => t.TagId == 1 || t.TagId == 2).ToListAsync();
            foreach (var tag in tags)
            {
                game.Tags.Add(tag);
            }
            await _mockContext.SaveChangesAsync();

            var request = new PatchGameTagsRequest
            {
                Type = GameTagsPatchType.Replace,
                TagIds = new HashSet<int> { 3 }
            };

            // Act
            await _repository.PatchGameTagsAsync(1, request);

            // Assert
            game = await _mockContext.Games
                .Include(g => g.Tags)
                .FirstOrDefaultAsync(g => g.GameId == 1);
            Assert.Single(game.Tags);
            Assert.Contains(game.Tags, t => t.TagId == 3);
            Assert.DoesNotContain(game.Tags, t => t.TagId == 1);
            Assert.DoesNotContain(game.Tags, t => t.TagId == 2);
        }
    }
}