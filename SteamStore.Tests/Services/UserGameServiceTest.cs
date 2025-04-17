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

        // Constants to avoid magic strings and numbers
        private const string GameName1 = "Game1";
        private const string GameName2 = "Game2";
        private const string GameName3 = "Game3";
        private const string TagName1 = "RPG";
        private const string TagName2 = "FPS";
        private const string TagName3 = "MMG";
        private const string TagName4 = "BLP";
        private const int UserPointsBeforePurchase = 10;
        private const int UserPointsAfterPurchase = 15;

        private const int NumberOfUsersGamesWithTagUsedOnce = 1;
        private const int NumberOfFavoriteTagsMaximum = 3;
        private const int NumberOfFavoriteTagsExpected = 2;
        private const decimal TagScoreComparator = 0.0001m;
        private const decimal TagScoreMinimum = 0.0m;
        private const decimal TagScoreExpected = 1m;
        private const decimal TrendingScoreExpected1 = 0.5m;
        private const decimal TrendingScoreExpected2 = 1.0m;
        private const int NumberRecommendedGamesMaximum = 10;
        private const int NumberRecommendedGamesExpected = 2;
        private const int NumberGenerateGamesNumberMinimum = 1;
        private const int NumberGenerateGamesNumberMaximum = 15;
        private const string SearchWishListString = "Game";
        private const decimal RatingHigh = 4.7m;
        private const decimal RatingMedium = 4.3m;
        private const decimal RatingLow = 2.5m;
        private const decimal RatingPoor = 1.5m;
        private const decimal DiscountHigh = 50;
        private const decimal DiscountLow = 10;
        private const int PriceHigh = 20;
        private const int PriceLow = 10;
        private const int RecentPurchasesGame1 = 5;
        private const int RecentPurchasesGame2 = 10;

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
        public void RemoveGameFromWishlist_Successful_CallsRepository()
        {
            var gameToRemove = new Game();

            userGameService.RemoveGameFromWishlist(gameToRemove);

            mockUserGameRepository.Verify(repository => repository.RemoveGameFromWishlist(gameToRemove), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_WhenAlreadyOwned_ThrowsException()
        {
            var gameToAdd = new Game { Name = GameName1 };

            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(gameToAdd)).Returns(true);

            var exceptionAddGameToWishList = Assert.Throws<Exception>(() => userGameService.AddGameToWishlist(gameToAdd));
            Assert.Equal(string.Format(ExceptionMessages.GameAlreadyOwned, GameName1), exceptionAddGameToWishList.Message);
        }

        [Fact]
        public void AddGameToWishlist_WhenNotOwned_CallsRepository()
        {
            var gameToAdd = new Game { Name = GameName2 };

            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(gameToAdd)).Returns(false);

            userGameService.AddGameToWishlist(gameToAdd);
            mockUserGameRepository.Verify(repository => repository.AddGameToWishlist(gameToAdd), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_WhenSqlException_ThrowsException()
        {
            var gameToAdd = new Game { Name = GameName3 };

            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(gameToAdd)).Returns(false);
            mockUserGameRepository.Setup(repository => repository.AddGameToWishlist(gameToAdd)).Throws(new Exception(ErrorStrings.SQLNONQUERYFAILUREINDICATOR));

            var exception = Assert.Throws<Exception>(() => userGameService.AddGameToWishlist(gameToAdd));
            Assert.Equal(string.Format(ExceptionMessages.GameAlreadyInWishlist, GameName3), exception.Message);
        }

        [Fact]
        public void PurchaseGames_Succesful_CallsRepository()
        {
            var gameToPurchase = new Game { Name = GameName1 };
            var gamesToPurchase = new List<Game> { gameToPurchase };

            mockUserGameRepository.SetupSequence(repository => repository.GetUserPointsBalance())
                                  .Returns(UserPointsBeforePurchase)
                                  .Returns(UserPointsAfterPurchase);

            userGameService.PurchaseGames(gamesToPurchase);

            mockUserGameRepository.Verify(repository => repository.AddGameToPurchased(gameToPurchase), Times.Once);
            mockUserGameRepository.Verify(repository => repository.RemoveGameFromWishlist(gameToPurchase), Times.Once);
        }

        [Fact]
        public void PurchaseGames_Succesful_CalculatesPointsCorrectly()
        {
            var gameToPurchase = new Game { Name = GameName1 };
            var gamesToPurchase = new List<Game> { gameToPurchase };

            mockUserGameRepository.SetupSequence(repository => repository.GetUserPointsBalance())
                                  .Returns(UserPointsBeforePurchase)
                                  .Returns(UserPointsAfterPurchase);

            userGameService.PurchaseGames(gamesToPurchase);

            Assert.Equal(UserPointsAfterPurchase - UserPointsBeforePurchase, userGameService.LastEarnedPoints);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_SetCountToOne_WhenTagIsUsedInOneGame()
        {
            var tagUsed = new Tag { Tag_name = TagName1 };
            var gameWithTag = new Game { Name = GameName1, Tags = new[] { TagName1 } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { gameWithTag });

            var tagCollection = new Collection<Tag> { tagUsed };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagUsedOnce, tagUsed.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_SetCountToZero_WhenTagNotPresentInAnyGame()
        {
            const int NumberOfUsersGamesWithTagNeverUsed = 0;

            var tagNotUsed = new Tag { Tag_name = TagName2 };
            var gameWithoutTag = new Game { Name = GameName1, Tags = new[] { TagName1 } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { gameWithoutTag });

            var tagCollection = new Collection<Tag> { tagNotUsed };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagNeverUsed, tagNotUsed.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_SetCountCorrectly_WhenMultipleGamesForSameTag()
        {
            const int NumberOfUsersGamesWithTagUsedMultiple = 2;

            var tagUsed = new Tag { Tag_name = TagName1 };
            var gameUsingTag1 = new Game { Name = GameName1, Tags = new[] { TagName1 } };
            var gameUsingTag2 = new Game { Name = GameName2, Tags = new[] { TagName1, TagName2 } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { gameUsingTag1, gameUsingTag2 });

            var tagCollection = new Collection<Tag> { tagUsed };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagUsedMultiple, tagUsed.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void ComputeNumberOfUserGamesForEachTag_SetCountCorrectly_HandleMultipleTagsAndGames()
        {
            var tagUsed1 = new Tag { Tag_name = TagName1 };
            var tagUsed2 = new Tag { Tag_name = TagName2 };

            var multipleTagGame = new Game { Name = GameName1, Tags = new[] { TagName1, TagName2 } };

            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game> { multipleTagGame });

            var tagCollection = new Collection<Tag> { tagUsed1, tagUsed2 };

            userGameService.ComputeNoOfUserGamesForEachTag(tagCollection);

            Assert.Equal(NumberOfUsersGamesWithTagUsedOnce, tagUsed1.NumberOfUserGamesWithTag);
            Assert.Equal(NumberOfUsersGamesWithTagUsedOnce, tagUsed2.NumberOfUserGamesWithTag);
        }

        [Fact]
        public void GetFavoriteUserTags_ReturnsTop3_VerifyCount()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 },
                new Tag { Tag_name = TagName3 }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { TagName2, TagName3 } },
                new Game { Tags = new[] { TagName1, TagName2 } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(NumberOfFavoriteTagsMaximum, favoriteTags.Count);
        }

        [Fact]
        public void GetFavoriteUserTags_ReturnsTop3_VerifyTag()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 },
                new Tag { Tag_name = TagName3 }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { TagName2, TagName3 } },
                new Game { Tags = new[] { TagName1, TagName2 } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(TagName2, favoriteTags[0].Tag_name);
        }

        [Fact]
        public void GetFavoriteUserTags_ReturnsEmpty_WhenNoGames()
        {
            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(new Collection<Tag>());
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(new Collection<Game>());

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Empty(favoriteTags);
        }

        [Fact]
        public void GetFavoriteUserTags_ReturnsOnlyExistingTags_VerifyCount()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { TagName1 } },
                new Game { Tags = new[] { TagName1 } },
                new Game { Tags = new[] { TagName2 } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(NumberOfFavoriteTagsExpected, favoriteTags.Count);
        }

        [Fact]
        public void GetFavoriteUserTags_ReturnsOnlyExistingTags_VerifyTag()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { TagName1 } },
                new Game { Tags = new[] { TagName1 } },
                new Game { Tags = new[] { TagName2 } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(TagName1, favoriteTags[0].Tag_name);
        }

        [Fact]
        public void GetFavoriteUserTags_IgnoresTagsNotInList()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { TagName1, TagName2, TagName3 } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Single(favoriteTags);
            Assert.Equal(TagName1, favoriteTags[0].Tag_name);
        }

        [Fact]
        public void GetFavoriteUserTags_ReturnsTop3_WhenMoreTags()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 },
                new Tag { Tag_name = TagName3 },
                new Tag { Tag_name = TagName4 }
            };

            var userGames = new Collection<Game>
            {
                new Game { Tags = new[] { TagName1, TagName2, TagName3 } },
                new Game { Tags = new[] { TagName2, TagName4 } },
                new Game { Tags = new[] { TagName4 } }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(userGames);

            var favoriteTags = userGameService.GetFavoriteUserTags();

            Assert.Equal(NumberOfFavoriteTagsMaximum, favoriteTags.Count);
        }

        [Fact]
        public void ComputeTagScoreForGames_CalculatesProperly()
        {
            var gameToScore = new Game { Tags = new[] { TagName1, TagName2 } };

            var allUserGames = new Collection<Game>
            {
                gameToScore,
                new Game { Tags = new[] { TagName2 } },
                new Game { Tags = new[] { TagName3 } }
            };

            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 },
                new Tag { Tag_name = TagName3 }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(allUserGames);

            userGameService.ComputeTagScoreForGames(allUserGames);

            Assert.True(Math.Abs(TagScoreExpected - gameToScore.TagScore) < TagScoreComparator);
        }

        [Fact]
        public void ComputeTagScoreForGames_SetsZero_WhenNoMatchingTags()
        {
            var gameWithoutValidTags = new Game { Tags = new[] { TagName4 } };

            var allUserGames = new Collection<Game> { gameWithoutValidTags };

            var knownTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 },
                new Tag { Tag_name = TagName3 }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(knownTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(allUserGames);

            userGameService.ComputeTagScoreForGames(allUserGames);

            Assert.Equal(TagScoreMinimum, gameWithoutValidTags.TagScore);
        }

        [Fact]
        public void ComputeTagScoreForGames_CalculatesCorrectly_WithUnevenTagCounts()
        {
            var gameToEvaluate = new Game { Tags = new[] { TagName2 } };

            var allUserGames = new Collection<Game>
            {
                gameToEvaluate,
                new Game { Tags = new[] { TagName2 } },
                new Game { Tags = new[] { TagName2 } },
                new Game { Tags = new[] { TagName1 } }
            };

            var knownTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1 },
                new Tag { Tag_name = TagName2 }
            };

            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(knownTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(allUserGames);

            userGameService.ComputeTagScoreForGames(allUserGames);

            Assert.True(gameToEvaluate.TagScore > TagScoreMinimum);
        }

        [Fact]
        public void ComputeTrendingScores_SetsTrendingScore()
        {
            var trendingGames = new Collection<Game>
            {
                new Game { Name = GameName1, NumberOfRecentPurchases = RecentPurchasesGame1 },
                new Game { Name = GameName2, NumberOfRecentPurchases = RecentPurchasesGame2 }
            };

            userGameService.ComputeTrendingScores(trendingGames);

            Assert.Equal(TrendingScoreExpected1, trendingGames[0].TrendingScore);
            Assert.Equal(TrendingScoreExpected2, trendingGames[1].TrendingScore);
        }

        [Fact]
        public void ComputeTrendingScores_HandlesSingleGame()
        {
            var singleGame = new Collection<Game>
            {
                new Game { Name = GameName1, NumberOfRecentPurchases = RecentPurchasesGame1 }
            };

            userGameService.ComputeTrendingScores(singleGame);

            Assert.Equal(TrendingScoreExpected2, singleGame[0].TrendingScore);
        }

        [Fact]
        public void GetRecommendedGames_ReturnsTop10()
        {
            var recommendedGames = new Collection<Game>
            {
                new Game { NumberOfRecentPurchases = RecentPurchasesGame1, Tags = new[] { TagName1 } },
                new Game { NumberOfRecentPurchases = RecentPurchasesGame2, Tags = new[] { TagName1 } }
            };

            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1, NumberOfUserGamesWithTag = 2 }
            };

            mockGameRepository.Setup(repository => repository.GetAllGames()).Returns(recommendedGames);
            mockTagRepository.Setup(repository => repository.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(repository => repository.GetAllUserGames()).Returns(recommendedGames);

            var recommendedResult = userGameService.GetRecommendedGames();

            Assert.Equal(NumberRecommendedGamesExpected, recommendedResult.Count);
        }

        [Fact]
        public void GetRecommendedGames_ReturnsTop10_WhenMoreThan10Games()
        {
            var allTags = new Collection<Tag>
            {
                new Tag { Tag_name = TagName1, NumberOfUserGamesWithTag = NumberGenerateGamesNumberMaximum }
            };

            var allGames = Enumerable.Range(NumberGenerateGamesNumberMinimum, NumberGenerateGamesNumberMaximum).Select(gameIndex => new Game
            {
                Name = $"Game{gameIndex}",
                NumberOfRecentPurchases = gameIndex,
                Tags = new[] { TagName1 }
            }).ToList();

            mockGameRepository.Setup(r => r.GetAllGames()).Returns(new Collection<Game>(allGames));
            mockTagRepository.Setup(r => r.GetAllTags()).Returns(allTags);
            mockUserGameRepository.Setup(r => r.GetAllUserGames()).Returns(new Collection<Game>(allGames));

            var recommendedGames = userGameService.GetRecommendedGames();

            Assert.Equal(NumberRecommendedGamesMaximum, recommendedGames.Count);
        }

        [Fact]
        public void SearchWishListByName_ReturnsMatches_SpecificWord()
        {
            var wishlistGames = new Collection<Game>
            {
                new Game { Name = GameName1 },
                new Game { Name = GameName2 }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(wishlistGames);

            var matchedGames = userGameService.SearchWishListByName(GameName1);
            Assert.Single(matchedGames);
            Assert.Equal(GameName1, matchedGames[0].Name);
        }

        [Fact]
        public void SearchWishListByName_ReturnsMatches_AllWords()
        {
            var wishlistGames = new Collection<Game>
            {
                new Game { Name = GameName1 },
                new Game { Name = GameName2 }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(wishlistGames);

            var matchedGames = userGameService.SearchWishListByName(SearchWishListString);
            Assert.Equal(wishlistGames, matchedGames);
        }

        [Fact]
        public void FilterWishListGames_FiltersByOverwhelminglyPositive()
        {
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
            Assert.Equal(RatingHigh, filteredGames[0].Rating);
        }

        [Fact]
        public void FilterWishListGames_FiltersByVeryPositive()
        {
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
            Assert.Equal(RatingMedium, filteredGames[0].Rating);
        }

        [Fact]
        public void FilterWishListGames_FiltersByMixed()
        {
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
            Assert.Equal(RatingLow, filteredGames[0].Rating);
        }

        [Fact]
        public void FilterWishListGames_FiltersByNegative()
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
        public void IsGamePurchased_DelegatesToRepository()
        {
            var game = new Game();
            mockUserGameRepository.Setup(repository => repository.IsGamePurchased(game)).Returns(true);

            Assert.True(userGameService.IsGamePurchased(game));
        }

        [Fact]
        public void SortWishListGames_SortsByRatingAscending()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = GameName1, Rating = RatingMedium },
                new Game { Name = GameName2, Rating = RatingHigh }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.RATING, true);
            Assert.Equal(RatingMedium, sortedGames[0].Rating);
            Assert.Equal(RatingHigh, sortedGames[1].Rating);
        }

        [Fact]
        public void SortWishListGames_SortsByRatingDescending()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = GameName1, Rating = RatingMedium },
                new Game { Name = GameName2, Rating = RatingHigh }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.RATING, false);
            Assert.Equal(RatingHigh, sortedGames[0].Rating);
            Assert.Equal(RatingMedium, sortedGames[1].Rating);
        }

        [Fact]
        public void SortWishListGames_SortsByPriceAscending()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = GameName1, Price = PriceHigh },
                new Game { Name = GameName2, Price = PriceLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.PRICE, true);
            Assert.Equal(PriceLow, sortedGames[0].Price);
            Assert.Equal(PriceHigh, sortedGames[1].Price);
        }

        [Fact]
        public void SortWishListGames_SortsByPriceDescending()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = GameName1, Price = PriceHigh },
                new Game { Name = GameName2, Price = PriceLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.PRICE, false);
            Assert.Equal(PriceHigh, sortedGames[0].Price);
            Assert.Equal(PriceLow, sortedGames[1].Price);
        }

        [Fact]
        public void SortWishListGames_SortsByDiscountAscending()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = GameName1, Discount = DiscountHigh },
                new Game { Name = GameName2, Discount = DiscountLow }
            };

            mockUserGameRepository.Setup(repository => repository.GetWishlistGames()).Returns(gamesList);

            var sortedGames = userGameService.SortWishListGames(FilterCriteria.DISCOUNT, true);
            Assert.Equal(DiscountLow, sortedGames[0].Discount);
            Assert.Equal(DiscountHigh, sortedGames[1].Discount);
        }

        [Fact]
        public void SortWishListGames_SortsByDiscountDescending()
        {
            var gamesList = new Collection<Game>
            {
                new Game { Name = GameName1, Discount = DiscountHigh },
                new Game { Name = GameName2, Discount = DiscountLow }
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
