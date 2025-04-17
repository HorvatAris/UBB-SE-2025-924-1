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
		private readonly Mock<IUserGameRepository> userGameRepoMock = new Mock<IUserGameRepository>();
        private const int TestGameId = 1;
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

        private readonly User testUser = new User() { UserIdentifier = 42 };

		public DeveloperServiceTests()
		{
			service = new DeveloperService
			{
				GameRepository = gameRepositoryMock.Object,
				TagRepository = tagRepositoryMock.Object,
				UserGameRepository = userGameRepoMock.Object,
				User = testUser
			};
		}

		[Fact]
		public void ValidateGame_ShouldCallRepository()
		{
			service.ValidateGame(TestGameId);
			gameRepositoryMock.Verify(repo => repo.ValidateGame(TestGameId), Times.Once);
		}

		[Fact]
		public void ValidateInputForAddingAGame_ShouldReturnGame_WhenValid()
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
			var tags = new List<Tag> { new Tag { TagId = 1 } };

			var expectedIdentifier = 1;
			var expectedName = "Test";
			var expectedPrice = 10;
			var expectedStatus = "Pending";

			var game = service.ValidateInputForAddingAGame(gameIdText, name, priceText, description, imageUrl, tralerUrl, gameplayUrl, minimumRequirement, recommendedRequirement, dicountText, tags);

			Assert.NotNull(game);
			Assert.Equal(expectedIdentifier, game.Identifier);
			Assert.Equal(expectedName, game.Name);
			Assert.Equal(expectedPrice, game.Price);
			Assert.Equal(expectedStatus, game.Status);
		}

		[Theory]
		[InlineData("", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("1", "", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("abc", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("1", "name", "-10", "desc", "img", "trailer", "gameplay", "min", "rec", "5")]
		[InlineData("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "abc")]
		[InlineData("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "150")]
		public void ValidateInputForAddingAGame_ShouldThrow_WhenInvalid(
			string id, string name, string price, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string recommendedRequirement, string discount)
		{
			Assert.Throws<Exception>(() =>
				service.ValidateInputForAddingAGame(id, name, price, description, imageUrl, trailerUrl, gameplayUrl, minimumRequirement, recommendedRequirement, discount, new List<Tag> { new Tag() }));
		}

		[Fact]
		public void FindGameInObservableCollectionById_ShouldReturnGame()
		{
			var list = new ObservableCollection<Game> { new Game { Identifier = 1 } };

			var searchedIdentifier = 1;

			var result = service.FindGameInObservableCollectionById(searchedIdentifier, list);

			Assert.NotNull(result);
		}

		[Fact]
		public void CreateGame_ShouldCallRepositoryWithUserId()
		{
			var game = new Game { Identifier = 1 };

			service.CreateGame(game);

			Assert.Equal(testUser.UserIdentifier, game.PublisherIdentifier);
			gameRepositoryMock.Verify(r => r.CreateGame(game), Times.Once);
		}

		[Fact]
		public void CreateGameWithTags_ShouldCallCreateAndInsert()
		{
			var game = new Game { Identifier = 1 };
			var tags = new List<Tag> { new Tag() { TagId = 2 } };

			var expectedGameIdentifier = 1;
			var expectedTagId = 2;

			service.CreateGameWithTags(game, tags);

			gameRepositoryMock.Verify(r => r.CreateGame(game), Times.Once);
			gameRepositoryMock.Verify(r => r.InsertGameTag(expectedGameIdentifier, expectedTagId), Times.Once);
		}

		[Fact]
		public void UpdateGame_ShouldCallUpdateWithUserId()
		{
			var game = new Game { Identifier = 1 };

			var expectedGameIdentifier = 1;

			service.UpdateGame(game);
			gameRepositoryMock.Verify(r => r.UpdateGame(expectedGameIdentifier, game), Times.Once);
		}

		[Fact]
		public void UpdateGameWithTags_ShouldCallUpdateAndDeleteTagsAndInsert()
		{
			var game = new Game { Identifier = 1 };
			var tags = new List<Tag> { new Tag() { TagId = 2 } };

			var expectedGameIdentifier = 1;
			var expectedTagIdentifier = 2;

			service.UpdateGameWithTags(game, tags);

			gameRepositoryMock.Verify(r => r.UpdateGame(expectedGameIdentifier, game), Times.Once);
			gameRepositoryMock.Verify(r => r.DeleteGameTags(expectedGameIdentifier), Times.Once);
			gameRepositoryMock.Verify(r => r.InsertGameTag(expectedGameIdentifier, expectedTagIdentifier), Times.Once);
		}

		[Fact]
		public void DeleteGame_ShouldCallRepo()
		{
			var expectedGameIdentifier = 1;

			service.DeleteGame(expectedGameIdentifier);
			gameRepositoryMock.Verify(r => r.DeleteGame(expectedGameIdentifier), Times.Once);
		}

		[Fact]
		public void GetDeveloperGames_ShouldReturnGames()
		{
			gameRepositoryMock.Setup(r => r.GetDeveloperGames(testUser.UserIdentifier)).Returns(new List<Game>());

			var result = service.GetDeveloperGames();

			Assert.NotNull(result);
		}

		[Fact]
		public void GetUnvalidated_ShouldReturnList()
		{
			gameRepositoryMock.Setup(r => r.GetUnvalidated(testUser.UserIdentifier)).Returns(new List<Game>());

			var result = service.GetUnvalidated();

			Assert.NotNull(result);
		}

		[Fact]
		public void RejectGame_ShouldCallRepo()
		{
			var expectedGameIdentifier = 1;

			service.RejectGame(expectedGameIdentifier);

			gameRepositoryMock.Verify(r => r.RejectGame(expectedGameIdentifier));
		}

		[Fact]
		public void RejectGameWithMessage_ShouldCallRepo()
		{
			var expectedGameIdentifier = 1;
			var expectedMessage = "msg";

			service.RejectGameWithMessage(expectedGameIdentifier, expectedMessage);

			gameRepositoryMock.Verify(r => r.RejectGameWithMessage(expectedGameIdentifier, expectedMessage));
		}

		[Fact]
		public void GetRejectionMessage_ShouldCallRepo()
		{
			var expectedGameIdentifier = 1;
			var expectedMessage = "msg";

			gameRepositoryMock.Setup(r => r.GetRejectionMessage(expectedGameIdentifier)).Returns(expectedMessage);

			var actualMessage = service.GetRejectionMessage(expectedGameIdentifier);

			Assert.Equal(expectedMessage, actualMessage);
		}

		[Fact]
		public void InsertGameTag_ShouldCallRepo()
		{
			var expectedGameIdentifier = 1;
			var expectedTagIdentifier = 2;

			service.InsertGameTag(expectedGameIdentifier, expectedTagIdentifier);

			gameRepositoryMock.Verify(r => r.InsertGameTag(expectedGameIdentifier, expectedTagIdentifier));
		}

		[Fact]
		public void GetAllTags_ShouldReturnTags()
		{
			tagRepositoryMock.Setup(r => r.GetAllTags()).Returns(new Collection<Tag>());

			var tags = service.GetAllTags();

			Assert.NotNull(tags);
		}

		[Fact]
		public void IsGameIdInUse_ShouldReturnTrue()
		{
			var expectedGameIdentifier = 1;

			gameRepositoryMock.Setup(r => r.IsGameIdInUse(expectedGameIdentifier)).Returns(true);

			var result = service.IsGameIdInUse(expectedGameIdentifier);

			Assert.True(result);
		}

		[Fact]
		public void GetGameTags_ShouldReturnList()
		{
			var expectedGameIdentifier = 1;

			gameRepositoryMock.Setup(r => r.GetGameTags(expectedGameIdentifier)).Returns(new List<Tag>());

			var result = service.GetGameTags(expectedGameIdentifier);

			Assert.NotNull(result);
		}

		[Fact]
		public void DeleteGameTags_ShouldCallRepo()
		{
			var expectedGameIdentifier = 1;

			service.DeleteGameTags(expectedGameIdentifier);

			gameRepositoryMock.Verify(r => r.DeleteGameTags(expectedGameIdentifier));
		}

		[Fact]
		public void GetGameOwnerCount_ShouldReturnCount()
		{
			var expectedGameIdentifier = 1;
			var expectedGameOwnerCount = 7;

			userGameRepoMock.Setup(r => r.GetGameOwnerCount(expectedGameIdentifier)).Returns(expectedGameOwnerCount);

			var result = service.GetGameOwnerCount(expectedGameIdentifier);

			Assert.Equal(expectedGameOwnerCount, result);
		}

		[Fact]
		public void GetCurrentUser_ShouldReturnUser()
		{
			var user = service.GetCurrentUser();

			Assert.Equal(testUser, user);
		}

		[Fact]
		public void CreateValidatedGame_ShouldThrowIfIdInUse()
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
			var tags = new List<Tag> { new Tag { TagId = 1 } };

			var expectedIdentifier = 1;

			gameRepositoryMock.Setup(r => r.IsGameIdInUse(expectedIdentifier)).Returns(true);
			Assert.Throws<Exception>(() =>
				service.CreateValidatedGame(gameIdText, name, priceText, description, imageUrl, tralerUrl, gameplayUrl, minimumRequirement, recommendedRequirement, dicountText, tags));
		}

		[Fact]
		public void DeleteGame_ShouldRemoveFromCollection()
		{
			var gameList = new ObservableCollection<Game> { new Game() { Identifier = 1 } };
			var expectedIdentifier = 1;

			service.DeleteGame(expectedIdentifier, gameList);

			Assert.Empty(gameList);
		}

		[Fact]
		public void UpdateGameAndRefreshList_ShouldUpdateCorrectly()
		{
			var existing = new Game { Identifier = 1 };
			var updated = new Game { Identifier = 1, Name = "Updated" };
			var games = new ObservableCollection<Game> { existing };

			service.UpdateGameAndRefreshList(updated, games);

			Assert.Single(games);
			Assert.Contains(updated, games);
		}

		[Fact]
		public void RejectGameAndRemoveFromUnvalidated_ShouldWork()
		{
			var games = new ObservableCollection<Game> { new Game { Identifier = 1 } };
			var expectedIdentifier = 1;

			service.RejectGameAndRemoveFromUnvalidated(expectedIdentifier, games);

			Assert.Empty(games);
		}

		[Fact]
		public void IsGameIdInUse_WithCollections_ShouldShortCircuit()
		{
			var devGames = new ObservableCollection<Game> { new Game { Identifier = 1 } };
			var unvalidated = new ObservableCollection<Game> { new Game { Identifier = 2 } };

			var expectedFirstIdentifier = 1;
			var expectedSecondIdentifier = 2;
			var expectedThirdIdentifier = 3;

			Assert.True(service.IsGameIdInUse(expectedFirstIdentifier, devGames, unvalidated));
			Assert.True(service.IsGameIdInUse(expectedSecondIdentifier, devGames, unvalidated));
			Assert.False(service.IsGameIdInUse(expectedThirdIdentifier, devGames, unvalidated));
		}

		[Fact]
		public void GetMatchingTagsForGame_ShouldReturnMatched()
		{
			var allTags = new List<Tag> { new Tag() { TagId = 1 }, new Tag() { TagId = 2 } };
			var gameTags = new List<Tag> { new Tag() { TagId = 1 } };
			var expectedIdentifier = 1;
			var expectedTagIdentifier = 1;
			var firstIndexOfResult = 0;

			gameRepositoryMock.Setup(r => r.GetGameTags(expectedIdentifier)).Returns(gameTags);
			var result = service.GetMatchingTagsForGame(expectedIdentifier, allTags);

			Assert.Single(result);
			Assert.Equal(expectedTagIdentifier, result[firstIndexOfResult].TagId);
		}
	}
}
