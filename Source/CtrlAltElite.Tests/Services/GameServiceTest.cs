using System.Collections.ObjectModel;
using Moq;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services;
using SteamStore.Tests.TestUtils;
using Xunit.Sdk;

namespace SteamStore.Tests.Services;

public class GameServiceTest
{
    private const string TEST_TAG_1 = "tag1";
    private const string TEST_TAG_2 = "tag2";
    private const string TEST_NAME = "Test";
    private const string NOT_MATCH_NAME = "NoMatch";
    private const string TEST_GAME_1 = "test Game 1";
    private const string TEST_GAME_2 = "TEST Game 2";
    private const string TEST_GAME_3 = "Game 2";
    private const string TEST_GAME_4 = "Game4";
    private const string TEST_GAME_5 = "Game5";
    private const int ZERO = 0;
    private const int OVER_CAP_ITEMS_COUNT = 11;
    private const int LOWER_MIN_RATING = 1;
    private const int LOWER_MIN_PRICE = 10;
    private const int LOWER_MAX_PRICE = 100;
    private const int UPPER_MAX_PRICE = 400;
    private const int AVERAGE_MAX_PRICE = 101;
    private const decimal RATING_GAME_1 = 5;
    private const decimal PRICE_GAME_1 = 20;
    private const decimal RATING_GAME_2 = 7;
    private const decimal PRICE_GAME_2 = 200;
    private const decimal GAME_DISCOUNT = 1;
    private const int EXPECTED_CAP_NUMBER = 10;
    private const int NUMBER_OF_RECENT_PURCHASES_GAME_1 = 10;
    private const decimal DISCOUNT_GAME_1 = 1;
    private const int NUMBER_OF_RECENT_PURCHASES_GAME_2 = 5;
    private const decimal DISCOUNT_GAME_2 = 2;
    private const int NUMBER_OF_RECENT_PURCHASES_GAME_3 = 5;
    private const decimal EXPECTED_TRENDING_SCORE_GAME_1 = 1;
    private const int EXPECTED_SIMILAR_GAMES = 3;
    private const int IDENTIFIER_1 = 1;
    private const int IDENTIFIER_2 = 2;
    private const int IDENTIFIER_3 = 3;
    private const int IDENTIFIER_4 = 4;
    private const int IDENTIFIER_5 = 5;
    private readonly GameService subject;
    private readonly Mock<IGameRepository> repoMock;
    private readonly Mock<ITagRepository> tagRepoMock;

    public GameServiceTest()
    {
        repoMock = new Mock<IGameRepository>();
        tagRepoMock = new Mock<ITagRepository>();
        subject = new GameService { GameRepository = repoMock.Object, TagRepository = tagRepoMock.Object };
    }

    [Fact]
    public void GetAllGames_WhenCalled_ShouldDelegateToRepositoryTheSameInstance()
    {
        var expectedGames = new Collection<Game>();
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(expectedGames);

        var actualGames = subject.GetAllGames();

        Assert.Same(expectedGames, actualGames);
    }

    [Fact]
    public void GetAllTags_WhenCalled_ShouldDelegateToRepositoryTheSameInstance()
    {
        var expectedGames = new Collection<Tag>();
        tagRepoMock.Setup(gameRepository => gameRepository.GetAllTags())
            .Returns(expectedGames);

        var actualTags = subject.GetAllTags();

        Assert.Same(expectedGames, actualTags);
    }

    [Fact]
    public void GetAllGameTags_WhenCalledWithGame_ReturnsTagsAssociatedWithThatGame()
    {
        var game = new Game { Tags = new[] { TEST_TAG_1, TEST_TAG_2 } };
        var expectedTag = new Tag { Tag_name = TEST_TAG_1 };
        tagRepoMock.Setup(gameRepository => gameRepository.GetAllTags())
            .Returns(new Collection<Tag> { expectedTag });

        var actualTag = subject.GetAllGameTags(game);

        AssertUtils.AssertContainsSingle(actualTag, expectedTag);
    }

    [Fact]
    public void SearchGames_WhenCalledWithSearchQuery_ReturnsOnlyMatchingItems()
    {
        var expectedGame1 = new Game { GameTitle = TEST_GAME_1 };
        var expectedGame2 = new Game { GameTitle = TEST_GAME_2 };
        var excludedGame = new Game { GameTitle = TEST_GAME_3 };
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(new Collection<Game> { expectedGame1, expectedGame2, excludedGame });

        var actualGames = subject.SearchGames(TEST_NAME);

        AssertUtils.AssertContainsExactly(actualGames, new Game[] { expectedGame1, expectedGame2 });
    }

    [Fact]
    public void SearchGames_WhenCalledWithSearchQueryThatDoesNotMatchAnyItem_ShouldReturnEmptyResult()
    {
        var excludedGame1 = new Game { GameTitle = TEST_GAME_1 };
        var excludedGame2 = new Game { GameTitle = TEST_GAME_2 };
        var excludedGame3 = new Game { GameTitle = TEST_GAME_3 };
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(new Collection<Game> { excludedGame1, excludedGame2, excludedGame3 });

        var actualGames = subject.SearchGames(NOT_MATCH_NAME);

        Assert.Empty(actualGames);
    }

    [Fact]
    public void FilterGames_WhenCalledWithNullTags_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            subject.FilterGames(LOWER_MIN_RATING, LOWER_MIN_PRICE, LOWER_MAX_PRICE, null));
    }

    [Fact]
    public void FilterGames_WhenCalledWithEmptyTags_ShouldFilterOnlyByRatingAndPriceRange()
    {
        var expectedGame1 = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };
        var expectedGame2 = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(new Collection<Game> { expectedGame1, expectedGame2 });

        var actualGames = subject.FilterGames(LOWER_MIN_RATING, LOWER_MIN_PRICE, UPPER_MAX_PRICE, new string[] { });

        AssertUtils.AssertContainsExactly(actualGames, new Game[] { expectedGame1, expectedGame2 });
    }

    [Fact]
    public void FilterGames_WhenCalledWithTags_ShouldFilterByRatingPriceRangeAndTags()
    {
        var expectedGame = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };
        var excludedGame = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };
        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { expectedGame, excludedGame });

        var actualGames =
            subject.FilterGames(LOWER_MIN_RATING, LOWER_MIN_PRICE, AVERAGE_MAX_PRICE, new[] { TEST_TAG_1 });

        AssertUtils.AssertContainsExactly(actualGames, new Game[] { expectedGame });
    }

    [Fact]
    public void GetTrendingGames_WhenCalled_ShouldCapAtTenElements()
    {
        var games = new Collection<Game>();
        for (var i = ZERO; i < OVER_CAP_ITEMS_COUNT; i++)
        {
            games.Add(new Game()
            {
                Discount = GAME_DISCOUNT
            });
        }
        repoMock.Setup(r => r.GetAllGames())
            .Returns(games);

        var actualGames = subject.GetTrendingGames();

        Assert.Equal(EXPECTED_CAP_NUMBER, actualGames.Count);
    }

    [Fact]
    public void GetDiscountedGames_WhenCalled_ShouldCapAtTenElements()
    {
        var games = new Collection<Game>();
        for (var i = ZERO; i < OVER_CAP_ITEMS_COUNT; i++)
        {
            games.Add(new Game()
            {
                Discount = GAME_DISCOUNT
            });
        }
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(games);

        var actualGames = subject.GetDiscountedGames();

        Assert.Equal(EXPECTED_CAP_NUMBER, actualGames.Count);
    }

    [Fact]
    public void GetDiscountedGames_WhenCalled_ShouldOnlyReturnGamesWithDiscount()
    {
        var expectedGame1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };
        var expectedGame2 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_2,
            Discount = DISCOUNT_GAME_2
        };
        var excludedGame = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_3
        };
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(new Collection<Game> { expectedGame1, expectedGame2, excludedGame });

        var actualGames = subject.GetDiscountedGames();

        AssertUtils.AssertContainsExactly(actualGames, expectedGame1, expectedGame2);
    }

    [Fact]
    public void GetDiscountedGames_WhenCalled_ShouldComputeTrendingScore()
    {
        var game1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };
        repoMock.Setup(gameRepository => gameRepository.GetAllGames())
            .Returns(new Collection<Game> { game1 });

        subject.GetDiscountedGames();

        Assert.Equal(EXPECTED_TRENDING_SCORE_GAME_1, game1.TrendingScore);
    }

    [Fact]
    public void GetSimilarGames_WhenCalled_ReturnsThreeFilteredElements()
    {
        var similarGames = GetSimilarGamesSetUp();

        Assert.Equal(EXPECTED_SIMILAR_GAMES, similarGames.Count);
    }

    [Fact]
    public void GetSimilarGames_WhenCalled_AssertDoesNotContainSelf()
    {
        var similarGames = GetSimilarGamesSetUp();

        Assert.DoesNotContain(similarGames, game => game.GameId == IDENTIFIER_1);
    }

    [Fact]
    public void GetSimilarGames_WhenCalled_ReturnsDistinctFilteredElements()
    {
        var similarGames = GetSimilarGamesSetUp();

        var identifiers = similarGames.Select(game => game.GameId).ToList();
        Assert.True(identifiers.Count == identifiers.Distinct().Count());
    }

    [Fact]
    public void GetSimilarGames_WhenCalled_ShouldBeRandomlyChosen()
    {
        var allGames = new Collection<Game>
        {
            new Game() { GameId = IDENTIFIER_1, GameTitle = TEST_GAME_1 },
            new Game() { GameId = IDENTIFIER_2, GameTitle = TEST_GAME_2 },
            new Game() { GameId = IDENTIFIER_3, GameTitle = TEST_GAME_3 },
            new Game() { GameId = IDENTIFIER_4, GameTitle = TEST_GAME_4 },
            new Game() { GameId = IDENTIFIER_5, GameTitle = TEST_GAME_5 }
        };
        repoMock.Setup(r => r.GetAllGames())
            .Returns(allGames);

        List<Game> similarGames1;
        List<Game> similarGames2;
        do
        {
            similarGames1 = subject.GetSimilarGames(IDENTIFIER_1);
            similarGames2 = subject.GetSimilarGames(IDENTIFIER_1);
        }
        while (similarGames1.SequenceEqual(similarGames2));

        Assert.NotEqual(similarGames1, similarGames2);
    }

    private List<Game> GetSimilarGamesSetUp()
    {
        var allGames = new Collection<Game>
        {
            new Game() { GameId = IDENTIFIER_1, GameTitle = TEST_GAME_1 },
            new Game() { GameId = IDENTIFIER_2, GameTitle = TEST_GAME_2 },
            new Game() { GameId = IDENTIFIER_3, GameTitle = TEST_GAME_3 },
            new Game() { GameId = IDENTIFIER_4, GameTitle = TEST_GAME_4 },
            new Game() { GameId = IDENTIFIER_5, GameTitle = TEST_GAME_5 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(allGames);

        var similarGames = subject.GetSimilarGames(IDENTIFIER_1);
        return similarGames;
    }
}