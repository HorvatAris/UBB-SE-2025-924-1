using System.Collections.ObjectModel;
using Moq;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services;
using Xunit.Sdk;

namespace SteamStore.Tests.Services;

public class GameServiceTest
{
    private const string TEST_TAG_1 = "tag1";
    private const string TEST_TAG_2 = "tag2";
    private const string TEST_NAME = "Test";
    private const string NOT_MATCH_NAME = "NoMatch";
    private const string TEST_GAME_1 = "test Game 1";
    private const string TEST_GAME_2 = "Game 2";
    private const string TEST_GAME_3 = "TEST Game 3";
    private const string TEST_GAME_4 = "Game4";
    private const string TEST_GAME_5 = "Game5";
    private const string GET_TRADING_GAMES_METHOD_NAME = "getTrendingGames";
    private const string GET_DISCOUNTED_GAMES_METHOD_NAME = "getDiscountedGames";
    private const string? INVALID_METHOD_NAME = "Invalid method name";
    private const int ZERO = 0;
    private const int OVER_CAP_ITEMS_COUNT = 11;
    private const string TEST_TAG_3 = "tag3";
    private const int TWO_EXPECTED_GAMES = 2;
    private const int SECOND_ARRAY_ELEMENT = 1;
    private const int TWO_FOUND_ELEMENTS = 2;
    private const int SINGULAR_FOUND_ELEMENT = 1;
    private const int LOWER_MIN_RATING = 1;
    private const int UPPER_MIN_RATING = 10;
    private const int LOWER_MIN_PRICE = 10;
    private const int UPPER_MIN_PRICE = 400;
    private const int AVERAGE_MIN_PRICE = 100;
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
    private const decimal EXPECTED_TRENDING_SCORE_GAME_2 = 0.5m;
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
    public void GetAllGames_ShouldDelegateToRepo()
    {
        var repoReturnedColl = new Collection<Game>();
        repoMock.Setup(r => r.GetAllGames())
            .Returns(repoReturnedColl);
        Assert.Same(repoReturnedColl, subject.GetAllGames());
    }

    [Fact]
    public void GetAllTags_ShouldDelegateToRepo()
    {
        var repoReturnedColl = new Collection<Tag>();
        tagRepoMock.Setup(r => r.GetAllTags())
            .Returns(repoReturnedColl);
        Assert.Same(repoReturnedColl, subject.GetAllTags());
    }

    [Fact]
    public void GetAllGameTags_ReturnsSameTagInstance()
    {
        var game = new Game { Tags = new[] { TEST_TAG_1, TEST_TAG_2 } };

        var expectedTag = new Tag { Tag_name = TEST_TAG_1 };
        tagRepoMock.Setup(r => r.GetAllTags())
            .Returns(new Collection<Tag> { expectedTag });

        var actualTag = subject.GetAllGameTags(game);
        Assert.Same(actualTag.First(), expectedTag);
    }
    [Fact]
    public void GetAllGameTags_ReturnsExpectedTagName()
    {
        var game = new Game { Tags = new[] { TEST_TAG_1, TEST_TAG_2 } };

        var expectedTag = new Tag { Tag_name = TEST_TAG_1 };
        tagRepoMock.Setup(r => r.GetAllTags())
            .Returns(new Collection<Tag> { expectedTag });

        var actualTag = subject.GetAllGameTags(game);
        Assert.Equal(actualTag.First().Tag_name, expectedTag.Tag_name);
    }
    [Fact]
    public void GetAllGameTags_ReturnsSingularExpectedTag()
    {
        var game = new Game { Tags = new[] { TEST_TAG_1, TEST_TAG_2 } };

        var expectedTag = new Tag { Tag_name = TEST_TAG_1 };
        tagRepoMock.Setup(r => r.GetAllTags())
            .Returns(new Collection<Tag> { expectedTag });

        var actualTag = subject.GetAllGameTags(game);
        Assert.Single(actualTag);
    }

    [Fact]
    public void SearchItems_ShouldMatchTwoItems()
    {
        var game1 = new Game { Name = TEST_GAME_1 };
        var game2 = new Game { Name = TEST_GAME_2 };
        var game3 = new Game { Name = TEST_GAME_3 };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.SearchGames(TEST_NAME);

        Assert.Equal(TWO_EXPECTED_GAMES, actualGames.Count);
    }
    [Fact]
    public void SearchItems_ShouldHaveFirstItemInstance()
    {
        var game1 = new Game { Name = TEST_GAME_1 };
        var game2 = new Game { Name = TEST_GAME_2 };
        var game3 = new Game { Name = TEST_GAME_3 };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.SearchGames(TEST_NAME);

        Assert.Same(actualGames.First(), game1);
    }
    [Fact]
    public void SearchItems_ShouldHaveSecondItemInstance()
    {
        var game1 = new Game { Name = TEST_GAME_1 };
        var game2 = new Game { Name = TEST_GAME_2 };
        var game3 = new Game { Name = TEST_GAME_3 };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.SearchGames(TEST_NAME);

        Assert.Same(actualGames[SECOND_ARRAY_ELEMENT], game3);
    }
    [Fact]
    public void SearchItems_ShouldNoMatchItems()
    {
        var game1 = new Game { Name = TEST_GAME_1 };
        var game2 = new Game { Name = TEST_GAME_2 };
        var game3 = new Game { Name = TEST_GAME_3 };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.SearchGames(NOT_MATCH_NAME);
        Assert.Empty(actualGames);
    }

    [Fact]
    public void FilterItems_ShouldThrowExceptionIfTagIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            subject.FilterGames(LOWER_MIN_RATING, LOWER_MIN_PRICE, LOWER_MAX_PRICE, null));
    }

    [Theory]
    [InlineData(LOWER_MIN_RATING, LOWER_MIN_PRICE, UPPER_MAX_PRICE, TWO_FOUND_ELEMENTS, new string[] { })]
    public void FilterItems_ShouldReturnTwoGames(int minRating, int minPrice, int maxPrice, int foundElems,
        string[] tags)
    {
        var game1 = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };

        var game2 = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2 });

        var actualGames = subject.FilterGames(minRating, minPrice, maxPrice, tags);

        Assert.Equal(foundElems, actualGames.Count);
    }
    [Theory]
    [InlineData(LOWER_MIN_RATING, LOWER_MIN_PRICE, UPPER_MAX_PRICE, new string[] { })]
    public void FilterItems_ShouldReturnFirstGameInstance(int minRating, int minPrice, int maxPrice,
        string[] tags)
    {
        var game1 = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };

        var game2 = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2 });

        var actualGames = subject.FilterGames(minRating, minPrice, maxPrice, tags);

        Assert.Same(actualGames.First(), game1);
    }
    [Theory]
    [InlineData(LOWER_MIN_RATING, LOWER_MIN_PRICE, UPPER_MAX_PRICE, new string[] { })]
    public void FilterItems_ShouldReturnSecondGameInstance(int minRating, int minPrice, int maxPrice,
        string[] tags)
    {
        var game1 = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };

        var game2 = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2 });

        var actualGames = subject.FilterGames(minRating, minPrice, maxPrice, tags);

        Assert.Same(actualGames[SECOND_ARRAY_ELEMENT], game2);
    }
    [Theory]
    [InlineData(LOWER_MIN_RATING, LOWER_MIN_PRICE, AVERAGE_MAX_PRICE, SINGULAR_FOUND_ELEMENT, new[] { TEST_TAG_1 })]
    public void FilterItems_ShouldReturnOneGame(int minRating, int minPrice, int maxPrice, int foundElems, string[] tags)
    {
        var game1 = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };

        var game2 = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2 });

        var actualGames = subject.FilterGames(minRating, minPrice, maxPrice, tags);

        Assert.Equal(foundElems, actualGames.Count);
    }
    [Theory]
    [InlineData(UPPER_MIN_RATING, LOWER_MIN_PRICE, LOWER_MAX_PRICE, ZERO, new[] { TEST_TAG_1, TEST_TAG_2 })]
    [InlineData(LOWER_MIN_RATING, UPPER_MIN_PRICE, UPPER_MAX_PRICE, ZERO, new[] { TEST_TAG_1, TEST_TAG_2 })]
    [InlineData(LOWER_MIN_RATING, AVERAGE_MIN_PRICE, AVERAGE_MAX_PRICE, ZERO, new[] { TEST_TAG_1, TEST_TAG_2 })]
    [InlineData(LOWER_MIN_RATING, AVERAGE_MIN_PRICE, UPPER_MAX_PRICE, ZERO, new[] { TEST_TAG_1, TEST_TAG_2, TEST_TAG_3 })]
    public void FilterItems_ShouldNotReturnGames(int minRating, int minPrice, int maxPrice, int foundElems,
        string[] tags)
    {
        var game1 = new Game()
        {
            Rating = RATING_GAME_1,
            Price = PRICE_GAME_1,
            Tags = new[] { TEST_TAG_1, TEST_TAG_2 }
        };

        var game2 = new Game()
        {
            Rating = RATING_GAME_2,
            Price = PRICE_GAME_2,
            Tags = new[] { TEST_TAG_2 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2 });

        var actualGames = subject.FilterGames(minRating, minPrice, maxPrice, tags);

        Assert.Equal(foundElems, actualGames.Count);
    }

    [Theory]
    [InlineData(GET_TRADING_GAMES_METHOD_NAME)]
    [InlineData(GET_DISCOUNTED_GAMES_METHOD_NAME)]
    public void GetGamesMethod_shouldCapAtTenElems(string methodName)
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

        var actualGames = methodName switch
        {
            GET_TRADING_GAMES_METHOD_NAME => subject.GetTrendingGames(),
            GET_DISCOUNTED_GAMES_METHOD_NAME => subject.GetDiscountedGames(),
            _ => throw new ArgumentException(INVALID_METHOD_NAME, nameof(methodName))
        };

        Assert.Equal(EXPECTED_CAP_NUMBER, actualGames.Count);
    }

    [Fact]
    public void GetDiscountedGames_HasFirstGameInstance()
    {
        var game1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };

        var game2 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_2,
            Discount = DISCOUNT_GAME_2
        };

        var game3 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_3
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.GetDiscountedGames();

        Assert.Same(actualGames.First(), game1);
    }
   [Fact]
    public void GetDiscountedGames_HasSecondGameInstance()
    {
        var game1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };

        var game2 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_2,
            Discount = DISCOUNT_GAME_2
        };

        var game3 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_3
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.GetDiscountedGames();

        Assert.Same(actualGames[SECOND_ARRAY_ELEMENT], game2);
    }
    [Fact]
    public void GetDiscountedGames_HasTwoExpectedElements()
    {
        var game1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };

        var game2 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_2,
            Discount = DISCOUNT_GAME_2
        };

        var game3 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_3
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });

        var actualGames = subject.GetDiscountedGames();
        Assert.Equal(TWO_EXPECTED_GAMES, actualGames.Count);
    }
    [Fact]
    public void GetDiscountedGames_CheckFirstGameComputedTrendingScore()
    {
        var game1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };

        var game2 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_2,
            Discount = DISCOUNT_GAME_2
        };

        var game3 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_3
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });
        subject.GetDiscountedGames();
        Assert.Equal(EXPECTED_TRENDING_SCORE_GAME_1, game1.TrendingScore);
    }
    [Fact]
    public void GetDiscountedGames_CheckSecondGameComputedTrendingScore()
    {
        var game1 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_1,
            Discount = DISCOUNT_GAME_1
        };

        var game2 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_2,
            Discount = DISCOUNT_GAME_2
        };

        var game3 = new Game()
        {
            NumberOfRecentPurchases = NUMBER_OF_RECENT_PURCHASES_GAME_3
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(new Collection<Game> { game1, game2, game3 });
        subject.GetDiscountedGames();

        Assert.Equal(EXPECTED_TRENDING_SCORE_GAME_2, game2.TrendingScore);
    }

    [Fact]
    public void GetSimilarGames_returnsThreeFilteredElements()
    {
        var similarGames = GetSimilarGamesSetUp();
        Assert.Equal(EXPECTED_SIMILAR_GAMES, similarGames.Count);
    }
    [Fact]
    public void GetSimilarGames_SelfExcludedElement()
    {
        var similarGames = GetSimilarGamesSetUp();
        Assert.DoesNotContain(similarGames, g => g.Identifier == IDENTIFIER_1);
    }

    [Fact]
    public void GetSimilarGames_returnsDistinctFilteredElements()
    {
        var similarGames = GetSimilarGamesSetUp();
        var identifiers = similarGames.Select(g => g.Identifier).ToList();
        Assert.True(identifiers.Count == identifiers.Distinct().Count());
    }

    [Fact]
    public void GetSimilarGames_ShouldBeRandomChosen()
    {
        var allGames = new Collection<Game>
        {
            new Game() { Identifier = IDENTIFIER_1, Name = TEST_GAME_1 },
            new Game() { Identifier = IDENTIFIER_2, Name = TEST_GAME_2 },
            new Game() { Identifier = IDENTIFIER_3, Name = TEST_GAME_3 },
            new Game() { Identifier = IDENTIFIER_4, Name = TEST_GAME_4 },
            new Game() { Identifier = IDENTIFIER_5, Name = TEST_GAME_5 }
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
            new Game() { Identifier = IDENTIFIER_1, Name = TEST_GAME_1 },
            new Game() { Identifier = IDENTIFIER_2, Name = TEST_GAME_2 },
            new Game() { Identifier = IDENTIFIER_3, Name = TEST_GAME_3 },
            new Game() { Identifier = IDENTIFIER_4, Name = TEST_GAME_4 },
            new Game() { Identifier = IDENTIFIER_5, Name = TEST_GAME_5 }
        };

        repoMock.Setup(r => r.GetAllGames())
            .Returns(allGames);

        var similarGames = subject.GetSimilarGames(IDENTIFIER_1);
        return similarGames;
    }
}