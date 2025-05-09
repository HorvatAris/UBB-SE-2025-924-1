namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Proxies;
    using SteamHub.ApiContract.Services;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.ItemTrade;
    using SteamHub.ApiContract.Models.ItemTradeDetails;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserInventory;
    using Xunit;

    public class TradeServiceTests
    {
        private readonly TradeService tradeService;
        private readonly Mock<ItemTradeRepositoryProxy> itemTradeServiceMock;
        private readonly Mock<ItemTradeDetailsRepositoryProxy> itemTradeDetailServiceMock;
        private readonly Mock<UserRepositoryProxy> userServiceMock;
        private readonly Mock<GameRepositoryProxy> gameServiceMock;
        private readonly Mock<ItemRepositoryProxy> itemServiceMock;
        private readonly Mock<UserInventoryRepositoryProxy> userInventoryServiceMock;
        private readonly User testUser;

        public TradeServiceTests()
        {
            itemTradeServiceMock = new Mock<ItemTradeRepositoryProxy>();
            itemTradeDetailServiceMock = new Mock<ItemTradeDetailsRepositoryProxy>();
            userServiceMock = new Mock<UserRepositoryProxy>();
            gameServiceMock = new Mock<GameRepositoryProxy>();
            itemServiceMock = new Mock<ItemRepositoryProxy>();
            userInventoryServiceMock = new Mock<UserInventoryRepositoryProxy>();

            testUser = new User { UserId = 1, UserName = "TestUser" };
            tradeService = new TradeService(
                itemTradeServiceMock.Object,
                testUser,
                itemTradeDetailServiceMock.Object,
                userServiceMock.Object,
                gameServiceMock.Object,
                itemServiceMock.Object,
                userInventoryServiceMock.Object);
        }

        [Fact]
        public void GetCurrentUser_ValidExistingUser_ShouldReturnUser()
        {
            var user = tradeService.GetCurrentUser();
            Assert.Equal(testUser, user);
        }

        [Fact]
        public async Task MarkTradeAsCompleted_ValidTradeId_ShouldCallUpdateTrade()
        {
            var tradeId = 123;
            await tradeService.MarkTradeAsCompletedAsync(tradeId);

            itemTradeServiceMock.Verify(proxy => proxy.UpdateItemTradeAsync(tradeId, It.Is<UpdateItemTradeRequest>(request =>
                request.TradeStatus == TradeStatusEnum.Completed &&
                request.AcceptedBySourceUser == true &&
                request.AcceptedByDestinationUser == true)), Times.Once);
        }

        [Fact]
        public async Task TransferItemAsync_ValidTransferDetails_ShouldCallAddAndRemove()
        {
            var itemId = 10;
            var fromUserId = 1;
            var toUserId = 2;
            var gameId = 3;

            await tradeService.TransferItemAsync(itemId, fromUserId, toUserId, gameId);

            userInventoryServiceMock.Verify(proxy => proxy.RemoveItemFromUserInventoryAsync(It.Is<ItemFromInventoryRequest>(request =>
                request.UserId == fromUserId && request.ItemId == itemId && request.GameId == gameId)), Times.Once);

            userInventoryServiceMock.Verify(proxy => proxy.AddItemToUserInventoryAsync(It.Is<ItemFromInventoryRequest>(request =>
                request.UserId == toUserId && request.ItemId == itemId && request.GameId == gameId)), Times.Once);
        }

        [Fact]
        public async Task AddItemTradeAsync_ValidTradeDetails_ShouldCreateTradeAndDetails()
        {
            var trade = new ItemTrade
            {
                SourceUser = new User { UserId = 1 },
                DestinationUser = new User { UserId = 2 },
                GameOfTrade = new Game { GameId = 100 },
                TradeDate = DateTime.Now,
                TradeDescription = "Test trade",
                SourceUserItems = new List<Item> { new Item { ItemId = 101 } },
                DestinationUserItems = new List<Item> { new Item { ItemId = 202 } },
            };

            itemTradeServiceMock.Setup(proxy => proxy.CreateItemTradeAsync(It.IsAny<CreateItemTradeRequest>()))
                .ReturnsAsync(new CreateItemTradeResponse { TradeId = 999 });

            await tradeService.AddItemTradeAsync(trade);

            Assert.Equal(999, trade.TradeId);

            itemTradeDetailServiceMock.Verify(proxy => proxy.CreateItemTradeDetailAsync(
                It.Is<CreateItemTradeDetailRequest>(request => request.ItemId == 101 && request.IsSourceUserItem)), Times.Once);

            itemTradeDetailServiceMock.Verify(proxy => proxy.CreateItemTradeDetailAsync(
                It.Is<CreateItemTradeDetailRequest>(request => request.ItemId == 202 && !request.IsSourceUserItem)), Times.Once);
        }

        [Fact]
        public async Task CreateTradeAsync_ValidTradeId_ShouldCallAddItemTrade()
        {
            var trade = new ItemTrade
            {
                SourceUser = new User { UserId = 1 },
                DestinationUser = new User { UserId = 2 },
                GameOfTrade = new Game { GameId = 100 },
                TradeDate = DateTime.Now,
                TradeDescription = "Test trade",
                SourceUserItems = new List<Item>(),
                DestinationUserItems = new List<Item>(),
            };

            itemTradeServiceMock.Setup(proxy => proxy.CreateItemTradeAsync(It.IsAny<CreateItemTradeRequest>()))
                .ReturnsAsync(new CreateItemTradeResponse { TradeId = 1 });

            await tradeService.CreateTradeAsync(trade);

            Assert.Equal(1, trade.TradeId);
        }

        [Fact]
        public async Task UpdateTradeAsync_ValidTradeId_ShouldCallUpdateItemTradeAsync()
        {
            var trade = new ItemTrade
            {
                TradeId = 5,
                AcceptedByDestinationUser = true,
                AcceptedBySourceUser = true,
                SourceUser = new User { UserId = 1 },
                DestinationUser = new User { UserId = 2 },
                GameOfTrade = new Game { GameId = 99 },
            };

            itemTradeDetailServiceMock.Setup(detailServiceProxy => detailServiceProxy.GetItemTradeDetailsAsync()).ReturnsAsync(new GetItemTradeDetailsResponse { ItemTradeDetails = new List<ItemTradeDetailResponse>() });

            await tradeService.UpdateTradeAsync(trade);

            itemTradeServiceMock.Verify(proxy => proxy.UpdateItemTradeAsync(trade.TradeId, It.IsAny<UpdateItemTradeRequest>()), Times.Once);
        }

        [Fact]
        public async Task AcceptTradeAsync_SourceUserAccepts_ShouldUpdateTrade()
        {
            var trade = new ItemTrade
            {
                TradeId = 1,
                AcceptedBySourceUser = false,
                AcceptedByDestinationUser = true,
                SourceUser = new User { UserId = 1 },
                DestinationUser = new User { UserId = 2 },
                GameOfTrade = new Game { GameId = 10 },
                SourceUserItems = new List<Item>(),
                DestinationUserItems = new List<Item>()
            };

            itemTradeDetailServiceMock.Setup(detailServiceProxy => detailServiceProxy.GetItemTradeDetailsAsync()).ReturnsAsync(new GetItemTradeDetailsResponse { ItemTradeDetails = new List<ItemTradeDetailResponse>() });

            await tradeService.AcceptTradeAsync(trade, true);

            Assert.True(trade.AcceptedBySourceUser);
        }

        [Fact]
        public async Task GetUserInventoryAsync_WithExistingItems_ShouldReturnItems()
        {
            var gameTitle = "GameX";

            userInventoryServiceMock.Setup(proxy => proxy.GetUserInventoryAsync(It.IsAny<int>()))
                .ReturnsAsync(new UserInventoryResponse
                {
                    Items = new List<InventoryItemResponse>
                    {
                        new InventoryItemResponse { ItemId = 1, ItemName = "Item1", Description = "Desc", Price = 10, IsListed = false, GameName = gameTitle }
                    }
                });

            gameServiceMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(new List<GameDetailedResponse> { new GameDetailedResponse { Name = gameTitle, Identifier = 10 } });

            var result = await tradeService.GetUserInventoryAsync(testUser.UserId);

            Assert.Single(result);
            Assert.Equal("Item1", result[0].ItemName);
        }
    }
}
