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

	public CartRepositoryTests()
	{
		dataLinkMock = new Mock<IDataLink>();
		testUser = new User
		{
			UserIdentifier = 1,
			WalletBalance = 100.50f
		};
		cartRepository = new CartRepository(dataLinkMock.Object, testUser);
	}

	[Fact]
	public void GetCartGames_ShouldReturnMappedGames_WhenDataExists()
	{
		var table = new DataTable();
		int firstRowIndex = 0;
		table.Columns.Add(SqlConstants.GAMEIDCOLUMN, typeof(int));
		table.Columns.Add(SqlConstants.NAMECOLUMN, typeof(string));
		table.Columns.Add(SqlConstants.DESCRIPTIONCOLUMN, typeof(string));
		table.Columns.Add(SqlConstants.IMAGEURLCOLUMN, typeof(string));
		table.Columns.Add(SqlConstants.PRICECOLUMN, typeof(decimal));

		var row = table.NewRow();
		row[SqlConstants.GAMEIDCOLUMN] = 1;
		row[SqlConstants.NAMECOLUMN] = "GameName";
		row[SqlConstants.DESCRIPTIONCOLUMN] = "Description";
		row[SqlConstants.IMAGEURLCOLUMN] = "image.png";
		row[SqlConstants.PRICECOLUMN] = 19.99m;
		table.Rows.Add(row);

		var expectedIdentifier = 1;
		var expectedName = "GameName";
		var expectedStatus = "Approved";

		dataLinkMock
			.Setup(d => d.ExecuteReader(SqlConstants.GetAllCartGamesProcedure, It.IsAny<SqlParameter[]>()))
			.Returns(table);

		var result = cartRepository.GetCartGames();
		var actualFirstItem = result[firstRowIndex];

		Assert.Single(result);
		Assert.Equal(expectedIdentifier, actualFirstItem.Identifier);
		Assert.Equal(expectedName, actualFirstItem.Name);
		Assert.Equal(expectedStatus, actualFirstItem.Status);
	}

	[Fact]
	public void GetCartGames_ShouldReturnEmptyList_WhenNoData()
	{
		dataLinkMock
			.Setup(d => d.ExecuteReader(SqlConstants.GetAllCartGamesProcedure, It.IsAny<SqlParameter[]>()))
			.Returns((DataTable)null);

		var result = cartRepository.GetCartGames();

		Assert.Empty(result);
	}

	[Fact]
	public void AddGameToCart_ShouldExecuteQuery()
	{
		var game = new Game { Identifier = 2 };

		cartRepository.AddGameToCart(game);

		dataLinkMock.Verify(d => d.ExecuteNonQuery(SqlConstants.AddGameToCartProcedure,
			It.IsAny<SqlParameter[]>()), Times.Once);
	}

	[Fact]
	public void AddGameToCart_ShouldThrowWrappedException_WhenDataLinkFails()
	{
		var game = new Game { Identifier = 3 };
		var expectedExceptionErrorMessage = "SQL error";

		dataLinkMock
			.Setup(d => d.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
			.Throws(new Exception(expectedExceptionErrorMessage));

		var exception = Assert.Throws<Exception>(() => cartRepository.AddGameToCart(game));

		Assert.Equal(expectedExceptionErrorMessage, exception.Message);
	}

	[Fact]
	public void RemoveGameFromCart_ShouldExecuteQuery()
	{
		var game = new Game { Identifier = 4 };

		cartRepository.RemoveGameFromCart(game);

		dataLinkMock.Verify(d => d.ExecuteNonQuery(SqlConstants.REMOVEGAMEFROMCART,
			It.IsAny<SqlParameter[]>()), Times.Once);
	}

	[Fact]
	public void RemoveGameFromCart_ShouldCatchExceptionAndNotThrow()
	{
		var game = new Game { Identifier = 5 };
		var expectedExceptionErrorMessage = "Something went wrong";

		dataLinkMock
			.Setup(d => d.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
			.Throws(new Exception(expectedExceptionErrorMessage));

		var exception = Record.Exception(() => cartRepository.RemoveGameFromCart(game));

		Assert.Null(exception);
	}

	[Fact]
	public void GetUserFunds_ShouldReturnUserWalletBalance()
	{
		var expectedFunds = 100.5f;

		var funds = cartRepository.GetUserFunds();

		Assert.Equal(expectedFunds, funds);
	}
}
