namespace SteamStore.Tests.Services
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Threading.Tasks;
	using CtrlAltElite.ServiceProxies;
	using CtrlAltElite.Services;
	using Moq;
	using SteamHub.ApiContract.Models.Game;
	using SteamHub.ApiContract.Models.Tag;
	using SteamStore.Models;
	using SteamStore.Services;
	using SteamStore.Tests.TestUtils;
	using Xunit;

	public class GameServiceTest
	{
		private const string TEST_TAG_1 = "tag1";
		private const string TEST_TAG_2 = "tag2";
		private const string TEST_NAME = "Test";
		private const string NOT_MATCH_NAME = "NoMatch";
		private const string TEST_GAME_1 = "test Game 1";
		private const string TEST_GAME_2 = "TEST Game 2";
		private const string TEST_GAME_3 = "Game 2";
		private readonly GameService subject;
		private readonly Mock<IGameServiceProxy> gameProxyMock;
		private readonly Mock<ITagServiceProxy> tagProxyMock;

		public GameServiceTest()
		{
			gameProxyMock = new Mock<IGameServiceProxy>();
			tagProxyMock = new Mock<ITagServiceProxy>();
			subject = new GameService { GameServiceProxy = gameProxyMock.Object, TagServiceProxy = tagProxyMock.Object };
		}

		[Fact]
		public async Task SearchGames_WhenQueryMatchesGames_ShouldReturnMatchingGames()
		{
			var expectedGame1 = new Game { GameTitle = TEST_GAME_1 };
			var expectedGame2 = new Game { GameTitle = TEST_GAME_2 };
			var excludedGame = new Game { GameTitle = TEST_GAME_3 };
			var allGames = new List<GameDetailedResponse>
			{
				new GameDetailedResponse() { Name = TEST_GAME_1 },
				new GameDetailedResponse() { Name = TEST_GAME_2 },
				new GameDetailedResponse() { Name = TEST_GAME_3 }
			};
			gameProxyMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
				.ReturnsAsync(allGames);

			var actualGames = await subject.SearchGames(TEST_NAME);

			var expectedGames = new[]
			{
				GameMapper.MapToGame(allGames[0]),
				GameMapper.MapToGame(allGames[1])
			};

			AssertUtils.AssertContainsEquivalent(actualGames, expectedGames);
		}

		[Fact]
		public async Task SearchGames_WhenQueryDoesNotMatchAnyGames_ShouldReturnEmptyList()
		{
			var allGames = new List<GameDetailedResponse>
			{
				new GameDetailedResponse() { Name = TEST_GAME_1 },
				new GameDetailedResponse() { Name = TEST_GAME_2 },
				new GameDetailedResponse() { Name = TEST_GAME_3 }
			};
			gameProxyMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
				.ReturnsAsync(allGames);

			var foundGames = await subject.SearchGames(NOT_MATCH_NAME);

			Assert.Empty(foundGames);
		}

		[Fact]
		public async Task GetAllTags_WhenCalled_ShouldReturnMappedTags()
		{
			var apiTags = new GetTagsResponse
			{
				Tags = new List<TagSummaryResponse>
				{
					new TagSummaryResponse() { TagName = TEST_TAG_1 },
					new TagSummaryResponse() { TagName = TEST_TAG_2 }
				}
			};
			var expectedTags = new Tag[]
			{
				new Tag { Tag_name = TEST_TAG_1 },
				new Tag { Tag_name = TEST_TAG_2 }
			};
			tagProxyMock.Setup(proxy => proxy.GetAllTagsAsync())
				.ReturnsAsync(apiTags);

			var actualTags = await subject.GetAllTags();

			AssertUtils.AssertContainsEquivalent(actualTags, expectedTags);
		}

		[Fact]
		public async Task GetAllGameTags_WhenGameHasMatchingTags_ShouldReturnOnlyMatchingTags()
		{
			var game = new Game { Tags = new[] { TEST_TAG_1, TEST_TAG_2 } };
			var apiTags = new GetTagsResponse
			{
				Tags = new List<TagSummaryResponse>
				{
					new TagSummaryResponse() { TagName = TEST_TAG_1 },
					new TagSummaryResponse() { TagName = "UnrelatedTag" }
				}
			};
			var expectedTags = new Tag[]
			{
				new Tag { Tag_name = TEST_TAG_1 }
			};
			tagProxyMock.Setup(proxy => proxy.GetAllTagsAsync())
				.ReturnsAsync(apiTags);

			var actualTags = await subject.GetAllGameTags(game);

			AssertUtils.AssertContainsEquivalent(actualTags, expectedTags);
		}

		[Fact]
		public async Task FilterGames_WhenCalledWithMatchingCriteria_ShouldReturnFilteredGames()
		{
			var game1 = new Game { Rating = 5, Price = 20, Tags = new[] { TEST_TAG_1 } };
			var game2 = new Game { Rating = 4, Price = 25, Tags = new[] { TEST_TAG_1, TEST_TAG_2 } };
			var game3 = new Game { Rating = 3, Price = 15, Tags = new[] { "otherTag" } };
			var games = new Collection<Game> { game1, game2, game3 };

			gameProxyMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
				.ReturnsAsync(new List<GameDetailedResponse>
				{
					new GameDetailedResponse
					{
						Name = game1.GameTitle, Rating = game1.Rating, Price = game1.Price,
						Tags = new List<TagDetailedResponse>
						{
							new TagDetailedResponse()
							{
								TagName = TEST_TAG_1
							}
						}
					},
					new GameDetailedResponse
					{
						Name = game2.GameTitle, Rating = game2.Rating, Price = game2.Price,
						Tags = new List<TagDetailedResponse>
						{
							new TagDetailedResponse()
							{
								TagName = TEST_TAG_1
							},
							new TagDetailedResponse()
							{
								TagName = TEST_TAG_2
							}
						}
					},
					new GameDetailedResponse
					{
						Name = game3.GameTitle, Rating = game3.Rating, Price = game3.Price,
						Tags = new List<TagDetailedResponse>
						{
							new TagDetailedResponse()
							{
								TagName = "otherTag"
							}
						}
					}
				});

			var result = await subject.FilterGames(4, 10, 30, new[] { TEST_TAG_1 });

			Assert.True(result.All(g => g.Tags.Contains(TEST_TAG_1)));
		}

		[Fact]
		public async Task GetTrendingGames_WhenCalled_ShouldReturnTopTrendingGames()
		{
			var game1 = new Game { GameId = 1, GameTitle = "Game1", Status = "Approved", NumberOfRecentPurchases = 5, TrendingScore = 0.5m, TagScore = Game.NOTCOMPUTED };
			var game2 = new Game { GameId = 2, GameTitle = "Game2", Status = "Approved", NumberOfRecentPurchases = 10, TrendingScore = 1, TagScore = Game.NOTCOMPUTED };
			var game3 = new Game { GameId = 3, GameTitle = "Game3", Status = "Rejected", NumberOfRecentPurchases = 1, TagScore = Game.NOTCOMPUTED };
			var approvedGames = new Collection<Game> { game1, game2, game3 };

			var expectedResult = new Game[]
			{
				game2, game1
			};

			gameProxyMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
				.ReturnsAsync(approvedGames.Select(game => new GameDetailedResponse
				{
					Name = game.GameTitle,
					Identifier = game.GameId,
					Status = game.Status == "Approved" ? GameStatusEnum.Approved : GameStatusEnum.Rejected,
					NumberOfRecentPurchases = game.NumberOfRecentPurchases
				}).ToList());

			var trendingGames = await subject.GetTrendingGames();

			AssertUtils.AssertContainsEquivalent(trendingGames, expectedResult);
		}

		[Fact]
		public async Task GetDiscountedGames_WhenGamesHaveDiscounts_ShouldReturnOnlyDiscounted()
		{
			var game1 = new Game { GameId = 1, GameTitle = "Game1", Status = "Approved", Discount = 5, TagScore = Game.NOTCOMPUTED };
			var game2 = new Game { GameId = 2, GameTitle = "Game2", Status = "Approved", Discount = 0 };

			var expectedGames = new Game[]
			{
				game1
			};

			gameProxyMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
				.ReturnsAsync(new List<GameDetailedResponse>
				{
					new GameDetailedResponse { Identifier = game1.GameId, Name = game1.GameTitle, Status = GameStatusEnum.Approved, Discount = 5 },
					new GameDetailedResponse { Identifier = game2.GameId, Name = game2.GameTitle, Status = GameStatusEnum.Approved, Discount = 0 }
				});

			var discountedGames = await subject.GetDiscountedGames();

			AssertUtils.AssertContainsEquivalent(discountedGames, expectedGames);
		}

		[Fact]
		public async Task GetSimilarGames_WhenCalledWithGameId_ShouldReturnOtherGames()
		{
			var game1 = new Game { GameId = 1, GameTitle = "Game1", Status = "Approved", TagScore = Game.NOTCOMPUTED };
			var game2 = new Game { GameId = 2, GameTitle = "Game2", Status = "Approved", TagScore = Game.NOTCOMPUTED };
			var game3 = new Game { GameId = 3, GameTitle = "Game3", Status = "Approved", TagScore = Game.NOTCOMPUTED };

			var expectedGames = new Game[]
			{
				game3, game2
			};

			gameProxyMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
				.ReturnsAsync(new List<GameDetailedResponse>
				{
					new GameDetailedResponse { Identifier = game1.GameId, Name = game1.GameTitle, Status = GameStatusEnum.Approved },
					new GameDetailedResponse { Identifier = game2.GameId, Name = game2.GameTitle, Status = GameStatusEnum.Approved },
					new GameDetailedResponse { Identifier = game3.GameId, Name = game3.GameTitle, Status = GameStatusEnum.Approved }
				});

			var similarGames = await subject.GetSimilarGames(1);

			Assert.Equal(similarGames.Count, expectedGames.Count());
		}

		[Fact]
		public async Task GetGameById_WhenCalled_ShouldReturnMappedGame()
		{
			var gameId = 42;
			var detailedResponse = new GameDetailedResponse { Identifier = gameId, Name = "Sample Game" };

			gameProxyMock.Setup(proxy => proxy.GetGameByIdAsync(gameId))
				.ReturnsAsync(detailedResponse);

			var result = await subject.GetGameById(gameId);

			Assert.Equal(gameId, result.GameId);
			Assert.Equal("Sample Game", result.GameTitle);
		}
	}
}
