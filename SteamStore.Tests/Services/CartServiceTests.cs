using Moq;
using SteamStore.Repositories.Interfaces;

namespace SteamStore.Tests.Services;

public class CartServiceTests
{
	private const decimal TEST_PRICE = 10.0m;
	private readonly CartService cartService;
	private readonly Mock<ICartRepository> repositoryMock;

	public CartServiceTests()
	{
		repositoryMock = new Mock<ICartRepository>();
		cartService = new CartService(repositoryMock.Object);
	}

	[Fact]
	public void GetCartGames_ShouldCallRepository()
	{
		var mockedRepositoryData = new List<Game>();

		repositoryMock.Setup(repository => repository.GetCartGames())
			.Returns(mockedRepositoryData)
			.Verifiable();

		cartService.GetCartGames();

		repositoryMock.Verify(repository => repository.GetCartGames());
	}

	[Fact]
	public void GetCartGames_ShouldReturnData()
	{
		var games = new List<Game>();

		repositoryMock.Setup(repository => repository.GetCartGames())
			.Returns(games)
			.Verifiable();

		var actualGames = cartService.GetCartGames();

		Assert.Same(games, actualGames);
	}

	[Fact]
	public void RemoveGameFromCart_ShouldCallRepository()
	{
		var game = new Game
		{
			Identifier = 1
		};

		repositoryMock.Setup(repository => repository.RemoveGameFromCart(It.IsAny<Game>()))
			.Verifiable();

		cartService.RemoveGameFromCart(game);

		repositoryMock.Verify(f => f.RemoveGameFromCart(game));
	}

	[Fact]
	public void AddGameToCart_ShouldCallRepository()
	{
		var game = new Game
		{
			Identifier = 1
		};

		repositoryMock.Setup(repository => repository.AddGameToCart(It.IsAny<Game>()))
			.Verifiable();

		cartService.AddGameToCart(game);

		repositoryMock.Verify(f => f.AddGameToCart(game));
	}

	[Fact]
	public void RemoveGamesFromCart_ShouldCallRepositoryForEach()
	{
		var games = new List<Game>
		{
			new Game
			{
				Identifier = 1
			},
			new Game
			{
				Identifier = 2
			}
		};

		repositoryMock.Setup(repository => repository.RemoveGameFromCart(It.IsAny<Game>()))
			.Verifiable();

		cartService.RemoveGamesFromCart(games);

		repositoryMock.Verify(f => f.RemoveGameFromCart(games[0]));
		repositoryMock.Verify(f => f.RemoveGameFromCart(games[1]));
	}

	[Fact]
	public void GetUserFunds_ShouldCallRepository()
	{
		repositoryMock.Setup(repository => repository.GetUserFunds())
			.Verifiable();

		cartService.GetUserFunds();

		repositoryMock.Verify(f => f.GetUserFunds());
	}

	[Fact]
	public void GetTotalSumToBePaid_ShouldCallRepository()
	{
		var games = new List<Game>
		{
			new Game
			{
				Identifier = 1,
				Price = 10
			},
			new Game
			{
				Identifier = 2,
				Price = 20
			}
		};
		repositoryMock.Setup(repository => repository.GetCartGames())
			.Returns(games)
			.Verifiable();

		cartService.GetTotalSumToBePaid();

		repositoryMock.Verify(f => f.GetCartGames());
	}

	[Fact]
	public void GetTotalSumToBePaid_ShouldReturnProperSumToBePaid()
	{
		var games = new List<Game>
		{
			new Game
			{
				Identifier = 1,
				Price = 10
			},
			new Game
			{
				Identifier = 2,
				Price = 20
			}
		};
		var expectedTotal = 30;

		repositoryMock.Setup(repository => repository.GetCartGames())
			.Returns(games);

		var result = cartService.GetTotalSumToBePaid();

		Assert.Equal(expectedTotal, result);
	}
	[Fact]
	public void GetTheTotalSumOfItemsInCart_WithMultipleGames_ReturnsCorrectTotal()
	{
        // Arrange
		float expectedResult = 30.0f;
		var cartGames = new List<Game>
		{
			new Game { Price = TEST_PRICE },
			new Game { Price = TEST_PRICE },
			new Game { Price = TEST_PRICE }
		};
		var actualResult = cartService.GetTheTotalSumOfItemsInCart(cartGames);

		Assert.Equal(expectedResult, actualResult);
	}
}
