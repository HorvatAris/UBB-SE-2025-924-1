namespace SteamHub.Tests.Services
{
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Threading.Tasks;
    using SteamHub.Models;
    using SteamHub.Proxies;
    using Moq;
    using Refit;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.Models;
    using Xunit;

    public class DeveloperServiceTests
    {
        private const int TestGameId = 1;
        private const string TestGameIdText = "1";

        private const int TestGamePrice = 10;
        private const string TestGamePriceText = "10";

        private const string TestGameNameText = "Test";
        private const string TestGameDescriptionText = "Desc";
        private const string TestGameImageInfoText = "img.png";
        private const string TestGameTrailerInfoText = "trailer";
        private const string TestGameGameplayInfoText = "gameplay";
        private const string TestGameMinimumRequirementText = "min";
        private const string TestGameRecommendedRequirementText = "rec";
        private const decimal TestGameDiscount = 0;
        private const string TestGameDiscountText = "0";

        private const int TestRating = 0;
        private const int TestPublisherIdentifier = 1;

        private const string TestPendingGameStatusText = "Pending";

        private const int TestTrendingScore = 0;
        private const int TestTagScore = 0;
        private const int TestNumberOfRecentPurchases = 0;

        private const int TestTagId = 1;
        private const int TestSecondTagId = 2;

        private readonly DeveloperService developerService;
        private readonly Mock<GameRepositoryProxy> gameServiceProxyMock;
        private readonly Mock<TagRepositoryProxy> tagServiceMock;
        private readonly Mock<UserGamesRepositoryProxy> userGameServiceProxyMock;
        private readonly Mock<UserRepositoryProxy> userServiceProxyMock;
        private readonly Mock<ItemRepositoryProxy> itemServiceProxyMock;
        private readonly Mock<ItemTradeDetailsRepositoryProxy> itemTradeDetailServiceProxyMock;

        private readonly User testUser;

        public DeveloperServiceTests()
        {
            gameServiceProxyMock = new Mock<GameRepositoryProxy>();
            tagServiceMock = new Mock<TagRepositoryProxy>();
            userGameServiceProxyMock = new Mock<UserGamesRepositoryProxy>();
            userServiceProxyMock = new Mock<UserRepositoryProxy>();
            itemServiceProxyMock = new Mock<ItemRepositoryProxy>();
            itemTradeDetailServiceProxyMock = new Mock<ItemTradeDetailsRepositoryProxy>();
            testUser = new User { UserId = 1, WalletBalance = 50f };

            developerService = new DeveloperService(gameServiceProxyMock.Object, tagServiceMock.Object, userGameServiceProxyMock.Object,
                userServiceProxyMock.Object, itemServiceProxyMock.Object, itemTradeDetailServiceProxyMock.Object, testUser);
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenValid_ShouldReturnGame()
        {
            var gameIdText = TestGameIdText;
            var gameName = TestGameNameText;
            var gamePriceText = TestGamePriceText;
            var gameDescription = TestGameDescriptionText;
            var gameImageUrl = TestGameImageInfoText;
            var gameTrailerUrl = TestGameTrailerInfoText;
            var gameGameplayUrl = TestGameGameplayInfoText;
            var gameMinimumRequirement = TestGameMinimumRequirementText;
            var gameRecommendedRequirement = TestGameRecommendedRequirementText;
            var gameDiscountText = TestGameDiscountText;
            var tags = new List<Tag> { new Tag { TagId = TestTagId } };

            var expectedGame = new Game
            {
                GameId = TestGameId,
                GameDescription = TestGameDescriptionText,
                Discount = TestGameDiscount,
                GameplayPath = TestGameGameplayInfoText,
                ImagePath = TestGameImageInfoText,
                TrailerPath = TestGameTrailerInfoText,
                MinimumRequirements = TestGameMinimumRequirementText,
                RecommendedRequirements = TestGameRecommendedRequirementText,
                GameTitle = TestGameNameText,
                Price = TestGamePrice,
                Rating = TestRating,
                PublisherIdentifier = TestPublisherIdentifier,
                Status = TestPendingGameStatusText,
                NumberOfRecentPurchases = TestNumberOfRecentPurchases,
                TagScore = TestTagScore,
                TrendingScore = TestTrendingScore,
            };

            var returnedGame = developerService.ValidateInputForAddingAGame(gameIdText, gameName, gamePriceText, gameDescription, gameImageUrl, gameTrailerUrl, gameGameplayUrl, gameMinimumRequirement, gameRecommendedRequirement, gameDiscountText, tags);

            Assert.Equivalent(expectedGame, returnedGame);
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenGameIdentifierEmpty_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame(string.Empty, "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenGameNameEmpty_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", string.Empty, "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenGameIdentifierNonNumeric_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("abc", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenPriceNegative_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", "name", "-10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenDiscountNotNumeric_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "abc", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenDiscountTooHigh_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "150", new List<Tag> { new Tag() }));
        }

        [Fact]
        public async Task IsGameIdInUse_WhenIdInUse_ShouldReturnTrue()
        {
            var expectedGameIdentifier = TestGameId;

            gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(expectedGameIdentifier)).ReturnsAsync(new GameDetailedResponse());

            var result = await developerService.IsGameIdInUseAsync(expectedGameIdentifier);

            Assert.True(result);
        }

        [Fact]
        public async Task IsGameIdInUse_WhenIdNotInUse_ShouldReturnFalse()
        {
            var expectedGameIdentifier = TestGameId;

            var exception = await ApiException.Create(
                new HttpRequestMessage(HttpMethod.Get, $"https://fake/api/games/{expectedGameIdentifier}"),
                HttpMethod.Get,
                new HttpResponseMessage(HttpStatusCode.NotFound),
                new RefitSettings());

            gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(expectedGameIdentifier)).ThrowsAsync(exception);

            var result = await developerService.IsGameIdInUseAsync(expectedGameIdentifier);

            Assert.False(result);
        }

        [Fact]
        public async Task GetGameOwnerCount_WhenCalledForAGameWithNonZeroOwnerCount_ShouldReturnProperCount()
        {
            var expectedGameIdentifier = TestGameId;
            var expectedGameOwnerCount = 2;
            var testUserId1 = 1;
            var testUserId2 = 2;

            userServiceProxyMock.Setup(proxy => proxy.GetUsersAsync()).ReturnsAsync(new GetUsersResponse
            {
                Users = new List<UserResponse>
                {
                    new UserResponse { UserId = 1, WalletBalance = 50f },
                    new UserResponse { UserId = 2, WalletBalance = 60f }
                }
            });

            userGameServiceProxyMock.Setup(proxy => proxy.GetUserGamesAsync(testUserId1))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                        new UserGamesResponse { GameId = expectedGameIdentifier, IsPurchased = true }
                    }
                });

            userGameServiceProxyMock.Setup(proxy => proxy.GetUserGamesAsync(testUserId2))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                        new UserGamesResponse { GameId = expectedGameIdentifier, IsPurchased = true }
                    }
                });

            var result = await developerService.GetGameOwnerCountAsync(expectedGameIdentifier);

            Assert.Equal(expectedGameOwnerCount, result);
        }

        [Fact]
        public void GetCurrentUser_WhenValid_ShouldReturnUser()
        {
            var user = developerService.GetCurrentUser();

            Assert.Equal(testUser, user);
        }

        [Fact]
        public async Task CreateValidatedGame_WhenIdInUse_ShouldThrow()
        {
            var gameIdText = TestGameIdText;
            var name = TestGameNameText;
            var priceText = TestGamePriceText;
            var description = TestGameDescriptionText;
            var imageUrl = TestGameImageInfoText;
            var tralerUrl = TestGameTrailerInfoText;
            var gameplayUrl = TestGameGameplayInfoText;
            var minimumRequirement = TestGameMinimumRequirementText;
            var recommendedRequirement = TestGameRecommendedRequirementText;
            var dicountText = TestGameDiscountText;
            var tags = new List<Tag> { new Tag { TagId = TestTagId } };

            var expectedGameIdentifier = TestGameId;

            gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(expectedGameIdentifier)).ReturnsAsync(new GameDetailedResponse());

            await Assert.ThrowsAsync<Exception>(() =>
                developerService.CreateValidatedGameAsync(gameIdText, name, priceText, description, imageUrl, tralerUrl, gameplayUrl, minimumRequirement, recommendedRequirement, dicountText, tags));
        }

        [Fact]
        public async Task DeleteGame_WhenDeletingValidGame_ShouldRemoveFromCollection()
        {
            var gameList = new ObservableCollection<Game> { new Game() { GameId = TestGameId } };
            var expectedIdentifier = TestGameId;

            await developerService.DeleteGameAsync(expectedIdentifier, gameList);

            Assert.Empty(gameList);
        }

        [Fact]
        public async Task UpdateGameAndRefreshList_WhenUpdatingValidGame_ShouldUpdateCorrectly()
        {
            var existing = new Game { GameId = TestGameId };
            var updated = new Game { GameId = TestGameId, GameTitle = "Updated" };
            var games = new ObservableCollection<Game> { existing };

            await developerService.UpdateGameAndRefreshListAsync(updated, games);

            Assert.Contains(updated, games);
        }

        [Fact]
        public async Task RejectGameAndRemoveFromUnvalidated_WhenRemovingGameFromList_ShouldHaveListEmpty()
        {
            var games = new ObservableCollection<Game> { new Game { GameId = TestGameId } };
            var expectedIdentifier = TestGameId;

            await developerService.RejectGameAndRemoveFromUnvalidatedAsync(expectedIdentifier, games);

            Assert.Empty(games);
        }

        [Fact]
        public async Task GetMatchingTagsForGame_WhenHavingTags_ShouldReturnAsManyMatchingTags()
        {
            var allTags = new List<Tag> { new Tag() { TagId = TestTagId }, new Tag() { TagId = TestSecondTagId } };
            var expectedIdentifier = TestGameId;
            var expectedMatchingTagsCount = 1;

            gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(expectedIdentifier)).ReturnsAsync(new GameDetailedResponse
            {
                Identifier = expectedIdentifier,
                Tags = new List<TagDetailedResponse>
                {
                    new TagDetailedResponse
                {
                    TagId = TestTagId
                }
                }
            });
            var actualMatchingTags = await developerService.GetMatchingTagsForGameAsync(expectedIdentifier, allTags);

            Assert.Equal(expectedMatchingTagsCount, actualMatchingTags.Count);
        }
    }
}