namespace SteamHub.Tests.Services
{
    using Moq;
    using Xunit;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;

    public class GameServiceTest
    {
        private const string TEST_TAG_1 = "tag1";
        private const string TEST_TAG_2 = "tag2";
        private const string TEST_NAME = "Test";
        private const string NOT_MATCH_NAME = "NoMatch";
        private const string TEST_GAME_1 = "test Game 1";
        private const string TEST_GAME_2 = "TEST Game 2";
        private const string TEST_GAME_3 = "Game 2";
        private readonly GameService subject;
        private readonly Mock<IGameRepository> gameRepoMock;
        private readonly Mock<ITagRepository> tagRepoMock;

        public GameServiceTest()
        {
            gameRepoMock = new Mock<IGameRepository>();
            tagRepoMock = new Mock<ITagRepository>();
            subject = new GameService(gameRepoMock.Object, tagRepoMock.Object);
        }

        [Fact]
        public async Task SearchGamesAsync_WhenQueryMatches_ReturnsCorrectNumberOfGames()
        {
            var allGames = new List<GameDetailedResponse>
        {
            new() { Name = TEST_GAME_1, Status = GameStatusEnum.Approved },
            new() { Name = TEST_GAME_2, Status = GameStatusEnum.Approved },
            new() { Name = TEST_GAME_3, Status = GameStatusEnum.Approved }
        };
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(allGames);

            var result = await subject.SearchGamesAsync(TEST_NAME);

            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task SearchGamesAsync_WhenQueryMatches_IncludesGame()
        {
            var allGames = new List<GameDetailedResponse> { new() { Name = TEST_GAME_1, Status = GameStatusEnum.Approved } };
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(allGames);

            var result = await subject.SearchGamesAsync(TEST_NAME);

            Assert.Contains(result, g => g.GameTitle == TEST_GAME_1);
        }

        [Fact]
        public async Task SearchGamesAsync_WhenNoMatch_ReturnsEmptyList()
        {
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(new List<GameDetailedResponse>());

            var result = await subject.SearchGamesAsync(NOT_MATCH_NAME);

            Assert.Empty(result);
        }
        [Fact]
        public async Task GetAllTagsAsync_WhenCalled_ReturnsCorrectNumberOfTags()
        {
            var apiTags = new GetTagsResponse
            {
                Tags = new List<TagSummaryResponse>
            {
                new TagSummaryResponse { TagName = TEST_TAG_1 },
                new TagSummaryResponse { TagName = TEST_TAG_2 }
            }
            };
            tagRepoMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(apiTags);

            var actualTags = await subject.GetAllTagsAsync();

            Assert.Equal(2, actualTags.Count);
        }

        [Fact]
        public async Task GetAllTagsAsync_WhenCalled_IncludesTag()
        {
            var apiTags = new GetTagsResponse
            {
                Tags = new List<TagSummaryResponse>
            {
                new TagSummaryResponse { TagName = TEST_TAG_1 },
                new TagSummaryResponse { TagName = TEST_TAG_2 }
            }
            };
            tagRepoMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(apiTags);

            var actualTags = await subject.GetAllTagsAsync();

            Assert.Contains(actualTags, t => t.Tag_name == TEST_TAG_1);
        }

        [Fact]
        public async Task GetTrendingGamesAsync_ReturnsOnlyApprovedGames()
        {
            var allGames = new List<GameDetailedResponse>
        {
            new() { Name = "Game1", Status = GameStatusEnum.Approved, NumberOfRecentPurchases = 5 },
            new() { Name = "Game2", Status = GameStatusEnum.Rejected, NumberOfRecentPurchases = 10 }
        };
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(allGames);

            var result = await subject.GetTrendingGamesAsync();

            Assert.All(result, game => Assert.Equal("Approved", game.Status));
        }

        [Fact]
        public async Task GetTrendingGamesAsync_SortsByTrendingScoreDescending()
        {
            var allGames = new List<GameDetailedResponse>
        {
            new() { Name = "Game1", Status = GameStatusEnum.Approved, NumberOfRecentPurchases = 5 },
            new() { Name = "Game2", Status = GameStatusEnum.Approved, NumberOfRecentPurchases = 10 }
        };
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(allGames);

            var result = await subject.GetTrendingGamesAsync();

            Assert.Equal("Game2", result[0].GameTitle);
        }

        [Fact]
        public async Task GetDiscountedGamesAsync_ReturnsOnlyGamesWithDiscount()
        {
            var allGames = new List<GameDetailedResponse>
        {
            new() { Name = "Game1", Status = GameStatusEnum.Approved, Discount = 5 },
            new() { Name = "Game2", Status = GameStatusEnum.Approved, Discount = 0 }
        };
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(allGames);

            var result = await subject.GetDiscountedGamesAsync();

            Assert.All(result, game => Assert.True(game.Discount > 0));
        }
        [Fact]
        public async Task GetSimilarGamesAsync_WhenCalledWithGameId_ShouldReturnOtherGames()
        {
            var allGames = new List<GameDetailedResponse>
        {
            new GameDetailedResponse { Identifier = 1, Name = "Game1", Status = GameStatusEnum.Approved },
            new GameDetailedResponse { Identifier = 2, Name = "Game2", Status = GameStatusEnum.Approved },
            new GameDetailedResponse { Identifier = 3, Name = "Game3", Status = GameStatusEnum.Approved}
        };
            gameRepoMock.Setup(repo => repo.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(allGames);

            var similarGames = await subject.GetSimilarGamesAsync(1);

            Assert.DoesNotContain(similarGames, g => g.GameId == 1);
        }

        [Fact]
        public async Task GetGameByIdAsync_MapsGameIdCorrectly()
        {
            const int gameId = 42;
            gameRepoMock.Setup(repo => repo.GetGameByIdAsync(gameId))
                .ReturnsAsync(new GameDetailedResponse { Identifier = gameId, Name = "Sample Game" });

            var result = await subject.GetGameByIdAsync(gameId);

            Assert.Equal(gameId, result.GameId);
        }

        [Fact]
        public async Task GetGameByIdAsync_MapsGameTitleCorrectly()
        {
            gameRepoMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GameDetailedResponse { Name = "Sample Game" });

            var result = await subject.GetGameByIdAsync(1);

            Assert.Equal("Sample Game", result.GameTitle);
        }
        [Fact]
        public async Task FilterGamesAsync_WhenTagsMatch_ReturnsGamesWithMatchingTags()
        {
            var testTag = TEST_TAG_1;
            var games = new List<GameDetailedResponse>
    {
        new() { Name = "Game1", Status = GameStatusEnum.Approved, Tags = new List<TagDetailedResponse> { new() { TagName = testTag } } },
        new() { Name = "Game2", Status = GameStatusEnum.Approved, Tags = new List<TagDetailedResponse>() }
    };

            gameRepoMock.Setup(x => x.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                       .ReturnsAsync(games);

            var result = await subject.FilterGamesAsync(0, 0, int.MaxValue, new[] { testTag });

            Assert.All(result, game => Assert.Contains(testTag, game.Tags));
        }

        [Fact]
        public async Task FilterGamesAsync_WhenRatingMatches_ReturnsGamesAboveMinimumRating()
        {

            var minRating = 4;
            var games = new List<GameDetailedResponse>
    {
        new() { Name = "Game1", Status = GameStatusEnum.Approved, Rating = 5 },
        new() { Name = "Game2", Status = GameStatusEnum.Approved, Rating = 3 }
    };

            gameRepoMock.Setup(x => x.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                       .ReturnsAsync(games);

            var result = await subject.FilterGamesAsync(minRating, 0, int.MaxValue, Array.Empty<string>());

            Assert.All(result, game => Assert.True(game.Rating >= minRating));
        }

        [Fact]
        public async Task FilterGamesAsync_WhenPriceInRange_ReturnsGamesWithinPriceRange()
        {

            var minPrice = 10;
            var maxPrice = 20;
            var games = new List<GameDetailedResponse>
    {
        new() { Name = "Game1", Status = GameStatusEnum.Approved, Price = 15 },
        new() { Name = "Game2", Status = GameStatusEnum.Approved, Price = 25 }
    };

            gameRepoMock.Setup(x => x.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                       .ReturnsAsync(games);

            var result = await subject.FilterGamesAsync(0, minPrice, maxPrice, Array.Empty<string>());


            Assert.All(result, game => Assert.InRange(game.Price, minPrice, maxPrice));
        }

        [Fact]
        public async Task FilterGamesAsync_WhenNoTagsProvided_ReturnsAllMatchingGames()
        {

            var games = new List<GameDetailedResponse>
    {
        new() { Name = "Game1", Status = GameStatusEnum.Approved, Tags = new List<TagDetailedResponse>() },
        new() { Name = "Game2", Status = GameStatusEnum.Approved, Tags = new List<TagDetailedResponse>() }
    };

            gameRepoMock.Setup(x => x.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                       .ReturnsAsync(games);

            var result = await subject.FilterGamesAsync(0, 0, int.MaxValue, Array.Empty<string>());

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task FilterGamesAsync_WhenMultipleCriteriaMatch_ReturnsIntersection()
        {
            var testTag = TEST_TAG_1;
            var minRating = 4;
            var minPrice = 10;
            var maxPrice = 20;

            var games = new List<GameDetailedResponse>
    {
        new() {
            Name = "Game1",
            Status = GameStatusEnum.Approved,
            Rating = 5,
            Price = 15,
            Tags = new List<TagDetailedResponse> { new() { TagName = testTag } }
        },
        new() {
            Name = "Game2",
            Status = GameStatusEnum.Approved,
            Rating = 3,
            Price = 15,
            Tags = new List<TagDetailedResponse> { new() { TagName = testTag } }
        },
        new() {
            Name = "Game3",
            Status = GameStatusEnum.Approved,
            Rating = 5,
            Price = 25,
            Tags = new List<TagDetailedResponse> { new() { TagName = testTag } }
        }
    };

            gameRepoMock.Setup(x => x.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                       .ReturnsAsync(games);

            var result = await subject.FilterGamesAsync(minRating, minPrice, maxPrice, new[] { testTag });

            Assert.Equal("Game1", result[0].GameTitle);
        }
    }
}