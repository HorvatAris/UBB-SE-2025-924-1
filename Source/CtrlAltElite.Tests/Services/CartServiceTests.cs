namespace SteamStore.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using CtrlAltElite.Models;
	using CtrlAltElite.ServiceProxies;
	using Moq;
	using SteamHub.ApiContract.Models.Game;
	using SteamHub.ApiContract.Models.UsersGames;
	using SteamStore.Services;
	using Xunit;

	public class CartServiceTests
	{
		private const int TestGameIdentifier = 1;
		private const int TestSecondGameIdentifier = 2;
		private const int TestGamePrice = 10;
		private const int TestSecondGamePrice = 20;

		private readonly CartService cartService;
		private readonly Mock<IUserGameServiceProxy> cartServiceProxyMock;
		private readonly Mock<IGameServiceProxy> gameServiceProxyMock;
		private readonly User testUser;

		public CartServiceTests()
		{
			cartServiceProxyMock = new Mock<IUserGameServiceProxy>();
			gameServiceProxyMock = new Mock<IGameServiceProxy>();
			testUser = new User { UserId = 1, WalletBalance = 50f };
			cartService = new CartService(cartServiceProxyMock.Object, testUser, gameServiceProxyMock.Object);
		}

		[Fact]
		public async Task GetCartGames_WhenServiceThrowsException_ShouldReturnEmptyList()
		{
			cartServiceProxyMock.Setup(proxy => proxy.GetUserCartAsync(It.IsAny<int>()))
				.ThrowsAsync(new Exception());

			var foundGames = await cartService.GetCartGames();

			Assert.Empty(foundGames);
		}

		[Fact]
		public void GetUserFunds_WhenCalled_ShouldReturnCorrectWalletBalance()
		{
			var foundWalletBalance = cartService.GetUserFunds();

			Assert.Equal(testUser.WalletBalance, foundWalletBalance);
		}

		[Fact]
		public void GetTheTotalSumOfItemsInCart_WhenMultipleGamesProvided_ShouldReturnCorrectTotalSum()
		{
			var cartGames = new List<Game>
			{
				new Game { Price = TestGamePrice },
				new Game { Price = TestGamePrice },
				new Game { Price = TestGamePrice }
			};

			var expectedTotalSum = 30f;

			var foundTotalSum = cartService.GetTheTotalSumOfItemsInCart(cartGames);

			Assert.Equal(expectedTotalSum, foundTotalSum);
		}

		[Fact]
		public async Task GetTotalSumToBePaidAsync_WhenCartContainsGames_ShouldReturnCorrectSum()
		{
			var games = new List<Game>
			{
				new Game { GameId = TestGameIdentifier, Price = TestGamePrice },
				new Game { GameId = TestSecondGameIdentifier, Price = TestSecondGamePrice }
			};
			var expectedTotalSum = 30m;

			cartServiceProxyMock.Setup(proxy => proxy.GetUserCartAsync(testUser.UserId))
				.ReturnsAsync(new GetUserGamesResponse
				{
					UserGames = new List<UserGamesResponse>
					{
						new UserGamesResponse { GameId = TestGameIdentifier },
						new UserGamesResponse { GameId = TestSecondGameIdentifier }
					}
				});

			gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(It.IsAny<int>()))
				.ReturnsAsync(new GameDetailedResponse());

			gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(TestGameIdentifier))
				.ReturnsAsync(new GameDetailedResponse
				{
					Identifier = TestGameIdentifier,
					Price = TestGamePrice
				});

			gameServiceProxyMock.Setup(proxy => proxy.GetGameByIdAsync(TestSecondGameIdentifier))
				.ReturnsAsync(new GameDetailedResponse
				{
					Identifier = TestSecondGameIdentifier,
					Price = TestSecondGamePrice
				});

			var foundTotalSum = await cartService.GetTotalSumToBePaidAsync();

			Assert.Equal(expectedTotalSum, foundTotalSum);
		}

		[Fact]
		public async Task RemoveGamesFromCart_WhenCalled_ShouldCallRemoveGameFromCartForEachGame()
		{
			var user = new User { UserId = 1 };

			var games = new List<Game>
			{
				new Game { GameId = 1 },
				new Game { GameId = 2 }
			};

			cartServiceProxyMock.Setup(proxy => proxy.RemoveFromCartAsync(It.IsAny<UserGameRequest>()))
							 .Returns(Task.CompletedTask);

			await cartService.RemoveGamesFromCart(games);

			cartServiceProxyMock.Verify(proxy => proxy.RemoveFromCartAsync(It.IsAny<UserGameRequest>()), Times.Exactly(games.Count));
		}

		[Fact]
		public void GetTheTotalSumOfItemsInCart_WhenCalled_ShouldReturnCorrectSum()
		{
			var games = new List<Game>
			{
				new Game { Price = 10.5m },
				new Game { Price = 20.0m }
			};

			var total = cartService.GetTheTotalSumOfItemsInCart(games);

			Assert.Equal(30.5f, total);
		}
	}
}
