using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Tests.TestUtils;

namespace SteamStore.Tests.Repositories;

public class GameRepositoryTest
{
    private const string? PENDING_STATUS = "Pending";
    private const string? APPROVED_STATUS = "Approved";
    private const string? REJECTED_STATUS = "Rejected";
    private const string TEST_MESSAGE = "TEST";
    private const decimal UPDATED_GAME_RATING = 0m;
    private const int NONEXISTENT_GAME_ID = -1;
    private const int TEST_UNVALIDATED_USER_ID = 2;
    private readonly GameRepository subject = new GameRepository(DataLinkTestUtils.GetDataLink());

    [Fact]
    public void CreateGame()
    {
        var testGame = CreateRandomGame();
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.Name == testGame.Name);
        AssertUtils.AssertAllPropertiesEqual(testGame, foundGame);
    }

    [Fact]
    public void UpdateGame()
    {
        var insertedGame = CreateRandomGame();
        var updatedGame = GameTestUtils.CreateRandomGame();
        updatedGame.Rating = UPDATED_GAME_RATING;
        updatedGame.Identifier = insertedGame.Identifier;
        subject.UpdateGame(updatedGame.Identifier, updatedGame);
        var foundGame = subject.GetDeveloperGames(updatedGame.PublisherIdentifier)
            .FirstOrDefault(game => game.Name == updatedGame.Name);
        AssertUtils.AssertAllPropertiesEqual(updatedGame, foundGame);
    }

    [Fact]
    public void ValidateGame()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);
        subject.ValidateGame(testGame.Identifier);
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.Name == testGame.Name);

        Assert.Equal(APPROVED_STATUS, foundGame!.Status);
    }

    [Fact]
    public void RejectGame_ReturnsRejectedStatus()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);
        subject.RejectGame(testGame.Identifier);
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.Name == testGame.Name);

        Assert.Equal(REJECTED_STATUS, foundGame!.Status);
    }
    [Fact]
    public void RejectGame_GetRejectionMessage()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);
        subject.RejectGame(testGame.Identifier);
        Assert.Equal(string.Empty, subject.GetRejectionMessage(testGame.Identifier));
    }
    [Fact]
    public void RejectGame_GetNullRejectionMessage()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);

        subject.RejectGame(testGame.Identifier);

        Assert.Equal(string.Empty, subject.GetRejectionMessage(NONEXISTENT_GAME_ID));
    }

    [Fact]
    public void RejectGameWithStatus()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);
        subject.RejectGameWithMessage(testGame.Identifier, TEST_MESSAGE);
        var foundGame = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.Name == testGame.Name);

        Assert.Equal(REJECTED_STATUS, foundGame!.Status);
    }
    [Fact]
    public void RejectGameWithMessage()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);
        subject.RejectGameWithMessage(testGame.Identifier, TEST_MESSAGE);
        Assert.Equal(TEST_MESSAGE, subject.GetRejectionMessage(testGame.Identifier));
    }

    [Fact]
    public void GetAllGames()
    {
        var testGame = CreateRandomGame(APPROVED_STATUS);
        var tags = CreateRandomTagsForGame(testGame);

        var foundGame = subject.GetAllGames().FirstOrDefault(game => game.Name == testGame.Name);
        testGame.Tags = tags.Select(tag => tag.Tag_name).ToArray();
        testGame.TrendingScore = Game.NOTCOMPUTED;
        testGame.TagScore = Game.NOTCOMPUTED;
        AssertUtils.AssertAllPropertiesEqual(testGame, foundGame);
    }

    [Fact]
    public void GetUnvalidated()
    {
        var testGame = CreateRandomGame(PENDING_STATUS);
        var foundGame = subject.GetUnvalidated(TEST_UNVALIDATED_USER_ID).FirstOrDefault(game => game.Name == testGame.Name);

        AssertUtils.AssertAllPropertiesEqual(testGame, foundGame);
    }

    [Fact]
    public void DeleteGame()
    {
        var testGame = CreateRandomGame(APPROVED_STATUS);
        CreateRandomTagsForGame(testGame);
        subject.DeleteGame(testGame.Identifier);
        var notFound = subject.GetDeveloperGames(testGame.PublisherIdentifier)
            .FirstOrDefault(game => game.Name == testGame.Name);

        Assert.Null(notFound);
    }

    [Fact]
    public void IsGameIdUsed()
    {
        var testGame = CreateRandomGame();
        Assert.True(subject.IsGameIdInUse(testGame.Identifier));
    }
    [Fact]
    public void IsGameIdNotUsed()
    {
        Assert.False(subject.IsGameIdInUse(NONEXISTENT_GAME_ID));
    }

    private Tag[] CreateRandomTagsForGame(Game testGame)
    {
        var tags = GameTestUtils.RandomTags();
        foreach (var tag in tags)
        {
            subject.InsertGameTag(testGame.Identifier, tag.TagId);
        }

        return tags;
    }

    private Game CreateRandomGame(string? status = null)
    {
        var testGame = GameTestUtils.CreateRandomGame();
        testGame.Rating = UPDATED_GAME_RATING;
        if (status != null)
        {
            testGame.Status = status;
        }

        subject.CreateGame(testGame);
        return testGame;
    }
}