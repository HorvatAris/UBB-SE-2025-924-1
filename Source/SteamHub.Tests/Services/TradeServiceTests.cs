namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Services;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.ItemTrade;
    using SteamHub.ApiContract.Models.ItemTradeDetails;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserInventory;
    using Xunit;
    using SteamHub.ApiContract.Repositories;
    public class TradeServiceTests
    {
        private readonly TradeService tradeService;
        
        private readonly Mock<IItemTradeRepository> itemTradeRepositoryMock;
        private readonly Mock<IItemTradeDetailRepository> itemTradeDetailRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IGameRepository> gameRepositoryMock;
        private readonly Mock<IItemRepository> itemRepositoryMock;
        private readonly Mock<IUserInventoryRepository> userInventoryRepositoryMock;
        private readonly User testUser;

        public TradeServiceTests()
        {
            itemTradeRepositoryMock = new Mock<IItemTradeRepository>();
            itemTradeDetailRepositoryMock = new Mock<IItemTradeDetailRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            gameRepositoryMock = new Mock<IGameRepository>();
            itemRepositoryMock = new Mock<IItemRepository>();
            userInventoryRepositoryMock = new Mock<IUserInventoryRepository>();

            testUser = new User { UserId = 1, UserName = "TestUser" };
            tradeService = new TradeService(
                itemTradeRepositoryMock.Object,
                testUser,
                itemTradeDetailRepositoryMock.Object,
                userRepositoryMock.Object,
                gameRepositoryMock.Object,
                itemRepositoryMock.Object,
                userInventoryRepositoryMock.Object);
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

            itemTradeRepositoryMock.Verify(proxy => proxy.UpdateItemTradeAsync(tradeId, It.Is<UpdateItemTradeRequest>(request =>
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

            userInventoryRepositoryMock.Verify(proxy => proxy.RemoveItemFromUserInventoryAsync(It.Is<ItemFromInventoryRequest>(request =>
                request.UserId == fromUserId && request.ItemId == itemId && request.GameId == gameId)), Times.Once);

            userInventoryRepositoryMock.Verify(proxy => proxy.AddItemToUserInventoryAsync(It.Is<ItemFromInventoryRequest>(request =>
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

            itemTradeRepositoryMock.Setup(proxy => proxy.CreateItemTradeAsync(It.IsAny<CreateItemTradeRequest>()))
                .ReturnsAsync(new CreateItemTradeResponse { TradeId = 999 });

            await tradeService.AddItemTradeAsync(trade);

            Assert.Equal(999, trade.TradeId);

            itemTradeDetailRepositoryMock.Verify(proxy => proxy.CreateItemTradeDetailAsync(
                It.Is<CreateItemTradeDetailRequest>(request => request.ItemId == 101 && request.IsSourceUserItem)), Times.Once);

            itemTradeDetailRepositoryMock.Verify(proxy => proxy.CreateItemTradeDetailAsync(
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

            itemTradeRepositoryMock.Setup(proxy => proxy.CreateItemTradeAsync(It.IsAny<CreateItemTradeRequest>()))
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

            itemTradeDetailRepositoryMock.Setup(detailServiceProxy => detailServiceProxy.GetItemTradeDetailsAsync()).ReturnsAsync(new GetItemTradeDetailsResponse { ItemTradeDetails = new List<ItemTradeDetailResponse>() });

            await tradeService.UpdateTradeAsync(trade);

            itemTradeRepositoryMock.Verify(proxy => proxy.UpdateItemTradeAsync(trade.TradeId, It.IsAny<UpdateItemTradeRequest>()), Times.Once);
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

            itemTradeDetailRepositoryMock.Setup(detailServiceProxy => detailServiceProxy.GetItemTradeDetailsAsync()).ReturnsAsync(new GetItemTradeDetailsResponse { ItemTradeDetails = new List<ItemTradeDetailResponse>() });

            await tradeService.AcceptTradeAsync(trade, true);

            Assert.True(trade.AcceptedBySourceUser);
        }

        [Fact]
        public async Task GetUserInventoryAsync_WithExistingItems_ShouldReturnItems()
        {
            var gameTitle = "GameX";

            userInventoryRepositoryMock.Setup(proxy => proxy.GetUserInventoryAsync(It.IsAny<int>()))
                .ReturnsAsync(new UserInventoryResponse
                {
                    Items = new List<InventoryItemResponse>
                    {
                        new InventoryItemResponse { ItemId = 1, ItemName = "Item1", Description = "Desc", Price = 10, IsListed = false, GameName = gameTitle }
                    }
                });

            gameRepositoryMock.Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(new List<GameDetailedResponse> { new GameDetailedResponse { Name = gameTitle, Identifier = 10 } });

            var result = await tradeService.GetUserInventoryAsync(testUser.UserId);

            Assert.Single(result);
            Assert.Equal("Item1", result[0].ItemName);
        }
    }
}
