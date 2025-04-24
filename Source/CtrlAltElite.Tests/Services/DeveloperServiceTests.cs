using System.Collections.ObjectModel;
using System.Globalization;
using Moq;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;

namespace SteamStore.Tests.Services
{
	public class DeveloperServiceTests
	{
		private readonly DeveloperService service;
		private readonly Mock<IGameRepository> gameRepositoryMock = new Mock<IGameRepository>();
		private readonly Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();
		private readonly Mock<IUserGameRepository> userGameRepositoryMock = new Mock<IUserGameRepository>();

		private const int TestGameId = 1;
		private const int TestGamePrice = 10;
		private const int TestGameDescription = 5;
		private const string TestGameIdText = "1";
		private const string TestGameNameText = "Test";
		private const string TestGamePriceText = "10";
		private const string TestGameDescriptionText = "Desc";
		private const string TestGameImageInfoText = "img.png";
        private const string TestGameTrailerInfoText = "trailer";
        private const string TestGameGameplayInfoText = "gameplay";
        private const string TestGameMinimumRequirementText = "min";
        private const string TestGameRecommendedRequirementText = "rec";
        private const string TestGameDiscountText = "5";
		private const string TestGameNoDiscountText = "0";
		private const string TestPendingGameStatusText = "Pending";
		private const int TestRating = 0;
		private const int TestPublisherIdentifier = 0;
		private const int TestTrendingScore = 0;
		private const int TestTagScore = 0;
		private const int TestNumberOfRecentPurchases = 0;
		private const int TestTagId = 1;
		private const int TestSecondTagId = 2;

		private readonly User testUser = new User() { UserId = 42 };

		public DeveloperServiceTests()
		{
			service = new DeveloperService
			{
				GameRepository = gameRepositoryMock.Object,
				TagRepository = tagRepositoryMock.Object,
				UserGameRepository = userGameRepositoryMock.Object,
				User = testUser
			};
		}

		[Fact]
		public void ValidateGame_WhenValid_ShouldCallRepository()
		{
			service.ValidateGame(TestGameId);
			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.ValidateGame(TestGameId), Times.Once);
		}

		[Fact]
		public void ValidateInputForAddingAGame_WhenValid_ShouldReturnGame()
		{
			var gameIdText = TestGameIdText;
			var gameName = TestGameNameText;
			var gamePriceText = TestGamePriceText;
			var gameDescription = TestGameDescriptionText;
			var gameImageUrl = TestGameImageInfoText;
			var gameTrailerUrk = TestGameTrailerInfoText;
			var gameGameplayUrl = TestGameGameplayInfoText;
			var gameMinimumRequirement = TestGameMinimumRequirementText;
			var gameRecommendedRequirement = TestGameRecommendedRequirementText;
			var gameDicountText = TestGameDiscountText;
			var tags = new List<Tag> { new Tag { TagId = TestTagId } };

			var expectedGame = new Game
			{
				GameId = TestGameId,
				GameDescription = TestGameDescriptionText,
				Discount = TestGameDescription,
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

			var returnedGame = service.ValidateInputForAddingAGame(gameIdText, gameName, gamePriceText, gameDescription, gameImageUrl, gameTrailerUrk, gameGameplayUrl, gameMinimumRequirement, gameRecommendedRequirement, gameDicountText, tags);

			Assert.Equivalent(expectedGame, returnedGame);
		}

		[Theory]
		[InlineData("", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("1", "", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("abc", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("1", "name", "-10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "abc")]
		[InlineData("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "150")]
		public void ValidateInputForAddingAGame_WhenInvalid_ShouldThrow(
			string gameIdentifier, string gameName, string gamePrice, string gameDescription, string gameImageUrl, string gameTrailerUrl, string gameGameplayUrl, string gameMinimumRequirement, string gameRecommendedRequirement, string gameDiscountText)
		{
			Assert.Throws<Exception>(() =>
				service.ValidateInputForAddingAGame(gameIdentifier, gameName, gamePrice, gameDescription, gameImageUrl, gameTrailerUrl, gameGameplayUrl, gameMinimumRequirement, gameRecommendedRequirement, gameDiscountText, new List<Tag> { new Tag() }));
		}

		[Fact]
		public void FindGameInObservableCollectionById_WhenValid_ShouldReturnGame()
		{
			var gamesToSearchIn = new ObservableCollection<Game> { new Game { GameId = TestGameId } };

			var searchedIdentifier = TestGameId;

			var foundGames = service.FindGameInObservableCollectionById(searchedIdentifier, gamesToSearchIn);

			Assert.NotNull(foundGames);
		}

		[Fact]
		public void CreateGame_WhenValid_ShouldCallRepository()
		{
			var gameToCreate = new Game { GameId = TestGameId };

			service.CreateGame(gameToCreate);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.CreateGame(gameToCreate), Times.Once);
		}

		[Fact]
		public void CreateGameWithTags_WhenValid_ShouldCallCreate()
		{
			var gameToCreate = new Game { GameId = TestGameId };
			var tagsToAttribute = new List<Tag> { new Tag() { TagId = TestSecondTagId } };

			var expectedGameIdentifier = TestGameId;
			var expectedTagId = TestSecondTagId;

			service.CreateGameWithTags(gameToCreate, tagsToAttribute);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.InsertGameTag(expectedGameIdentifier, expectedTagId), Times.Once);
		}

		[Fact]
		public void CreateGameWithTags_WhenValid_ShouldCallInsert()
		{
			var gameToCreate = new Game { GameId = TestGameId };
			var tagsToAttribute = new List<Tag> { new Tag() { TagId = TestSecondTagId } };

			var expectedGameIdentifier = TestGameId;
			var expectedTagId = TestSecondTagId;

			service.CreateGameWithTags(gameToCreate, tagsToAttribute);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.InsertGameTag(expectedGameIdentifier, expectedTagId), Times.Once);
		}

		[Fact]
		public void UpdateGame_WhenValid_ShouldCallUpdateWithUserId()
		{
			var gameToUpdate = new Game { GameId = TestGameId };

			var expectedGameIdentifier = TestGameId;

			service.UpdateGame(gameToUpdate);
			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.UpdateGame(expectedGameIdentifier, gameToUpdate), Times.Once);
		}

		[Fact]
		public void UpdateGameWithTags_WhenValid_ShouldCallUpdate()
		{
			var gameToUpdate = new Game { GameId = TestGameId };
			var tagsToAttribute = new List<Tag> { new Tag() { TagId = TestSecondTagId } };

			var expectedGameIdentifier = TestGameId;
			var expectedTagIdentifier = TestSecondTagId;

			service.UpdateGameWithTags(gameToUpdate, tagsToAttribute);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.UpdateGame(expectedGameIdentifier, gameToUpdate), Times.Once);
		}

		[Fact]
		public void UpdateGameWithTags_WhenValid_ShouldCallDeleteTag()
		{
			var gameToUpdate = new Game { GameId = TestGameId };
			var tagsToAttribute = new List<Tag> { new Tag() { TagId = TestSecondTagId } };

			var expectedGameIdentifier = TestGameId;
			var expectedTagIdentifier = TestSecondTagId;

			service.UpdateGameWithTags(gameToUpdate, tagsToAttribute);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.DeleteGameTags(expectedGameIdentifier), Times.Once);
		}

		[Fact]
		public void UpdateGameWithTags_WhenValid_ShouldCallInsertTag()
		{
			var gameToUpdate = new Game { GameId = TestGameId };
			var tagsToAttribute = new List<Tag> { new Tag() { TagId = TestSecondTagId } };

			var expectedGameIdentifier = TestGameId;
			var expectedTagIdentifier = TestSecondTagId;

			service.UpdateGameWithTags(gameToUpdate, tagsToAttribute);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.InsertGameTag(expectedGameIdentifier, expectedTagIdentifier), Times.Once);
		}

		[Fact]
		public void DeleteGame_WhenValid_ShouldCallDeleteGame()
		{
			var expectedGameIdentifier = TestGameId;

			service.DeleteGame(expectedGameIdentifier);
			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.DeleteGame(expectedGameIdentifier), Times.Once);
		}

		[Fact]
		public void GetDeveloperGames_WhenValid_ShouldReturnDeveloperGames()
		{
			gameRepositoryMock.Setup(r => r.GetDeveloperGames(testUser.UserId)).Returns(new List<Game>());

			var foundDeveloperGames = service.GetDeveloperGames();

			Assert.NotNull(foundDeveloperGames);
		}

		[Fact]
		public void GetUnvalidated_WhenValid_ShouldReturnUnvalidatedGames()
		{
			gameRepositoryMock.Setup(r => r.GetUnvalidated(testUser.UserId)).Returns(new List<Game>());

			var unvalidatedGames = service.GetUnvalidated();

			Assert.NotNull(unvalidatedGames);
		}

		[Fact]
		public void RejectGame_WhenValid_ShouldCallRejectGame()
		{
			var expectedGameIdentifier = TestGameId;

			service.RejectGame(expectedGameIdentifier);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.RejectGame(expectedGameIdentifier));
		}

		[Fact]
		public void RejectGameWithMessage_WhenValid_ShouldCallRejectGameWithMessage()
		{
			var expectedGameIdentifier = TestGameId;
			var expectedMessage = "msg";

			service.RejectGameWithMessage(expectedGameIdentifier, expectedMessage);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.RejectGameWithMessage(expectedGameIdentifier, expectedMessage));
		}

		[Fact]
		public void GetRejectionMessage_WhenValid_ShouldCallGetRejectionMesssage()
		{
			var expectedGameIdentifier = TestGameId;
			var expectedMessage = "msg";

			gameRepositoryMock.Setup(r => r.GetRejectionMessage(expectedGameIdentifier)).Returns(expectedMessage);

			var actualMessage = service.GetRejectionMessage(expectedGameIdentifier);

			Assert.Equal(expectedMessage, actualMessage);
		}

		[Fact]
		public void InsertGameTag_WhenValid_ShouldCallRepositoryInsertGameTag()
		{
			var expectedGameIdentifier = TestGameId;
			var expectedTagIdentifier = TestSecondTagId;

			service.InsertGameTag(expectedGameIdentifier, expectedTagIdentifier);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.InsertGameTag(expectedGameIdentifier, expectedTagIdentifier));
		}

		[Fact]
		public void GetAllTags_WhenValid_ShouldReturnAllTags()
		{
			tagRepositoryMock.Setup(r => r.GetAllTags()).Returns(new Collection<Tag>());

			var foundTags = service.GetAllTags();

			Assert.NotNull(foundTags);
		}

		[Fact]
		public void GetGameTags_WhenCalledWithGameIdentifier_ShouldReturnListOfGameTags()
		{
			var expectedGameIdentifier = TestGameId;

			gameRepositoryMock.Setup(r => r.GetGameTags(expectedGameIdentifier)).Returns(new List<Tag>());

			var result = service.GetGameTags(expectedGameIdentifier);

			Assert.NotNull(result);
		}

		[Fact]
		public void IsGameIdInUse_WhenIdInUse_ShouldReturnTrue()
		{
			var expectedGameIdentifier = TestGameId;

			gameRepositoryMock.Setup(r => r.IsGameIdInUse(expectedGameIdentifier)).Returns(true);

			var result = service.IsGameIdInUse(expectedGameIdentifier);

			Assert.True(result);
		}

		[Fact]
		public void DeleteGameTags_WhenValid_ShouldCallRepositoryDeleteGameTags()
		{
			var expectedGameIdentifier = TestGameId;

			service.DeleteGameTags(expectedGameIdentifier);

			gameRepositoryMock.Verify(gameRepositoryMock => gameRepositoryMock.DeleteGameTags(expectedGameIdentifier));
		}

		[Fact]
		public void GetGameOwnerCount_WhenCalledForAGameWithNonZeroOwnerCount_ShouldReturnProperCount()
		{
			var expectedGameIdentifier = TestGameId;
			var expectedGameOwnerCount = 7;

			userGameRepositoryMock.Setup(userGameRepositoryMock => userGameRepositoryMock.GetGameOwnerCount(expectedGameIdentifier)).Returns(expectedGameOwnerCount);

			var result = service.GetGameOwnerCount(expectedGameIdentifier);

			Assert.Equal(expectedGameOwnerCount, result);
		}

		[Fact]
		public void GetCurrentUser_WhenValid_ShouldReturnUser()
		{
			var user = service.GetCurrentUser();

			Assert.Equal(testUser, user);
		}

		[Fact]
		public void CreateValidatedGame_WhenIdInUse_ShouldThrow()
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
			var dicountText = TestGameNoDiscountText;
			var tags = new List<Tag> { new Tag { TagId = TestTagId } };

			var expectedIdentifier = TestGameId;

			gameRepositoryMock.Setup(gameRepositoryMock => gameRepositoryMock.IsGameIdInUse(expectedIdentifier)).Returns(true);

			Assert.Throws<Exception>(() =>
				service.CreateValidatedGame(gameIdText, name, priceText, description, imageUrl, tralerUrl, gameplayUrl, minimumRequirement, recommendedRequirement, dicountText, tags));
		}

		[Fact]
		public void DeleteGame_WhenDeletingValidGame_ShouldRemoveFromCollection()
		{
			var gameList = new ObservableCollection<Game> { new Game() { GameId = TestGameId } };
			var expectedIdentifier = TestGameId;

			service.DeleteGame(expectedIdentifier, gameList);

			Assert.Empty(gameList);
		}

		[Fact]
		public void UpdateGameAndRefreshList_WhenUpdatingValidGame_ShouldUpdateCorrectly()
		{
			var existing = new Game { GameId = TestGameId };
			var updated = new Game { GameId = TestGameId, GameTitle = "Updated" };
			var games = new ObservableCollection<Game> { existing };

			service.UpdateGameAndRefreshList(updated, games);

			Assert.Contains(updated, games);
		}

		[Fact]
		public void RejectGameAndRemoveFromUnvalidated_WhenRemovingGameFromList_ShouldHaveListEmpty()
		{
			var games = new ObservableCollection<Game> { new Game { GameId = TestGameId } };
			var expectedIdentifier = TestGameId;

			service.RejectGameAndRemoveFromUnvalidated(expectedIdentifier, games);

			Assert.Empty(games);
		}

		[Fact]
		public void IsGameIdInUse_WhenGameIdIsNotInUse_ShouldReturnFalse()
		{
			var devGames = new ObservableCollection<Game> { new Game { GameId = TestGameId } };
			var unvalidated = new ObservableCollection<Game> { new Game { GameId = 2 } };

			var expectedThirdIdentifier = 3;

			Assert.False(service.IsGameIdInUse(expectedThirdIdentifier, devGames, unvalidated));
		}

		[Fact]
		public void GetMatchingTagsForGame_WhenHavingTags_ShouldReturnAsManyMatchingTags()
		{
			var allTags = new List<Tag> { new Tag() { TagId = TestTagId }, new Tag() { TagId = TestSecondTagId } };
			var gameTags = new List<Tag> { new Tag() { TagId = TestTagId } };
			var expectedIdentifier = TestGameId;
			var expectedMatchingTagsCount = 1;

			gameRepositoryMock.Setup(gameRepositoryMock => gameRepositoryMock.GetGameTags(expectedIdentifier)).Returns(gameTags);
			var actualMatchingTags = service.GetMatchingTagsForGame(expectedIdentifier, allTags);

			Assert.Equal(expectedMatchingTagsCount, actualMatchingTags.Count);
		}
	}
}
