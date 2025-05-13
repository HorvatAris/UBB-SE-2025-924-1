namespace SteamHub.Tests.Services
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;
    using Xunit;
    using SteamHub.ApiContract.Constants;
    using SteamHub.ApiContract.Models.Tag;

    public class UserGameServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IUsersGamesRepository> userGameRepositoryMock;
        private readonly Mock<IGameRepository> gameRepositoryMock;
        private readonly Mock<ITagRepository> tagRepositoryMock;
        private readonly Mock<IUserDetails> userDetailsMock;

        private readonly UserGameService userGameService;

        public UserGameServiceTests()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.userGameRepositoryMock = new Mock<IUsersGamesRepository>();
            this.gameRepositoryMock = new Mock<IGameRepository>();
            this.tagRepositoryMock = new Mock<ITagRepository>();
            this.userDetailsMock = new Mock<IUserDetails>();

            this.userDetailsMock.SetupGet(u => u.UserId).Returns(1);

            this.userGameService = new UserGameService(
                this.userRepositoryMock.Object,
                this.userGameRepositoryMock.Object,
                this.gameRepositoryMock.Object,
                this.tagRepositoryMock.Object,
                this.userDetailsMock.Object);
        }

        [Fact]
        public async Task RemoveGameFromWishlistAsync_ValidGameId_CallsRepositoryCorrectly()
        {
            var game = new Game { GameId = 1 };

            await this.userGameService.RemoveGameFromWishlistAsync(game);

            this.userGameRepositoryMock.Verify(repo => repo.RemoveFromWishlistAsync(
                It.Is<UserGameRequest>(req =>
                    req.GameId == game.GameId &&
                    req.UserId == 1)), Times.Once);
        }

        [Fact]
        public async Task AddGameToWishlistAsync_ValidId_CallsRepository()
        {
            var game = new Game { GameId = 2, GameTitle = "Not Owned Game" };

            this.userGameRepositoryMock
                .Setup(repo => repo.GetUserPurchasedGamesAsync(userDetailsMock.Object.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            await this.userGameService.AddGameToWishlistAsync(game);

            this.userGameRepositoryMock.Verify(repo => repo.AddToWishlistAsync(
                It.Is<UserGameRequest>(request =>
                    request.GameId == game.GameId && request.UserId == userDetailsMock.Object.UserId)), Times.Once);
        }


        [Fact]
        public async Task GetAllGamesAsync_WhenGamesExist_ReturnsMappedGames()
        {
            var userGamesResponse = new GetUserGamesResponse
            {
                UserGames = new List<UserGamesResponse>
        {
            new UserGamesResponse { GameId = 1 },
            new UserGamesResponse { GameId = 2 }
        }
            };

            this.userGameRepositoryMock.Setup(repo =>
                repo.GetUserGamesAsync(1)).ReturnsAsync(userGamesResponse);

            this.gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(1)).ReturnsAsync(
                new GameDetailedResponse { Identifier = 1, Name = "Game1", Price = 59.99m });

            this.gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(2)).ReturnsAsync(
                new GameDetailedResponse { Identifier = 2, Name = "Game2", Price = 39.99m });

            var result = await this.userGameService.GetAllGamesAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, game => game.GameId == 1 && game.GameTitle == "Game1");
            Assert.Contains(result, game => game.GameId == 2 && game.GameTitle == "Game2");
        }


        [Fact]
        public async Task PurchaseGamesAsync_ValidGames_UpdatesLastEarnedPoints()
        {
            var games = new List<Game> { new Game { GameId = 1 }, new Game { GameId = 2 } };

            this.userGameRepositoryMock.Setup(repo =>
                repo.PurchaseGameAsync(It.IsAny<UserGameRequest>()))
                .Returns(Task.CompletedTask);

            await this.userGameService.PurchaseGamesAsync(games, true);

            this.userGameRepositoryMock.Verify(repo =>
                repo.PurchaseGameAsync(It.IsAny<UserGameRequest>()), Times.Exactly(games.Count));

            Assert.Equal(0, this.userGameService.LastEarnedPoints);
        }


        [Fact]
        public async Task SearchWishListByNameAsync_ValidExistingName_FindsMatchingGames()
        {
            var wishlistGames = new List<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda" },
        new Game { GameId = 2, GameTitle = "Halo Infinite" },
        new Game { GameId = 3, GameTitle = "Cool Game" }
    };

            this.userGameRepositoryMock.Setup(repo =>
                repo.GetUserWishlistAsync(1)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = wishlistGames.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            this.gameRepositoryMock.Setup(repo =>
                repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var match = wishlistGames.First(g => g.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = match.GameTitle
                    };
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

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var result = await this.userGameService.FilterWishListGamesAsync("UNKNOWN");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByPriceAscending_ShouldReturnCorrectOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15 }
    };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Name = games.First(g => g.GameId == id).GameTitle,
                    Price = games.First(g => g.GameId == id).Price
                });

            var result = await userGameService.SortWishListGamesAsync(FilterCriteria.PRICE, true);

            Assert.Equal(10, result[0].Price);
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByPriceAscending_ShouldReturnCheapestGameFirst()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Price = 20 },
        new Game { GameId = 2, GameTitle = "Halo", Price = 10 },
        new Game { GameId = 3, GameTitle = "Among Us", Price = 15 }
    };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Name = games.First(g => g.GameId == id).GameTitle,
                    Price = games.First(g => g.GameId == id).Price
                });

            var result = await userGameService.SortWishListGamesAsync(FilterCriteria.PRICE, true);

            Assert.Equal("Halo", result[0].GameTitle);
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

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.PRICE, false);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Zelda", sorted.First().GameTitle);  
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

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.RATING, true);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Among Us", sorted.First().GameTitle);  
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

            SetupUserWishlistWithGames(games);
            SetupGameServiceMock(games);

            var sorted = await this.userGameService.SortWishListGamesAsync(FilterCriteria.RATING, false);

            Assert.Equal(3, sorted.Count);
            Assert.Equal("Halo", sorted[0].GameTitle);     
            Assert.Equal("Zelda", sorted[1].GameTitle);
            Assert.Equal("Among Us", sorted[2].GameTitle);
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByDiscountDescending_ShouldReturnHighestDiscountFirst()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, Discount = 10 },
        new Game { GameId = 2, Discount = 5 },
        new Game { GameId = 3, Discount = 20 }
    };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Discount = games.First(g => g.GameId == id).Discount
                });

            var result = await userGameService.SortWishListGamesAsync(FilterCriteria.DISCOUNT, false);

            Assert.Equal(20, result[0].Discount);
        }

        [Fact]
        public async Task SortWishListGamesAsync_WhenSortedByDiscountDescending_ShouldReturnCorrectGameOrder()
        {
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Zelda", Discount = 10 },
        new Game { GameId = 2, GameTitle = "Halo", Discount = 5 },
        new Game { GameId = 3, GameTitle = "Among Us", Discount = 20 }
    };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Name = games.First(g => g.GameId == id).GameTitle,
                    Discount = games.First(g => g.GameId == id).Discount
                });

            var result = await userGameService.SortWishListGamesAsync(FilterCriteria.DISCOUNT, false);

            Assert.Equal("Among Us", result[0].GameTitle);
            Assert.Equal("Zelda", result[1].GameTitle);
            Assert.Equal("Halo", result[2].GameTitle);
        }

        [Fact]
        public void ComputeTrendingScores_WhenMultipleGames_ShouldCalculateCorrectScores()
        {
            var games = new Collection<Game>
    {
        new Game { NumberOfRecentPurchases = 100 },
        new Game { NumberOfRecentPurchases = 50 },
        new Game { NumberOfRecentPurchases = 0 }
    };

            userGameService.ComputeTrendingScores(games);

            Assert.Equal(1.0m, games[0].TrendingScore);
            Assert.Equal(0.5m, games[1].TrendingScore);
            Assert.Equal(0.0m, games[2].TrendingScore);
        }

        [Fact]
        public async Task ComputeTagScoreForGamesAsync_WhenCalled_ShouldSetNonNegativeTagScores()
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

            tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            userGameRepositoryMock.Setup(repo => repo.GetUserGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = 1 } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Puzzle" }
                    }
                });

            await userGameService.ComputeTagScoreForGamesAsync(games);

            Assert.True(games.All(game => game.TagScore >= 0));
        }

        [Fact]
        public async Task GetFavoriteUserTagsAsync_WhenCalled_ShouldReturnThreeTags()
        {
            var allTags = new List<TagSummaryResponse>
    {
        new TagSummaryResponse { TagId = 1, TagName = "Action" },
        new TagSummaryResponse { TagId = 2, TagName = "Puzzle" },
        new TagSummaryResponse { TagId = 3, TagName = "Adventure" },
        new TagSummaryResponse { TagId = 4, TagName = "RPG" }
    };

            tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            userGameRepositoryMock.Setup(repo => repo.GetUserGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                new UserGamesResponse { GameId = 1 },
                new UserGamesResponse { GameId = 2 },
                new UserGamesResponse { GameId = 3 }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(1))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Puzzle" }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(2))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "RPG" }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(3))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Puzzle" },
                new TagDetailedResponse { TagName = "Adventure" }
                    }
                });

            var result = await userGameService.GetFavoriteUserTagsAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetFavoriteUserTagsAsync_WhenCalled_ShouldIncludeMostCommonTag()
        {
            var allTags = new List<TagSummaryResponse>
    {
        new TagSummaryResponse { TagId = 1, TagName = "Action" },
        new TagSummaryResponse { TagId = 2, TagName = "Puzzle" }
    };

            tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            userGameRepositoryMock.Setup(repo => repo.GetUserGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                new UserGamesResponse { GameId = 1 },
                new UserGamesResponse { GameId = 2 }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Action" }
                    }
                });

            var result = await userGameService.GetFavoriteUserTagsAsync();

            Assert.Contains(result, tag => tag.Tag_name == "Action");
        }

        [Fact]
        public async Task AddGameToWishlistAsync_WhenGameNotPurchased_ShouldCallAddToWishlist()
        {
            var game = new Game { GameId = 7, GameTitle = "NewGame" };

            userGameRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            await userGameService.AddGameToWishlistAsync(game);

            userGameRepositoryMock.Verify(repo => repo.AddToWishlistAsync(
                It.Is<UserGameRequest>(r => r.UserId == 1 && r.GameId == game.GameId)),
                Times.Once);
        }

        [Fact]
        public async Task RemoveGameFromWishlistAsync_WhenCalled_ShouldCallRemoveFromWishlist()
        {
            var game = new Game { GameId = 5 };

            await userGameService.RemoveGameFromWishlistAsync(game);

            userGameRepositoryMock.Verify(repo => repo.RemoveFromWishlistAsync(
                It.Is<UserGameRequest>(r => r.UserId == 1 && r.GameId == game.GameId)),
                Times.Once);
        }

        [Fact]
        public async Task PurchaseGamesAsync_WhenCalledWithMultipleGames_ShouldCallPurchaseForEachGame()
        {
            var gamesToBuy = new List<Game>
    {
        new Game { GameId = 10 },
        new Game { GameId = 11 }
    };

            await userGameService.PurchaseGamesAsync(gamesToBuy, true);

            userGameRepositoryMock.Verify(repo => repo.PurchaseGameAsync(
                It.Is<UserGameRequest>(r => r.UserId == 1)),
                Times.Exactly(2));
        }

        [Fact]
        public async Task PurchaseGamesAsync_WhenCalled_ShouldUpdateUserRepository()
        {
            var gamesToBuy = new List<Game> { new Game { GameId = 10, Price = 20m } };

            await userGameService.PurchaseGamesAsync(gamesToBuy, true);

            userRepositoryMock.Verify(repo => repo.UpdateUserAsync(
                It.IsAny<int>(),
                It.IsAny<UpdateUserRequest>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchWishListByNameAsync_WhenNameMatches_ShouldReturnFilteredGames()
        {
            var searchText = "halo";
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Halo Infinite" },
        new Game { GameId = 2, GameTitle = "Zelda" }
    };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Name = games.First(g => g.GameId == id).GameTitle
                });

            var results = await userGameService.SearchWishListByNameAsync(searchText);

            Assert.Single(results);
        }

        [Fact]
        public async Task SearchWishListByNameAsync_WhenNameMatches_ShouldReturnCorrectGame()
        {
            var searchText = "halo";
            var games = new Collection<Game>
    {
        new Game { GameId = 1, GameTitle = "Halo Infinite" }
    };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new GameDetailedResponse
                {
                    Identifier = id,
                    Name = games.First(g => g.GameId == id).GameTitle
                });

            var results = await userGameService.SearchWishListByNameAsync(searchText);

            Assert.Contains("Halo", results[0].GameTitle);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenOverwhelminglyPositive_ShouldReturnMatchingGame()
        {
            var game = new Game { GameId = 1, Rating = 4.5m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });

            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.OVERWHELMINGLYPOSITIVE);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenVeryPositive_ShouldReturnMatchingGame()
        {
            var game = new Game { GameId = 2, Rating = 4.1m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });

            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.VERYPOSITIVE);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenOverwhelminglyPositive_ShouldReturnGameWithCorrectRating()
        {
            var game = new Game { GameId = 1, Rating = 4.5m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });

            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.OVERWHELMINGLYPOSITIVE);

            Assert.Equal(4.5m, result[0].Rating);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenMixedRating_ShouldReturnMatchingGame()
        {
            var game = new Game { GameId = 3, Rating = 3.0m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });

            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.MIXED);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenNegativeRating_ShouldReturnMatchingGame()
        {
            var game = new Game { GameId = 4, Rating = 1.5m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });
            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.NEGATIVE);

            Assert.Single(result);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenMixedRating_ShouldReturnCorrectRating()
        {
            var game = new Game { GameId = 3, Rating = 3.0m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });

            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.MIXED);

            Assert.Equal(3.0m, result[0].Rating);
        }

        [Fact]
        public async Task FilterWishListGamesAsync_WhenNegativeRating_ShouldReturnCorrectRating()
        {
            var game = new Game { GameId = 4, Rating = 1.5m };

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = game.GameId } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(game.GameId))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Identifier = game.GameId,
                    Rating = game.Rating,
                    Status = GameStatusEnum.Approved
                });

            var result = await userGameService.FilterWishListGamesAsync(FilterCriteria.NEGATIVE);

            Assert.Equal(1.5m, result[0].Rating);
        }

        [Fact]
        public async Task IsGamePurchasedAsync_WhenGameNotPurchased_ShouldReturnFalse()
        {
            var game = new Game { GameId = 2 };

            userGameRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            var result = await userGameService.IsGamePurchasedAsync(game);

            Assert.False(result);
        }

        private void SetupUserWishlistWithGames(IEnumerable<Game> games)
        {
            this.userGameRepositoryMock.Setup(repo =>
                repo.GetUserWishlistAsync(1)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });
        }

        private void SetupGameServiceMock(IEnumerable<Game> games)
        {
            this.gameRepositoryMock.Setup(repo =>
                repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var match = games.First(g => g.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = match.GameTitle,
                        Rating = match.Rating 
                    };
                });
        }

        }
}
