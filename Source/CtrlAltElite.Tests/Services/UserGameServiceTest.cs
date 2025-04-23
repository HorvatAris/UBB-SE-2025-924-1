using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SteamStore.Constants;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;
using Windows.UI.WebUI;

namespace SteamStore.Tests.Services
{
    public class UserGameServiceTest
    {
        private readonly Mock<IUserGameRepository> mockUserGameRepository;
        private readonly Mock<IGameRepository> mockGameRepository;
        private readonly Mock<ITagRepository> mockTagRepository;
        private readonly UserGameService userGameService;

        private const string FirstGame = "FirstGame";
        private const string SecondGame = "SecondGame";
        private const string FirstTagName = "RPG";
        private const string SecondTagName = "FPS";
        private const string ThirdTagName = "MMG";
        private const string FourthTagName = "BLP";
        private const int UserPointsBeforePurchase = 10;
        private const int UserPointsAfterPurchase = 15;

        private const int NumberOfUsersGamesWithTagUsedOnce = 1;
        private const int NumberOfFavoriteTagsMaximum = 3;
        private const decimal TagScoreMinimum = 0.0m;

        private const decimal RatingHigh = 4.7m;
        private const decimal RatingMedium = 4.3m;
        private const decimal RatingLow = 2.5m;
        private const decimal RatingPoor = 1.5m;
        private const decimal DiscountHigh = 50;
        private const decimal DiscountLow = 10;
        private const int PriceHigh = 20;
        private const int PriceLow = 10;
        private const int RecentPurchasesFirstGame = 5;
        private const int RecentPurchasesSecondGame = 10;

        public UserGameServiceTest()
        {
            mockUserGameRepository = new Mock<IUserGameRepository>();
            mockGameRepository = new Mock<IGameRepository>();
            mockTagRepository = new Mock<ITagRepository>();

            userGameService = new UserGameService
            {
                UserGameRepository = mockUserGameRepository.Object,
                GameRepository = mockGameRepository.Object,
                TagRepository = mockTagRepository.Object
            };
        }

        [Fact]
        public void RemoveGameFromWishlist_WhenGameIsRemovedSuccessfully_CallsRepository()
        {
            var gameToRemove = new Game();

            userGameService.RemoveGameFromWishlist(gameToRemove);

            mockUserGameRepository.Verify(repository => repository.RemoveGameFromWishlist(gameToRemove), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_WhenGameIsAlreadyOwned_ThrowsException()
        {
            var gameToAdd = new Game { Name = FirstGame };
            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(gameToAdd)).Returns(true);

            var exceptionAddGameToWishList = Assert.Throws<Exception>(() => userGameService.AddGameToWishlist(gameToAdd));

            Assert.Equal(string.Format(ExceptionMessages.GameAlreadyOwned, FirstGame), exceptionAddGameToWishList.Message);
        }

        [Fact]
        public void AddGameToWishlist_WhenGameIsNotOwned_CallsRepository()
        {
            var gameToAdd = new Game { Name = SecondGame };

            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(gameToAdd)).Returns(false);

            userGameService.AddGameToWishlist(gameToAdd);
            mockUserGameRepository.Verify(repository => repository.AddGameToWishlist(gameToAdd), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_WhenSqlException_ThrowsException()
        {
            const string ThirdGame = "ThirdGame";
            var gameToAdd = new Game { Name = ThirdGame };

            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(gameToAdd)).Returns(false);
            mockUserGameRepository.Setup(repository => repository.AddGameToWishlist(gameToAdd)).Throws(new Exception(ErrorStrings.SQLNONQUERYFAILUREINDICATOR));

            var exception = Assert.Throws<Exception>(() => userGameService.AddGameToWishlist(gameToAdd));
            Assert.Equal(string.Format(ExceptionMessages.GameAlreadyInWishlist, ThirdGame), exception.Message);
        }

        [Fact]
        public void PurchaseGames_WhenGameIsPurchasedSuccesful_CallsRepository()
        {
            var gameToPurchase = new Game { Name = FirstGame };
            var gamesToPurchase = new List<Game> { gameToPurchase };

            mockUserGameRepository.SetupSequence(repository => repository.GetUserPointsBalance())
                                  .Returns(UserPointsBeforePurchase)
                                  .Returns(UserPointsAfterPurchase);

            userGameService.PurchaseGames(gamesToPurchase);

            mockUserGameRepository.Verify(repository => repository.AddGameToPurchased(gameToPurchase), Times.Once);
            mockUserGameRepository.Verify(repository => repository.RemoveGameFromWishlist(gameToPurchase), Times.Once);
        }

        [Fact]
        public void PurchaseGames_WhenGameIsPurchasedSuccesfull_CalculatesPointsCorrectly()
        {
            var gameToPurchase = new Game { Name = FirstGame };
            var gamesToPurchase = new List<Game> { gameToPurchase };

            mockUserGameRepository.SetupSequence(repository => repository.GetUserPointsBalance())
                                  .Returns(UserPointsBeforePurchase)
                                  .Returns(UserPointsAfterPurchase);

            userGameService.PurchaseGames(gamesToPurchase);

            Assert.Equal(UserPointsAfterPurchase - UserPointsBeforePurchase, userGameService.LastEarnedPoints);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_WhenTagIsUsedInOneGame_CountIsSetToOne()
        {
            var tagUsed = new Tag { Tag_name = FirstTagName };
            var gameWithTag = new Game { Name = FirstGame, Tags = new[] { FirstTagName } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { gameWithTag });

            var tagCollection = new Collection<Tag> { tagUsed };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagUsedOnce, tagUsed.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTago_WhenTagNotPresentInAnyGame_CountIsSetToZero()
        {
            const int NumberOfUsersGamesWithTagNeverUsed = 0;

            var tagNotUsed = new Tag { Tag_name = SecondTagName };
            var gameWithoutTag = new Game { Name = FirstGame, Tags = new[] { FirstTagName } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { gameWithoutTag });

            var tagCollection = new Collection<Tag> { tagNotUsed };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagNeverUsed, tagNotUsed.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_WhenMultipleGamesForSameTag_CountIsSetCorrectly()
        {
            const int NumberOfUsersGamesWithTagUsedMultiple = 2;

            var tagUsed = new Tag { Tag_name = FirstTagName };
            var gameUsingTag1 = new Game { Name = FirstGame, Tags = new[] { FirstTagName } };
            var gameUsingTag2 = new Game { Name = SecondGame, Tags = new[] { FirstTagName, SecondTagName } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { gameUsingTag1, gameUsingTag2 });

            var tagCollection = new Collection<Tag> { tagUsed };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagUsedMultiple, tagUsed.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_HandleMultipleTagsAndGames_CountIsSetCorrectly()
        {
            var tagUsed1 = new Tag { Tag_name = FirstTagName };
            var tagUsed2 = new Tag { Tag_name = SecondTagName };

            var multipleTagGame = new Game { Name = FirstGame, Tags = new[] { FirstTagName, SecondTagName } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { multipleTagGame });

            var tagCollection = new Collection<Tag> { tagUsed1, tagUsed2 };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagUsedOnce, tagUsed1.NumberOfUserGamesWithTag);
            Assert.Equal(NumberOfUsersGamesWithTagUsedOnce, tagUsed2.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenCalled_ReturnsTop3Tags()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName },
                new Tag { Tag_name = ThirdTagName }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { SecondTagName, ThirdTagName } },
                new Game { Tags = new[] { FirstTagName, SecondTagName } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(NumberOfFavoriteTagsMaximum, favoriteTags.Count);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenCalled_ReturnsTop3TagsCorrectly()
        {
            const int First = 0;
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName },
                new Tag { Tag_name = ThirdTagName }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { SecondTagName, ThirdTagName } },
                new Game { Tags = new[] { FirstTagName, SecondTagName } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(SecondTagName, favoriteTags[First].Tag_name);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenNoGamesAreFound_ReturnsEmpty()
        {
            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(new Collection<Tag>());
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game>());

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Empty(favoriteTags);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenCountIsCorrect_ReturnsOnlyExistingTags()
        {
            const int NumberOfFavoriteTagsExpected = 2;
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { FirstTagName } },
                new Game { Tags = new[] { FirstTagName } },
                new Game { Tags = new[] { SecondTagName } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(NumberOfFavoriteTagsExpected, favoriteTags.Count);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenTagIsCorrect_ReturnsOnlyExistingTags()
        {
            const int First = 0;
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { FirstTagName } },
                new Game { Tags = new[] { FirstTagName } },
                new Game { Tags = new[] { SecondTagName } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(FirstTagName, favoriteTags[First].Tag_name);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenCalled_IgnoreTagsNotInList()
        {
            const int First = 0;
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { FirstTagName, SecondTagName, ThirdTagName } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Single(favoriteTags);
            Assert.Equal(FirstTagName, favoriteTags[First].Tag_name);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenMoreTags_ReturnsTop3Tags()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName },
                new Tag { Tag_name = ThirdTagName },
                new Tag { Tag_name = FourthTagName }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { FirstTagName, SecondTagName, ThirdTagName } },
                new Game { Tags = new[] { SecondTagName, FourthTagName } },
                new Game { Tags = new[] { FourthTagName } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(NumberOfFavoriteTagsMaximum, favoriteTags.Count);
        }

        [Fact]
        public void ComputeTagScoreForGames_WhenCalled_CalculatesProperly()
        {
            const decimal TagScoreExpected = 1m;
            const decimal TagScoreComparator = 0.0001m;
            var gameToScore = new Game { Tags = new[] { FirstTagName, SecondTagName } };

            var allUserGames = new Collection<Game>
            {
                gameToScore,
                new Game { Tags = new[] { SecondTagName } },
                new Game { Tags = new[] { ThirdTagName } }
            };

            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName },
                new Tag { Tag_name = ThirdTagName }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(allUserGames);

            userGameService.ComputeTagScoreForGames(allUserGames);

            Assert.True(Math.Abs(TagScoreExpected - gameToScore.TagScore) < TagScoreComparator);
        }

        [Fact]
        public void ComputeTagScoreForGames_WhenNoMatchingTags_SetsZero()
        {
            var gameWithoutValidTags = new Game { Tags = new[] { FourthTagName } };

            var allUserGames = new Collection<Game> { gameWithoutValidTags };

            var knownTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName },
                new Tag { Tag_name = ThirdTagName }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(knownTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(allUserGames);

            userGameService.ComputeTagScoreForGames(allUserGames);

            Assert.Equal(TagScoreMinimum, gameWithoutValidTags.TagScore);
        }

        [Fact]
        public void ComputeTagScoreForGames_WhenUnevenTagCounts_CalculatesCorrectly()
        {
            var gameToEvaluate = new Game { Tags = new[] { SecondTagName } };

            var allUserGames = new Collection<Game>
            {
                gameToEvaluate,
                new Game { Tags = new[] { SecondTagName } },
                new Game { Tags = new[] { SecondTagName } },
                new Game { Tags = new[] { FirstTagName } }
            };

            var knownTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName },
                new Tag { Tag_name = SecondTagName }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(knownTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(allUserGames);

            userGameService.ComputeTagScoreForGames(allUserGames);

            Assert.True(gameToEvaluate.TagScore > TagScoreMinimum);
        }

        [Fact]
        public void ComputeTrendingScores_WhenCalled_SetsTrendingScore()
        {
            const int First = 0;
            const int Second = 1;
            const decimal TrendingScoreExpected1 = 0.5m;
            const decimal TrendingScoreExpected2 = 1.0m;
            var trendingGames = new Collection<Game>
            {
                new Game { Name = FirstGame, NumberOfRecentPurchases = RecentPurchasesFirstGame },
                new Game { Name = SecondGame, NumberOfRecentPurchases = RecentPurchasesSecondGame }
            };

            userGameService.ComputeTrendingScores(trendingGames);

            Assert.Equal(TrendingScoreExpected1, trendingGames[First].TrendingScore);
            Assert.Equal(TrendingScoreExpected2, trendingGames[Second].TrendingScore);
        }

        [Fact]
        public void ComputeTrendingScores_WhenCalled_HandlesSingleGame()
        {
            const int First = 0;
            const decimal TrendingScoreExpected = 1.0m;
            var singleGame = new Collection<Game>
            {
                new Game { Name = FirstGame, NumberOfRecentPurchases = RecentPurchasesFirstGame }
            };

            userGameService.ComputeTrendingScores(singleGame);

            Assert.Equal(TrendingScoreExpected, singleGame[First].TrendingScore);
        }

        [Fact]
        public void GetRecommendedGames_WhenNoMoreThan10Gamesd_ReturnsTopGames()
        {
            const int NumberRecommendedGamesExpected = 2;
            var recommendedGames = new Collection<Game>
            {
                new Game { NumberOfRecentPurchases = RecentPurchasesFirstGame, Tags = new[] { FirstTagName } },
                new Game { NumberOfRecentPurchases = RecentPurchasesSecondGame, Tags = new[] { FirstTagName } }
            };

            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName, NumberOfUserGamesWithTag = 2 }
            };

            mockGameRepository.Setup(repository => repository.GetAllGames()).Returns(recommendedGames);
            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(recommendedGames);

            var recommendedResult = userGameService.GetRecommendedGames();

            Assert.Equal(NumberRecommendedGamesExpected, recommendedResult.Count);
        }

        [Fact]
        public void GetRecommendedGames_WhenMoreThan10Games_ReturnsTop10()
        {
            const int NumberGenerateGamesNumberMinimum = 1;
            const int NumberGenerateGamesNumberMaximum = 15;
            const int NumberRecommendedGamesMaximum = 10;
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = FirstTagName, NumberOfUserGamesWithTag = NumberGenerateGamesNumberMaximum }
            };

            var allGames = Enumerable.Range(NumberGenerateGamesNumberMinimum, NumberGenerateGamesNumberMaximum).Select(gameIndex => new Game
            {
                Name = $"Game{gameIndex}",
                NumberOfRecentPurchases = gameIndex,
                Tags = new[] { FirstTagName }
            }).ToList();

            mockGameRepository.Setup(repository => repository.GetAllGames()).Returns(new Collection<Game>(allGames));
            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game>(allGames));

            var recommendedGames = userGameService.GetRecommendedGames();

            Assert.Equal(NumberRecommendedGamesMaximum, recommendedGames.Count);
        }

        [Fact]
        public void SearchWishListByName_WhenSpecificWordEntered_ReturnsMatches()
        {
            const int First = 0;
            var wishlistGames = new Collection<Game>
            {
                new Game { Name = FirstGame },
                new Game { Name = SecondGame }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(wishlistGames);

            var matchedGames = userGameService.SearchWishListByName(FirstGame);
            Assert.Single(matchedGames);
            Assert.Equal(FirstGame, matchedGames[First].Name);
        }

        [Fact]
        public void SearchWishListByName_WhenAllMatch_ReturnsAllWords()
        {
            const string SearchWishListString = "Game";
            var wishlistGames = new Collection<Game>
            {
                new Game { Name = FirstGame },
                new Game { Name = SecondGame }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(wishlistGames);

            var matchedGames = userGameService.SearchWishListByName(SearchWishListString);
            Assert.Equal(wishlistGames, matchedGames);
        }

        [Fact]
        public void FilterWishListGames_WhenFilterByOverwhelminglyPositive_ReturnsFiltered()
        {
            const int First = 0;
            var gamesList = new Collection<Game>
            {
                new Game { Rating = RatingHigh },
                new Game { Rating = RatingMedium },
                new Game { Rating = RatingLow },
                new Game { Rating = RatingPoor }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var filteredGames = userGameService.FilterWishListGames(FilterCriteria.OVERWHELMINGLYPOSITIVE);
            Assert.Single(filteredGames);
            Assert.Equal(RatingHigh, filteredGames[First].Rating);
        }

        [Fact]
        public void FilterWishListGames_WhenFilterByVeryPositive_ReturnsFiltered()
        {
            const int First = 0;
            var gamesList = new Collection<Game>
            {
                new Game { Rating = RatingHigh },
                new Game { Rating = RatingMedium },
                new Game { Rating = RatingLow },
                new Game { Rating = RatingPoor }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var filteredGames = userGameService.FilterWishListGames(FilterCriteria.VERYPOSITIVE);
            Assert.Single(filteredGames);
            Assert.Equal(RatingMedium, filteredGames[First].Rating);
        }

        [Fact]
        public void FilterWishListGames_WhenFilterByMixed_ReturnsFiltered()
        {
            const int First = 0;
            var gamesList = new Collection<Game>
            {
                new Game { Rating = RatingHigh },
                new Game { Rating = RatingMedium },
                new Game { Rating = RatingLow },
                new Game { Rating = RatingPoor }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var filteredGames = userGameService.FilterWishListGames(FilterCriteria.MIXED);
            Assert.Single(filteredGames);
            Assert.Equal(RatingLow, filteredGames[First].Rating);
        }

        [Fact]
        public void FilterWishListGames_WhenFilterByNegative_ReturnsFiltered()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Rating = RatingHigh },
                new Game { Rating = RatingMedium },
                new Game { Rating = RatingLow },
                new Game { Rating = RatingPoor }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var filteredGames = userGameService.FilterWishListGames(FilterCriteria.NEGATIVE);
            Assert.Single(filteredGames);
            Assert.Equal(RatingPoor, filteredGames[0].Rating);
        }

        [Fact]
        public void IsGamePurchased_WhenCalled_DelegatesToRepository()
        {
            var game = new Game();
            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(game)).Returns(true);

            Assert.True(userGameService.IsGamePurchased(game));
        }

        [Fact]
        public void SortWishListGames_WhenSortsByRatingAscending_ReturnsSorted()
        {
            const int First = 0;
            const int Second = 1;
            var gamesList = new Collection<Game>
            {
                new Game { Name = FirstGame, Rating = RatingMedium },
                new Game { Name = SecondGame, Rating = RatingHigh }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.RATING, true);
            Assert.Equal(RatingMedium, sortedGames[First].Rating);
            Assert.Equal(RatingHigh, sortedGames[Second].Rating);
        }

        [Fact]
        public void SortWishListGames_WhenSortByRatingDescending_ReturnsSorted()
        {
            const int First = 0;
            const int Second = 1;
            var gamesList = new Collection<Game>
            {
                new Game { Name = FirstGame, Rating = RatingMedium },
                new Game { Name = SecondGame, Rating = RatingHigh }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.RATING, false);
            Assert.Equal(RatingHigh, sortedGames[First].Rating);
            Assert.Equal(RatingMedium, sortedGames[Second].Rating);
        }

        [Fact]
        public void SortWishListGames_WhenSortByPriceAscending_ReturnsSorted()
        {
            const int First = 0;
            const int Second = 1;
            var gamesList = new Collection<Game>
            {
                new Game { Name = FirstGame, Price = PriceHigh },
                new Game { Name = SecondGame, Price = PriceLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.PRICE, true);
            Assert.Equal(PriceLow, sortedGames[First].Price);
            Assert.Equal(PriceHigh, sortedGames[Second].Price);
        }

        [Fact]
        public void SortWishListGames_WhenSortByPriceDescending_ReturnsSorted()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = FirstGame, Price = PriceHigh },
                new Game { Name = SecondGame, Price = PriceLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.PRICE, false);
            Assert.Equal(PriceHigh, sortedGames[0].Price);
            Assert.Equal(PriceLow, sortedGames[1].Price);
        }

        [Fact]
        public void SortWishListGames_WhenSortByDiscountAscending_ReturnsSorted()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = FirstGame, Discount = DiscountHigh },
                new Game { Name = SecondGame, Discount = DiscountLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.DISCOUNT, true);
            Assert.Equal(DiscountLow, sortedGames[0].Discount);
            Assert.Equal(DiscountHigh, sortedGames[1].Discount);
        }

        [Fact]
        public void SortWishListGames_WhenSortByDiscountDescending_ReturnsSorted()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = FirstGame, Discount = DiscountHigh },
                new Game { Name = SecondGame, Discount = DiscountLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.DISCOUNT, false);
            Assert.Equal(DiscountHigh, sortedGames[0].Discount);
            Assert.Equal(DiscountLow, sortedGames[1].Discount);
        }

        [Fact]
        public void GetFavoriteUserTags_WhenNoTags_ReturnsEmptyList()
        {
            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(new Collection<Tag>());
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game>());

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Empty(favoriteTags);
        }
    }
}
