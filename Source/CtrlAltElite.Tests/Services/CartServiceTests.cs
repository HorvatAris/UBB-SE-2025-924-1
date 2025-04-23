using Moq;
using SteamStore.Repositories.Interfaces;

namespace SteamStore.Tests.Services;

public class CartServiceTests
{
	private readonly CartService cartService;
	private readonly Mock<ICartRepository> repositoryMock;

	private const int TestGameIdentifier = 1;
	private const int TestSecondGameIdentifier = 2;
	private const int TestGamePrice = 10;
	private const int TestSecondGamePrice = 20;

	public CartServiceTests()
	{
		repositoryMock = new Mock<ICartRepository>();
		cartService = new CartService(repositoryMock.Object);
	}

	[Fact]
	public void GetCartGames_WhenCalled_ShouldCallRepositoryGetCartGames()
	{
		var mockedRepositoryData = new List<Game>();

		repositoryMock.Setup(repositoryMock => repositoryMock.GetCartGames())
			.Returns(mockedRepositoryData)
			.Verifiable();

		cartService.GetCartGames();

		repositoryMock.Verify(repositoryMock => repositoryMock.GetCartGames());
	}

	[Fact]
	public void GetCartGames_WhenCalled_ShouldReturnData()
	{
		var games = new List<Game>();

		repositoryMock.Setup(repositoryMock => repositoryMock.GetCartGames())
			.Returns(games)
			.Verifiable();

		var actualGames = cartService.GetCartGames();

		Assert.Same(games, actualGames);
	}

	[Fact]
	public void RemoveGameFromCart_WhenCalledWithValidGame_ShouldCallRepositoryRemoveGameFromCart()
	{
		var game = new Game
		{
			Identifier = TestGameIdentifier
		};

		repositoryMock.Setup(repositoryMock => repositoryMock.RemoveGameFromCart(It.IsAny<Game>()))
			.Verifiable();

		cartService.RemoveGameFromCart(game);

		repositoryMock.Verify(repositoryMock => repositoryMock.RemoveGameFromCart(game));
	}

	[Fact]
	public void AddGameToCart_WhenCalledWithValidGame_ShouldCallRepositoryAddGameToCart()
	{
		var game = new Game
		{
			Identifier = TestGameIdentifier
		};

		repositoryMock.Setup(repositoryMock => repositoryMock.AddGameToCart(It.IsAny<Game>()))
			.Verifiable();

		cartService.AddGameToCart(game);

		repositoryMock.Verify(repositoryMock => repositoryMock.AddGameToCart(game));
	}

	[Fact]
	public void RemoveGamesFromCart_ShouldCallRepositoryForEach()
	{
		var games = new List<Game>
		{
			new Game
			{
				Identifier = TestGameIdentifier
			},
			new Game
			{
				Identifier = TestSecondGameIdentifier
			}
		};

		var expectedRemoveCallsCount = 2;

		repositoryMock.Setup(repositoryMock => repositoryMock.RemoveGameFromCart(It.IsAny<Game>()))
			.Verifiable();

		cartService.RemoveGamesFromCart(games);

		repositoryMock.Verify(repositoryMock => repositoryMock.RemoveGameFromCart(It.IsAny<Game>()), Times.Exactly(expectedRemoveCallsCount));
	}

	[Fact]
	public void GetUserFunds_WhenCalled_ShouldCallRepositoryGetUserFunds()
	{
		repositoryMock.Setup(repositoryMock => repositoryMock.GetUserFunds())
			.Verifiable();

		cartService.GetUserFunds();

		repositoryMock.Verify(repositoryMock => repositoryMock.GetUserFunds());
	}

	[Fact]
	public void GetTotalSumToBePaid_WhenCalled_ShouldCallRepositoryGetCartGames()
	{
		var games = new List<Game>
		{
			new Game
			{
				Identifier = TestGameIdentifier,
				Price = TestGamePrice
			},
			new Game
			{
				Identifier = TestSecondGameIdentifier,
				Price = TestSecondGamePrice
			}
		};
		repositoryMock.Setup(repositoryMock => repositoryMock.GetCartGames())
			.Returns(games)
			.Verifiable();

		cartService.GetTotalSumToBePaid();

		repositoryMock.Verify(repositoryMock => repositoryMock.GetCartGames());
	}

	[Fact]
	public void GetTotalSumToBePaid_WhenCalledWithTwoGamesWithPrices_ShouldReturnProperSumToBePaid()
	{
		var games = new List<Game>
		{
			new Game
			{
				Identifier = TestGameIdentifier,
				Price = TestGamePrice
			},
			new Game
			{
				Identifier = TestSecondGameIdentifier,
				Price = TestSecondGamePrice
			}
		};
		var expectedTotal = 30;

		repositoryMock.Setup(repositoryMock => repositoryMock.GetCartGames())
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
			new Game { Price = TestGamePrice },
			new Game { Price = TestGamePrice },
			new Game { Price = TestGamePrice }
		};
		var expectedTotalPrice = 30f;

		var result = cartService.GetTheTotalSumOfItemsInCart(cartGames);

		Assert.Equal(expectedTotalPrice, result);
	}
}
