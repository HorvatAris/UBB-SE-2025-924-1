namespace SteamStore.Tests.Services
{
	using System.Collections.Generic;
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
	}
}
