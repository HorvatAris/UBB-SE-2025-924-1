using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Moq;
using SteamStore.Constants;
using SteamStore.Data;
using SteamStore.Repositories.Interfaces;
using Xunit;

namespace SteamStore.Tests.Repositories
{
    public class UserGameRepositoryTest
    {
        private readonly Mock<IDataLink> mockDataLink;
        private readonly User mockUser;
        private readonly UserGameRepository userGameRepository;

        private const int TestUserIdentifier = 100;
        private const float InitialWalletBalance = 100.0f;
        private const float InitialPointsBalance = 100.0f;
        private const int TestGameIdentifier = 1;
        private const float TestPurchaseAmount = 10.0f;
        private const string ExceptionMessageDatabaseError = "Database error";

        public UserGameRepositoryTest()
        {
            mockDataLink = new Mock<IDataLink>();
            mockUser = new User { UserId = TestUserIdentifier, WalletBalance = InitialWalletBalance, PointsBalance = InitialPointsBalance };
            userGameRepository = new UserGameRepository(mockDataLink.Object, mockUser);
        }

        [Fact]
        public void IsGamePurchased_WhenGameIsPurchased_ShouldReturnTrue()
        {
            const int ItemWasPurchased = 1;
            var purchasedGame = new Game { GameId = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalar<int>(SqlConstants.IsGamePurchasedProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(ItemWasPurchased);

            var isPurchased = userGameRepository.IsGamePurchased(purchasedGame);

            Assert.True(isPurchased);
        }

        [Fact]
        public void IsGamePurchased_WhenGameIsNotPurchased_ShouldReturnFalse()
        {
            const int ItemWasNotPurchased = 0;
            var unpurchasedGame = new Game { GameId = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalar<int>(SqlConstants.IsGamePurchasedProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(ItemWasNotPurchased);

            var isPurchased = userGameRepository.IsGamePurchased(unpurchasedGame);

            Assert.False(isPurchased);
        }

        [Fact]
        public void RemoveGameFromWishlist_WhenGameIsValid_CallsExecuteNonQuery()
        {
            var gameToRemove = new Game { GameId = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.RemoveGameFromWishlist(gameToRemove);

            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void RemoveGameFromWishlist_WhenDatabaseErrorOccurs_ThrowsException()
        {
            var gameToRemove = new Game { GameId = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionRemoveFromWishList = Assert.Throws<Exception>(() => userGameRepository.RemoveGameFromWishlist(gameToRemove));
            Assert.Equal(ExceptionMessageDatabaseError, exceptionRemoveFromWishList.Message);
        }

        [Fact]
        public void AddGameToPurchased_WhenFundsAreInsufficient_ThrowsException()
        {
            const decimal TestGamePriceExpensive = 200.0m;
            const string ExceptionMessageInsufficientFunds = "Insufficient funds";

            var expensiveGame = new Game { GameId = TestGameIdentifier, Price = TestGamePriceExpensive };
            var exceptionAddGameToPurchased = Assert.Throws<Exception>(() => userGameRepository.AddGameToPurchased(expensiveGame));

            Assert.Equal(ExceptionMessageInsufficientFunds, exceptionAddGameToPurchased.Message);
        }

        [Fact]
        public void AddGameToPurchased_WhenPurchaseIsSuccessful_UpdatesWalletBalance()
        {
            const decimal TestGamePriceAffordable = 10.0m;
            const float TestGameCheckPrice = 90.0f;

            var affordableGame = new Game { GameId = TestGameIdentifier, Price = TestGamePriceAffordable };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToPurchasedGamesProcedure, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.AddGameToPurchased(affordableGame);

            Assert.Equal(TestGameCheckPrice, mockUser.WalletBalance);
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToPurchasedGamesProcedure, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_WhenGameIsValid_CallsExecuteNonQuery()
        {
            var gameToAdd = new Game { GameId = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.AddGameToWishlist(gameToAdd);

            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_WhenDatabaseFails_ThrowsException()
        {
            var gameToAdd = new Game { GameId = TestGameIdentifier };

            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionAddGameToWishlist = Assert.Throws<Exception>(() => userGameRepository.AddGameToWishlist(gameToAdd));
            Assert.Equal(ExceptionMessageDatabaseError, exceptionAddGameToWishlist.Message);
        }

        [Fact]
        public void GetGameTags_WhenTagsAreAvailable_ReturnsTagList()
        {
            const string TestTag1 = "Action";
            const string TestTag2 = "Adventure";

            var tagsTable = new DataTable();
            tagsTable.Columns.Add(SqlConstants.TagNameColumn);
            tagsTable.Rows.Add(TestTag1);
            tagsTable.Rows.Add(TestTag2);

            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(tagsTable);

            var gameTags = userGameRepository.GetGameTags(TestGameIdentifier);

            var expectedTags = new[] { TestTag1, TestTag2 };
            Assert.Equal(expectedTags, gameTags);
        }

        [Fact]
        public void GetGameTags_WhenDatabaseFails_ThrowsException()
        {
            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionGetGameTags = Assert.Throws<Exception>(() => userGameRepository.GetGameTags(TestGameIdentifier));

            Assert.Equal($"Error getting tags for game {TestGameIdentifier}: {ExceptionMessageDatabaseError}", exceptionGetGameTags.Message);
        }

        [Fact]
        public void GetGameOwnerCount_WhenDataExists_ReturnsCorrectCount()
        {
            const string OwnerCountParameter = "OwnerCount";
            const int OwnerCountValue = 5;
            var ownerCountTable = new DataTable();
            ownerCountTable.Columns.Add(OwnerCountParameter);
            ownerCountTable.Rows.Add(OwnerCountValue);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameOwnerCountProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(ownerCountTable);

            var expectedOwnerCount = userGameRepository.GetGameOwnerCount(TestGameIdentifier);

            Assert.Equal(OwnerCountValue, expectedOwnerCount);
        }

        [Fact]
        public void GetGameOwnerCount_WhenNoDataAvailable_ReturnsZero()
        {
            const int OwnerCountMinimum = 0;
            var emptyTable = new DataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameOwnerCountProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(emptyTable);

            var expectedOwnerCount = userGameRepository.GetGameOwnerCount(TestGameIdentifier);

            Assert.Equal(OwnerCountMinimum, expectedOwnerCount);
        }

        [Fact]
        public void AddPointsForPurchase_WhenValidAmountIsProvided_IncreasesUserPoints()
        {
            const float PointBalanceMinimum = 0.0f;
            const int TestRewardPoints = 1210;
            mockUser.PointsBalance = PointBalanceMinimum;
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.UpdateUserPointBalance, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.AddPointsForPurchase(TestPurchaseAmount);

            Assert.Equal(TestRewardPoints, mockUser.PointsBalance);
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.UpdateUserPointBalance, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void AddPointsForPurchase_WhenDatabaseFails_ThrowsException()
        {
            const string ExceptionMessageFailAddPoints = "Failed to add points for purchase: Database error";
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.UpdateUserPointBalance, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionFailAddPoints = Assert.Throws<Exception>(() => userGameRepository.AddPointsForPurchase(TestPurchaseAmount));

            Assert.Equal(ExceptionMessageFailAddPoints, exceptionFailAddPoints.Message);
        }

        [Fact]
        public void GetUserPointsBalance_Always_ReturnsCorrectBalance()
        {
            mockUser.PointsBalance = InitialPointsBalance;

            var expectedUserPointsBalance = userGameRepository.GetUserPointsBalance();

            Assert.Equal(InitialPointsBalance, expectedUserPointsBalance);
        }

        [Fact]
        public void GetWishlistGames_WhenDataExists_ReturnsListOfGames()
        {
            var wishlistTable = new DataTable();
            wishlistTable.Columns.Add(SqlConstants.GameIdColumn, typeof(int));
            wishlistTable.Columns.Add(SqlConstants.GameNameColumn, typeof(string));
            wishlistTable.Columns.Add(SqlConstants.GamePriceColumn, typeof(decimal));
            wishlistTable.Columns.Add(SqlConstants.DescriptionIdColumnWithCapitalLetter, typeof(string));
            wishlistTable.Columns.Add(SqlConstants.ImageUrlColumn, typeof(string));
            wishlistTable.Columns.Add(SqlConstants.MinimumRequirementsColumn, typeof(string));
            wishlistTable.Columns.Add(SqlConstants.RecommendedRequirementsColumn, typeof(string));
            wishlistTable.Columns.Add(SqlConstants.GameStatusColumn, typeof(string));
            wishlistTable.Columns.Add(SqlConstants.DiscountColumn, typeof(decimal));
            wishlistTable.Columns.Add(SqlConstants.RatingColumn, typeof(decimal));

            const int FirstGameIdentifier = 1;
            const string FirstGameName = "FirstGame";
            const decimal FirstGamePrice = 19.99m;
            const string FirstGameDescription = "FirstGame GameDescription";
            const string FirstGameImage = "FirstGame Image";
            const string FirstGameMinimumRequirement = "FirstGame Min";
            const string FirstGameRecommendedRequirement = "FirstGame Recommended";
            const string FirstGameStatus = "Available";
            const decimal FirstGameDiscount = 10.0m;
            const decimal FirstGameRating = 4.5m;

            const int SecondGameIdentifier = 2;
            const string SecondGameName = "SecondGame";
            const decimal SecondGamePrice = 29.99m;
            const string SecondGameDescription = "SecondGame GameDescription";
            const string SecondGameImage = "SecondGame Image";
            const string SecondGameMinimumRequirement = "SecondGame Min";
            const string SecondGameRecommendedRequirement = "SecondGame Recommended";
            const string SecondGameStatus = "Available";
            const decimal SecondGameDiscount = 15.0m;
            const decimal SecondGameRating = 4.0m;
            const int ExpectedCountGamesWishlist = 2;

            wishlistTable.Rows.Add(FirstGameIdentifier, FirstGameName, FirstGamePrice, FirstGameDescription, FirstGameImage, FirstGameMinimumRequirement, FirstGameRecommendedRequirement, FirstGameStatus, FirstGameDiscount, FirstGameRating);
            wishlistTable.Rows.Add(SecondGameIdentifier, SecondGameName, SecondGamePrice, SecondGameDescription, SecondGameImage, SecondGameMinimumRequirement, SecondGameRecommendedRequirement, SecondGameStatus, SecondGameDiscount, SecondGameRating);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetWishlistGamesProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(wishlistTable);

            var wishlistGames = userGameRepository.GetWishlistGames();

            Assert.Equal(ExpectedCountGamesWishlist, wishlistGames.Count);
        }

        [Fact]
        public void GetWishlistGames_WhenNoDataExists_ReturnsEmptyList()
        {
            var emptyWishlist = new DataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetWishlistGamesProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(emptyWishlist);

            var wishlistGames = userGameRepository.GetWishlistGames();

            Assert.Empty(wishlistGames);
        }
    }
}
