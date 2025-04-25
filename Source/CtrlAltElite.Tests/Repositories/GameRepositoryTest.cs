using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Tests.TestUtils;

namespace SteamStore.Tests.Repositories;

public class GameRepositoryTest : IDisposable
{
    private Game templateTestGame = new Game()
    {
        GameId = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        GameTitle = "TestName",
        GameDescription = "TestDescription",
        ImagePath = "TestImagePath",
        Price = 1.99m,
        TrailerPath = "TestTrailerPath",
        GameplayPath = "TestGameplayPath",
        MinimumRequirements = "TestMinimumRequirements",
        RecommendedRequirements = "TestRecommendedRequirements",
        Status = PENDING_STATUS,
        Tags = null,
        Rating = 2,
        Discount = 3,
        PublisherIdentifier = 1
    };

    private const string? PENDING_STATUS = "Pending";
    private const string? APPROVED_STATUS = "Approved";
    private const string? REJECTED_STATUS = "Rejected";
    private const string TEST_MESSAGE = "TEST";
    private const decimal UPDATED_GAME_RATING = 0m;
    private const int NONEXISTENT_GAME_IDENTIFIER = -1;
    private const int TEST_UNVALIDATED_USER_ID = 2;
    private readonly GameRepository subject = new GameRepository(DataLinkTestUtils.GetDataLink());

    [Fact]
    public void CreateGame_WhenCalled_IsInsertedInDatabase()
    {
        // Arrange & Act
        var testGame = CreateTestGame();

        // Assert
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.GameTitle == testGame.GameTitle);
        AssertUtils.AssertAllPropertiesEqual(testGame, foundGame);
    }

    [Fact]
    public void UpdateGame_WhenCalled_IsUpdatedInDatabase()
    {
        // Arrange
        var insertedGame = CreateTestGame();
        var updatedGame = new Game()
        {
            GameId = insertedGame.GameId,
            GameTitle = "TestNameUpdated",
            GameDescription = "TestDescriptionUpdated",
            ImagePath = "TestImagePathUpdated",
            Price = 9.99m,
            TrailerPath = "TestTrailerPathUpdated",
            GameplayPath = "TestGameplayPathUpdated",
            MinimumRequirements = "TestMinimumRequirementsUpdated",
            RecommendedRequirements = "TestRecommendedRequirementsUpdated",
            Status = APPROVED_STATUS,
            Rating = 1,
            Discount = 5,
            PublisherIdentifier = 1
        };
        updatedGame.Rating = UPDATED_GAME_RATING;
        updatedGame.GameId = insertedGame.GameId;

        // Act
        subject.UpdateGame(updatedGame.GameId, updatedGame);

        // Assert
        var foundGame = subject.GetDeveloperGames(updatedGame.PublisherIdentifier)
            .FirstOrDefault(game => game.GameTitle == updatedGame.GameTitle);
        AssertUtils.AssertAllPropertiesEqual(updatedGame, foundGame);
    }

    [Fact]
    public void ValidateGame_WhenCalled_StatusIsChangedToApproved()
    {
        // Arrange
        var testGame = CreateTestGame(PENDING_STATUS);

        // Act
        subject.ValidateGame(testGame.GameId);

        // Assert
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.GameTitle == testGame.GameTitle);
        Assert.Equal(APPROVED_STATUS, foundGame!.Status);
    }

    [Fact]
    public void RejectGame_WhenCalled_StatusIsChangedToRejected()
    {
        // Arrange
        var testGame = CreateTestGame(PENDING_STATUS);

        // Act
        subject.RejectGame(testGame.GameId);

        // Assert
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.GameTitle == testGame.GameTitle);
        Assert.Equal(REJECTED_STATUS, foundGame!.Status);
    }

    [Fact]
    public void RejectGame_WhenCalledWithExistingIdentifier_SetsEmptyRejectionMessage()
    {
        // Arrange
        var testGame = CreateTestGame(PENDING_STATUS);

        // Act
        subject.RejectGame(testGame.GameId);

        // Assert
        Assert.Equal(string.Empty, subject.GetRejectionMessage(testGame.GameId));
    }

    [Fact]
    public void GetRejectionMessage_WhenCalledWithNonExistingIdentifier_ReturnsEmptyString()
    {
        // Act
        var rejectionMessage = subject.GetRejectionMessage(NONEXISTENT_GAME_IDENTIFIER);

        // Assert
        Assert.Equal(string.Empty, rejectionMessage);
    }

    [Fact]
    public void RejectGameWithMessage_WhenCalled_StatusIsChangedToRejected()
    {
        // Arrange
        var testGame = CreateTestGame(PENDING_STATUS);

        // Act
        subject.RejectGameWithMessage(testGame.GameId, TEST_MESSAGE);

        // Assert
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.GameTitle == testGame.GameTitle);
        Assert.Equal(REJECTED_STATUS, foundGame!.Status);
    }

    [Fact]
    public void RejectGameWithMessage_WhenCalled_SetsRejectionMessage()
    {
        // Arrange
        var testGame = CreateTestGame(PENDING_STATUS);

        // Act
        subject.RejectGameWithMessage(testGame.GameId, TEST_MESSAGE);

        // Assert
        Assert.Equal(TEST_MESSAGE, subject.GetRejectionMessage(testGame.GameId));
    }

    [Fact]
    public void GetAllGames_WhenCalled_ReturnsAllGames()
    {
        // Arrange
        var testGame = CreateTestGame(APPROVED_STATUS);
        var tags = CreateTagsForGame(testGame);
        testGame.Tags = tags.Select(tag => tag.Tag_name).ToArray();
        testGame.TrendingScore = Game.NOTCOMPUTED;
        testGame.TagScore = Game.NOTCOMPUTED;

        // Act
        var foundGame = subject.GetAllGames().FirstOrDefault(game => game.GameTitle == testGame.GameTitle);

        // Assert
        AssertUtils.AssertAllPropertiesEqual(testGame, foundGame);
    }

    [Fact]
    public void GetUnvalidated_WhenCalled_ReturnsAllPendingStatusGamesBelongingToOtherUsers()
    {
        // Arrange
        var testGame = CreateTestGame(PENDING_STATUS);

        // Act
        var foundGame = subject.GetUnvalidated(TEST_UNVALIDATED_USER_ID)
            .FirstOrDefault(game => game.GameTitle == testGame.GameTitle);

        // Assert
        AssertUtils.AssertAllPropertiesEqual(testGame, foundGame);
    }

    [Fact]
    public void DeleteGame_WhenCalled_DeletesGameFromDatabase()
    {
        // Arrange
        var testGame = CreateTestGame(APPROVED_STATUS);
        CreateTagsForGame(testGame);

        // Act
        subject.DeleteGame(testGame.GameId);

        // Assert
        var notFound = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.GameTitle == testGame.GameTitle);
        Assert.Null(notFound);
    }

    [Fact]
    public void IsGameIdInUse_WhenCalledOnExistingGameIdentifier_ReturnsTrue()
    {
        // Arrange
        var testGame = CreateTestGame();

        // Act
        var isGameIdInUse = subject.IsGameIdInUse(testGame.GameId);

        // Assert
        Assert.True(isGameIdInUse);
    }

    [Fact]
    public void IsGameIdInUse_WhenCalledOnNonExistingGameIdentifier_ReturnsFalse()
    {
        // Act
        var isGameIdInUse = subject.IsGameIdInUse(NONEXISTENT_GAME_IDENTIFIER);

        // Assert
        Assert.False(isGameIdInUse);
    }

    private Tag[] CreateTagsForGame(Game testGame)
    {
        var tags = TagsConstants.ALL_TAGS.OrderBy(tag => tag.Tag_name).ToArray();
        foreach (var tag in tags)
        {
            subject.InsertGameTag(testGame.GameId, tag.TagId);
        }

        return tags;
    }

    private Game CreateTestGame(string? status = null)
    {
        templateTestGame.Rating = UPDATED_GAME_RATING;
        if (status != null)
        {
            templateTestGame.Status = status;
        }

        subject.CreateGame(templateTestGame);
        return templateTestGame;
    }

    public void Dispose()
    {
        subject.DeleteGame(templateTestGame.GameId);
    }
}