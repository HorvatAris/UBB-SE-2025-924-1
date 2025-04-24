using System.Data;
using System.Data.SqlClient;
using global::SteamStore.Constants;
using global::SteamStore.Data;
using Moq;

namespace SteamStore.Tests.Repositories;

public class CartRepositoryTests
{
	private readonly Mock<IDataLink> dataLinkMock;
	private readonly User testUser;
	private readonly CartRepository cartRepository;

	private const int TestUserIdentifier = 1;
	private const float TestWalletBallance = 100.5f;

	public CartRepositoryTests()
	{
		dataLinkMock = new Mock<IDataLink>();
		testUser = new User
		{
			UserId = TestUserIdentifier,
			WalletBalance = TestWalletBallance
		};
		cartRepository = new CartRepository(dataLinkMock.Object, testUser);
	}

	[Fact]
	public void GetCartGames_WhenDataExists_ShouldReturnAsMuchData()
	{
		var table = new DataTable();
		var expectedGamesCount = 2;
		table.Columns.Add(SqlConstants.GAMEIDCOLUMN, typeof(int));
		table.Columns.Add(SqlConstants.NAMECOLUMN, typeof(string));
		table.Columns.Add(SqlConstants.DESCRIPTIONCOLUMN, typeof(string));
		table.Columns.Add(SqlConstants.IMAGEURLCOLUMN, typeof(string));
		table.Columns.Add(SqlConstants.PRICECOLUMN, typeof(decimal));

		var row = table.NewRow();
		row[SqlConstants.GAMEIDCOLUMN] = 1;
		row[SqlConstants.NAMECOLUMN] = "GameName";
		row[SqlConstants.DESCRIPTIONCOLUMN] = "GameDescription";
		row[SqlConstants.IMAGEURLCOLUMN] = "image.png";
		row[SqlConstants.PRICECOLUMN] = 19.99m;
		table.Rows.Add(row);

		row = table.NewRow();
		row[SqlConstants.GAMEIDCOLUMN] = 2;
		row[SqlConstants.NAMECOLUMN] = "GameName2";
		row[SqlConstants.DESCRIPTIONCOLUMN] = "Description2";
		row[SqlConstants.IMAGEURLCOLUMN] = "image2.png";
		row[SqlConstants.PRICECOLUMN] = 20.99m;
		table.Rows.Add(row);

		dataLinkMock
			.Setup(dataLinkMock => dataLinkMock.ExecuteReader(SqlConstants.GetAllCartGamesProcedure, It.IsAny<SqlParameter[]>()))
			.Returns(table);

		var result = cartRepository.GetCartGames();
		var actualGamesCount = result.Count;

		Assert.Equal(expectedGamesCount, actualGamesCount);
	}

	[Fact]
	public void GetCartGames_WhenNoData_ShouldReturnEmptyList()
	{
		dataLinkMock
			.Setup(dataLinkMock => dataLinkMock.ExecuteReader(SqlConstants.GetAllCartGamesProcedure, It.IsAny<SqlParameter[]>()))
			.Returns((DataTable)null);

		var result = cartRepository.GetCartGames();

		Assert.Empty(result);
	}

	[Fact]
	public void AddGameToCart_WhenCalledValidIdentifier_ShouldExecuteQuery()
	{
		var game = new Game { GameId = TestUserIdentifier };

		cartRepository.AddGameToCart(game);

		dataLinkMock.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuery(SqlConstants.AddGameToCartProcedure,
			It.IsAny<SqlParameter[]>()), Times.Once);
	}

	[Fact]
	public void AddGameToCart_WhenDataLinkFails_ShouldThrowWrappedException()
	{
		var game = new Game { GameId = TestUserIdentifier };
		var expectedExceptionErrorMessage = "SQL error";

		dataLinkMock
			.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
			.Throws(new Exception(expectedExceptionErrorMessage));

		var exception = Assert.Throws<Exception>(() => cartRepository.AddGameToCart(game));

		Assert.Equal(expectedExceptionErrorMessage, exception.Message);
	}

	[Fact]
	public void RemoveGameFromCart_WhenCalledWithValidIdentifier_ShouldExecuteQuery()
	{
		var game = new Game { GameId = TestUserIdentifier };

		cartRepository.RemoveGameFromCart(game);

		dataLinkMock.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuery(SqlConstants.REMOVEGAMEFROMCART,
			It.IsAny<SqlParameter[]>()), Times.Once);
	}

	[Fact]
	public void RemoveGameFromCart_WhenExceptionIsThrownByDataLink_ShouldCatchExceptionAndNotThrow()
	{
		var game = new Game { GameId = TestUserIdentifier };
		var expectedExceptionErrorMessage = "Something went wrong";

		dataLinkMock
			.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
			.Throws(new Exception(expectedExceptionErrorMessage));

		var exception = Record.Exception(() => cartRepository.RemoveGameFromCart(game));

		Assert.Null(exception);
	}

	[Fact]
	public void GetUserFunds_WhenValid_ShouldReturnUserWalletBalance()
	{
		var expectedFunds = TestWalletBallance;

		var funds = cartRepository.GetUserFunds();

		Assert.Equal(expectedFunds, funds);
	}
}
