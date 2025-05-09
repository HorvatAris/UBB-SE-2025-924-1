namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Proxies;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Constants;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Services;
    using Xunit;

    public class UserGameServiceTests
    {
        private readonly Mock<UserGamesRepositoryProxy> userGameServiceProxyMock;
        private readonly Mock<GameRepositoryProxy> gameServiceProxyMock;
        private readonly Mock<TagRepositoryProxy> tagServiceProxyMock;

        private readonly User testUser;
        private readonly UserGameService userGameService;

        public UserGameServiceTests()
        {
            this.userGameServiceProxyMock = new Mock<UserGamesRepositoryProxy>();
            this.gameServiceProxyMock = new Mock<GameRepositoryProxy>();
            this.tagServiceProxyMock = new Mock<TagRepositoryProxy>();

            this.testUser = new User
            {
                UserId = 1,
                WalletBalance = 100,
                PointsBalance = 10
            };

            this.userGameService = new UserGameService(
                this.userGameServiceProxyMock.Object,
                this.gameServiceProxyMock.Object,
                this.tagServiceProxyMock.Object,
                this.testUser);
        }

        [Fact]
        public async Task RemoveGameFromWishlistAsync_ValidGameId_CallsProxyCorrectly()
        {
            var game = new Game { GameId = 1 };

            await this.userGameService.RemoveGameFromWishlistAsync(game);

            this.userGameServiceProxyMock.Verify(proxy => proxy.RemoveFromWishlistAsync(
                It.Is<UserGameRequest>(request => request.GameId == game.GameId && request.UserId == testUser.UserId)), Times.Once);
        }

        [Fact]
        public async Task AddGameToWishlistAsync_ValidId_CallsProxy()
        {
            var game = new Game { GameId = 2, GameTitle = "Not Owned Game" };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserPurchasedGamesAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            await this.userGameService.AddGameToWishlistAsync(game);

            this.userGameServiceProxyMock.Verify(proxy => proxy.AddToWishlistAsync(
                It.Is<UserGameRequest>(request => request.GameId == game.GameId && request.UserId == testUser.UserId)), Times.Once);
        }

        [Fact]
        public async Task GetAllGamesAsync_WhenGamesExist_ReturnsMappedGames()
        {
            var response = new GetUserGamesResponse
            {
                UserGames = new List<UserGamesResponse>
                {
                    new UserGamesResponse { GameId = 1 },
                    new UserGamesResponse { GameId = 2 }
                }
            };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserGamesAsync(testUser.UserId)).ReturnsAsync(response);

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse { Identifier = id, Name = $"Game{id}" });

            var result = await this.userGameService.GetAllGamesAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task PurchaseGamesAsync_ValidGames_UpdatesLastEarnedPoints()
        {
            var initialPoints = testUser.PointsBalance;
            var games = new List<Game> { new Game { GameId = 1 }, new Game { GameId = 2 } };

            await this.userGameService.PurchaseGamesAsync(games);

            this.userGameServiceProxyMock.Verify(proxy => proxy.PurchaseGameAsync(
                It.IsAny<UserGameRequest>()), Times.Exactly(games.Count));
            Assert.Equal(0, this.userGameService.LastEarnedPoints);
        }

        [Fact]
        public async Task SearchWishListByNameAsync_ValidExistingName_FindsMatchingGames()
        {
            var games = new Collection<Game>
            {
                new Game { GameId = 1, GameTitle = "Zelda" },
                new Game { GameId = 2, GameTitle = "Halo Infinite" },
                new Game { GameId = 3, GameTitle = "Cool Game" }
            };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse { Identifier = id, Name = game.GameTitle };
                });

            var result = await this.userGameService.SearchWishListByNameAsync("cool");

            Assert.Single(result);
            Assert.Equal("Cool Game", result[0].GameTitle);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs5AndFilterIsOverwhelminglyPositive_ReturnsGame()
        {
            var games = new Collection<Game>
            {
                new Game { GameId = 1, GameTitle = "Test Game", Rating = 5 }
            };

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.OVERWHELMINGLYPOSITIVE);

            Assert.Single(result);
            Assert.Equal("Test Game", result[0].GameTitle);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs4Point1AndFilterIsVeryPositive_ReturnsGame()
        {
            var games = new Collection<Game>
            {
                new Game { GameId = 2, GameTitle = "Test Game", Rating = 4.1m }
            };

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.VERYPOSITIVE);

            Assert.Single(result);
            Assert.Equal("Test Game", result[0].GameTitle);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs3AndFilterIsMixed_ReturnsGame()
        {
            var games = new Collection<Game>
            {
                new Game { GameId = 3, GameTitle = "Test Game", Rating = 3m }
            };

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.MIXED);

            Assert.Single(result);
            Assert.Equal("Test Game", result[0].GameTitle);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs1AndFilterIsNegative_ReturnsGame()
        {
            var games = new Collection<Game>
            {
                new Game { GameId = 4, GameTitle = "Test Game", Rating = 1m }
            };

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.NEGATIVE);

            Assert.Single(result);
            Assert.Equal("Test Game", result[0].GameTitle);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_UnknownCriteria_ReturnsAll()
        {
            var games = new Collection<Game>
            {
                new Game { GameId = 1, GameTitle = "Game A", Rating = 2.5m },
                new Game { GameId = 2, GameTitle = "Game B", Rating = 4.8m }
            };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse { Identifier = id, Name = game.GameTitle, Rating = (decimal)game.Rating };
                });

            var result = await this.userGameService.FilterWishListGamesAsync("UNKNOWN");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByPrice_AndAscendingOrder_ReturnsCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20, Rating = 3.5m, Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10, Rating = 4.5m, Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15, Rating = 2.5m, Discount = 20 }
    };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Price = game.Price,
                        Rating = (decimal)game.Rating,
                        Discount = game.Discount
                    };
                });

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.PRICE, true);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Halo", sorted.First().GameTitle);  // Expecting lowest price first
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByPrice_AndDescendingOrder_ReturnsCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20, Rating = 3.5m, Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10, Rating = 4.5m, Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15, Rating = 2.5m, Discount = 20 }
    };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Price = game.Price,
                        Rating = (decimal)game.Rating,
                        Discount = game.Discount
                    };
                });

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.PRICE, false);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Zelda", sorted.First().GameTitle);  // Expecting highest price first
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByRating_AndAscendingOrder_ReturnsCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20, Rating = 3.5m, Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10, Rating = 4.5m, Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15, Rating = 2.5m, Discount = 20 }
    };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Price = game.Price,
                        Rating = (decimal)game.Rating,
                        Discount = game.Discount
                    };
                });

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.RATING, true);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Among Us", sorted.First().GameTitle);  // Expecting lowest rating first
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByRating_AndDescendingOrder_ReturnsCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20, Rating = 3.5m, Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10, Rating = 4.5m, Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15, Rating = 2.5m, Discount = 20 }
    };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Price = game.Price,
                        Rating = (decimal)game.Rating,
                        Discount = game.Discount
                    };
                });

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.RATING, false);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Halo", sorted.First().GameTitle);  // Expecting highest rating first
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByDiscount_AndAscendingOrder_ReturnsCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20, Rating = 3.5m, Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10, Rating = 4.5m, Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15, Rating = 2.5m, Discount = 20 }
    };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Price = game.Price,
                        Rating = (decimal)game.Rating,
                        Discount = game.Discount
                    };
                });

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.DISCOUNT, true);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Halo", sorted.First().GameTitle);  // Expecting lowest discount first
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByDiscount_AndDescendingOrder_ReturnsCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20, Rating = 3.5m, Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10, Rating = 4.5m, Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15, Rating = 2.5m, Discount = 20 }
    };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Price = game.Price,
                        Rating = (decimal)game.Rating,
                        Discount = game.Discount
                    };
                });

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.DISCOUNT, false);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Among Us", sorted.First().GameTitle);  // Expecting highest discount first
        }

        [Fact]
        public void ComputeTrendingScores_ComputesCorrectly()
        {
            var games = new Collection<Game>
            {
                new Game { GameTitle = "A", NumberOfRecentPurchases = 100 },
                new Game { GameTitle = "B", NumberOfRecentPurchases = 50 },
                new Game { GameTitle = "C", NumberOfRecentPurchases = 0 }
            };

            this.userGameService.ComputeTrendingScores(games);

            Assert.Equal(1.0m, games[0].TrendingScore);
            Assert.Equal(0.5m, games[1].TrendingScore);
            Assert.Equal(0.0m, games[2].TrendingScore);
        }

        [Fact]
        public async Task ComputeTagScoreForGamesAsync_ValidComputingFormula_AddsScore()
        {
            var games = new Collection<Game>
            {
                new Game { GameTitle = "Game1", Tags = new[] { "Action", "RPG" } },
                new Game { GameTitle = "Game2", Tags = new[] { "Puzzle" } }
            };

            var allTags = new List<TagSummaryResponse>
            {
                new TagSummaryResponse { TagId = 1, TagName = "Action" },
                new TagSummaryResponse { TagId = 2, TagName = "RPG" }
            };

            var userGames = new List<Game>
            {
                new Game { GameTitle = "GameX", Tags = new[] { "Action", "Action", "Puzzle" } }
            };

            this.tagServiceProxyMock.Setup(proxy => proxy.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = userGames.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Name = "GameX",
                    Tags = new List<TagDetailedResponse>
                    {
                        new TagDetailedResponse { TagId = 1, TagName = "Action" },
                        new TagDetailedResponse { TagId = 2, TagName = "Puzzle" },
                        new TagDetailedResponse { TagId = 1, TagName = "Action" }
                    }
                });

            await this.userGameService.ComputeTagScoreForGamesAsync(games);

            Assert.True(games.All(game => game.TagScore >= 0));
        }

        [Fact]
        public async Task GetFavoriteUserTagsAsync_ValidIds_ReturnsTopTags()
        {
            var allTags = new List<TagSummaryResponse>
            {
                 new TagSummaryResponse { TagId = 1, TagName = "Action" },
                 new TagSummaryResponse { TagId = 2, TagName = "Puzzle" },
                 new TagSummaryResponse { TagId = 3, TagName = "Adventure" },
                 new TagSummaryResponse { TagId = 4, TagName = "RPG" }
            };

            var purchasedGames = new List<Game>
            {
                new Game { GameId = 1, GameTitle = "G1", Tags = new List<string> { "Action", "Puzzle" }.ToArray() },
                new Game { GameId = 2, GameTitle = "G2", Tags = new List<string> { "Action", "RPG" }.ToArray() },
                new Game { GameId = 3, GameTitle = "G3", Tags = new List<string> { "Puzzle", "Adventure" }.ToArray() }
            };

            this.tagServiceProxyMock.Setup(proxy => proxy.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = purchasedGames.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = purchasedGames.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Tags = game.Tags.Select(tag => new TagDetailedResponse { TagId = 0, TagName = tag }).ToList()
                    };
                });

            var result = await this.userGameService.GetFavoriteUserTagsAsync();

            Assert.Equal(3, result.Count);
            Assert.True(result.Any(tag => tag.Tag_name == "Action"));
        }

        [Fact]
        public async Task AddGameToWishlistAsync_ValidGame_Successful()
        {
            var game = new Game { GameId = 7, GameTitle = "NewGame" };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserPurchasedGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            await this.userGameService.AddGameToWishlistAsync(game);

            this.userGameServiceProxyMock.Verify(proxy => proxy.AddToWishlistAsync(It.Is<UserGameRequest>(
                request => request.UserId == testUser.UserId && request.GameId == game.GameId)), Times.Once);
        }

        [Fact]
        public async Task RemoveGameFromWishlistAsync_ValidGameId_RemovesSuccessfully()
        {
            var game = new Game { GameId = 5 };

            await this.userGameService.RemoveGameFromWishlistAsync(game);

            this.userGameServiceProxyMock.Verify(proxy => proxy.RemoveFromWishlistAsync(It.Is<UserGameRequest>(
                request => request.UserId == testUser.UserId && request.GameId == game.GameId)), Times.Once);
        }

        [Fact]
        public async Task PurchaseGamesAsync_ValidGames_BuysCorrectGames()
        {
            var game1 = new Game { GameId = 10 };
            var game2 = new Game { GameId = 11 };
            var gamesToBuy = new List<Game> { game1, game2 };

            float originalPoints = testUser.PointsBalance;
            testUser.PointsBalance += 20;

            await this.userGameService.PurchaseGamesAsync(gamesToBuy);

            this.userGameServiceProxyMock.Verify(proxy => proxy.PurchaseGameAsync(It.IsAny<UserGameRequest>()), Times.Exactly(2));
        }

        [Fact]
        public async Task SearchWishListByNameAsync_ExistingName_ReturnsMatchingGames()
        {
            var searchText = "halo";
            var games = new Collection<Game>
            {
                new Game { GameId = 1, GameTitle = "Halo Infinite" },
                new Game { GameId = 2, GameTitle = "Zelda" }
            };

            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse { Identifier = id, Name = game.GameTitle };
                });

            var results = await this.userGameService.SearchWishListByNameAsync(searchText);

            Assert.Single(results);
            Assert.Contains(results, game => game.GameTitle.ToLower().Contains(searchText));
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs4Point5AndCriteriaIsOverwhelminglyPositive_ReturnsGame()
        {
            var game = new Game { GameId = 1, GameTitle = "Game1", Rating = 4.5m };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserWishlistAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Name = game.GameTitle,
                    Rating = game.Rating
                });

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.OVERWHELMINGLYPOSITIVE);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs4Point1AndCriteriaIsVeryPositive_ReturnsGame()
        {
            var game = new Game { GameId = 2, GameTitle = "Game1", Rating = 4.1m };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserWishlistAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Name = game.GameTitle,
                    Rating = game.Rating
                });

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.VERYPOSITIVE);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs3Point0AndCriteriaIsMixed_ReturnsGame()
        {
            var game = new Game { GameId = 3, GameTitle = "Game1", Rating = 3.0m };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserWishlistAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Name = game.GameTitle,
                    Rating = game.Rating
                });

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.MIXED);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenRatingIs1Point5AndCriteriaIsNegative_ReturnsGame()
        {
            var game = new Game { GameId = 4, GameTitle = "Game1", Rating = 1.5m };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserWishlistAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Name = game.GameTitle,
                    Rating = game.Rating
                });

            var result = await this.userGameService.FilterWishListGamesAsync(FilterCriteria.NEGATIVE);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_InvalidCriteria_ReturnsAll()
        {
            var games = new List<Game>
            {
                new Game { GameId = 1, GameTitle = "Halo", Rating = 3.5m },
                new Game { GameId = 2, GameTitle = "Zelda", Rating = 2.0m }
            };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserWishlistAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });

            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = game.GameTitle,
                        Rating = (decimal)game.Rating
                    };
                });

            var result = await this.userGameService.FilterWishListGamesAsync("INVALID");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task IsGamePurchasedAsync_GameNotExists_ReturnsFalse()
        {
            var game = new Game { GameId = 2 };

            this.userGameServiceProxyMock.Setup(proxy => proxy.GetUserPurchasedGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            var result = await this.userGameService.IsGamePurchasedAsync(game);

            Assert.False(result);
        }

        private void SetupUserWishlistWithGames(Collection<Game> games)
        {
            this.userGameServiceProxyMock.Setup(proxy =>
                proxy.GetUserWishlistAsync(testUser.UserId)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(game => new UserGamesResponse { GameId = game.GameId }).ToList()
                });
        }

        private void SetupGameServiceMock(Collection<Game> games)
        {
            this.gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var game = games.First(game => game.GameId == id);
                    return new GameDetailedResponse { Identifier = id, Name = game.GameTitle, Rating = game.Rating };
                });
        }
    }
}