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
            mockUser = new User { UserIdentifier = TestUserIdentifier, WalletBalance = InitialWalletBalance, PointsBalance = InitialPointsBalance };
            userGameRepository = new UserGameRepository(mockDataLink.Object, mockUser);
        }

        [Fact]
        public void IsGamePurchased_ReturnsTrue_WhenGameIsPurchased()
        {
            var purchasedGame = new Game { Identifier = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalar<int>(SqlConstants.IsGamePurchasedProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(1);

            var isPurchased = userGameRepository.IsGamePurchased(purchasedGame);

            Assert.True(isPurchased);
        }

        [Fact]
        public void IsGamePurchased_ReturnsFalse_WhenGameIsNotPurchased()
        {
            var unpurchasedGame = new Game { Identifier = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalar<int>(SqlConstants.IsGamePurchasedProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(0);

            var isPurchased = userGameRepository.IsGamePurchased(unpurchasedGame);

            Assert.False(isPurchased);
        }

        [Fact]
        public void RemoveGameFromWishlist_CallsExecuteNonQuery_WhenGameIsValid()
        {
            var gameToRemove = new Game { Identifier = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.RemoveGameFromWishlist(gameToRemove);

            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void RemoveGameFromWishlist_ThrowsException_WhenDatabaseErrorOccurs()
        {
            var gameToRemove = new Game { Identifier = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionRemoveFromWishList = Assert.Throws<Exception>(() => userGameRepository.RemoveGameFromWishlist(gameToRemove));
            Assert.Equal(ExceptionMessageDatabaseError, exceptionRemoveFromWishList.Message);
        }

        [Fact]
        public void AddGameToPurchased_ThrowsException_WhenFundsAreInsufficient()
        {
            const decimal TestGamePriceExpensive = 200.0m;
            const string ExceptionMessageInsufficientFunds = "Insufficient funds";

            var expensiveGame = new Game { Identifier = TestGameIdentifier, Price = TestGamePriceExpensive };
            var exceptionAddGameToPurchased = Assert.Throws<Exception>(() => userGameRepository.AddGameToPurchased(expensiveGame));

            Assert.Equal(ExceptionMessageInsufficientFunds, exceptionAddGameToPurchased.Message);
        }

        [Fact]
        public void AddGameToPurchased_UpdatesWalletBalance_WhenPurchaseIsSuccessful()
        {
            const decimal TestGamePriceAffordable = 10.0m;
            const float TestGameCheckPrice = 90.0f;

            var affordableGame = new Game { Identifier = TestGameIdentifier, Price = TestGamePriceAffordable };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToPurchasedGamesProcedure, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.AddGameToPurchased(affordableGame);

            Assert.Equal(TestGameCheckPrice, mockUser.WalletBalance);
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToPurchasedGamesProcedure, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_CallsExecuteNonQuery_WhenGameIsValid()
        {
            var gameToAdd = new Game { Identifier = TestGameIdentifier };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            userGameRepository.AddGameToWishlist(gameToAdd);

            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Fact]
        public void AddGameToWishlist_ThrowsException_WhenDatabaseFails()
        {
            var gameToAdd = new Game { Identifier = TestGameIdentifier };

            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionAddGameToWishlist = Assert.Throws<Exception>(() => userGameRepository.AddGameToWishlist(gameToAdd));
            Assert.Equal(ExceptionMessageDatabaseError, exceptionAddGameToWishlist.Message);
        }

        [Fact]
        public void GetGameTags_ReturnsTagList_WhenTagsAreAvailable()
        {
            const string TestTag1 = "Action";
            const string TestTag2 = "Adventure";
            const int GetNumberGameTagsExpected = 2;

            var tagsTable = new DataTable();
            tagsTable.Columns.Add(SqlConstants.TagNameColumn);
            tagsTable.Rows.Add(TestTag1);
            tagsTable.Rows.Add(TestTag2);

            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(tagsTable);

            var gameTags = userGameRepository.GetGameTags(TestGameIdentifier);

            Assert.Equal(GetNumberGameTagsExpected, gameTags.Length);
            Assert.Contains(TestTag1, gameTags);
            Assert.Contains(TestTag2, gameTags);
        }

        [Fact]
        public void GetGameTags_ThrowsException_WhenDatabaseFails()
        {
            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionGetGameTags = Assert.Throws<Exception>(() => userGameRepository.GetGameTags(TestGameIdentifier));
            Assert.Equal($"Error getting tags for game {TestGameIdentifier}: {ExceptionMessageDatabaseError}", exceptionGetGameTags.Message);
        }

        [Fact]
        public void GetGameOwnerCount_ReturnsCorrectCount_WhenDataExists()
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
        public void GetGameOwnerCount_ReturnsZero_WhenNoDataAvailable()
        {
            const int OwnerCountMinimum = 0;

            var emptyTable = new DataTable();

            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetGameOwnerCountProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(emptyTable);

            var expectedOwnerCount = userGameRepository.GetGameOwnerCount(TestGameIdentifier);

            Assert.Equal(OwnerCountMinimum, expectedOwnerCount);
        }

        [Fact]
        public void AddPointsForPurchase_IncreasesUserPoints_WhenValidAmountIsProvided()
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
        public void AddPointsForPurchase_ThrowsException_WhenDatabaseFails()
        {
            const string ExceptionMessageFailAddPoints = "Failed to add points for purchase: Database error";

            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery(SqlConstants.UpdateUserPointBalance, It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception(ExceptionMessageDatabaseError));

            var exceptionFailAddPoints = Assert.Throws<Exception>(() => userGameRepository.AddPointsForPurchase(TestPurchaseAmount));

            Assert.Equal(ExceptionMessageFailAddPoints, exceptionFailAddPoints.Message);
        }

        [Fact]
        public void GetUserPointsBalance_ReturnsCorrectBalance()
        {
            mockUser.PointsBalance = InitialPointsBalance;

            var expectedUserPointsBalance = userGameRepository.GetUserPointsBalance();

            Assert.Equal(InitialPointsBalance, expectedUserPointsBalance);
        }

        [Fact]
        public void GetWishlistGames_ReturnsListOfGames_WhenDataExists()
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

            const int Game1Identifier = 1;
            const string Game1Name = "Game1";
            const decimal Game1Price = 19.99m;
            const string Game1Description = "Desc1";
            const string Game1Image = "Image1";
            const string Game1MinimumRequirement = "Min1";
            const string Game1RecommendedRequirement = "Rec1";
            const string Game1Status = "Available";
            const decimal Game1Discount = 10.0m;
            const decimal Game1Rating = 4.5m;

            const int Game2Identifier = 2;
            const string Game2Name = "Game2";
            const decimal Game2Price = 29.99m;
            const string Game2Description = "Desc2";
            const string Game2Image = "Image2";
            const string Game2MinimumRequirement = "Min2";
            const string Game2RecommendedRequirement = "Rec2";
            const string Game2Status = "Available";
            const decimal Game2Discount = 15.0m;
            const decimal Game2Rating = 4.0m;

            const int ExpectedCountGamesWishlist = 2;

            wishlistTable.Rows.Add(Game1Identifier, Game1Name, Game1Price, Game1Description, Game1Image, Game1MinimumRequirement, Game1RecommendedRequirement, Game1Status, Game1Discount, Game1Rating);
            wishlistTable.Rows.Add(Game2Identifier, Game2Name, Game2Price, Game2Description, Game2Image, Game2MinimumRequirement, Game2RecommendedRequirement, Game2Status, Game2Discount, Game2Rating);

            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetWishlistGamesProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(wishlistTable);

            var wishlistGames = userGameRepository.GetWishlistGames();

            Assert.Equal(ExpectedCountGamesWishlist, wishlistGames.Count);
        }

        [Fact]
        public void GetWishlistGames_ReturnsEmptyList_WhenNoDataExists()
        {
            var emptyWishlist = new DataTable();

            mockDataLink.Setup(dataLink => dataLink.ExecuteReader(SqlConstants.GetWishlistGamesProcedure, It.IsAny<SqlParameter[]>()))
                        .Returns(emptyWishlist);

            var wishlistGames = userGameRepository.GetWishlistGames();

            Assert.Empty(wishlistGames);
        }
    }
}
