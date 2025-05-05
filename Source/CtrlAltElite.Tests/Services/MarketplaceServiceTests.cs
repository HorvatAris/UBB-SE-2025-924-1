namespace SteamStore.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamStore.Services;
    using Xunit;

    public class MarketplaceServiceTests
    {
        private readonly int testItemId = 1;
        private readonly string testItemName = "Normal Banner";
        private readonly string testItemDescription = "A Normal banner";
        private readonly float testItemPrice = 34;
        private readonly string testItemImagePath = "img";

        private readonly bool testItemListed = true;
        private readonly bool testItemNotListed = false;

        private readonly int testItemId2 = 2;
        private readonly string testItemName2 = "Cool Banner";
        private readonly string testItemDescription2 = "A Cool banner";
        private readonly float testItemPrice2 = 54;
        private readonly string testItemImagePath2 = "img2";

        private readonly int testItemId3 = 3;
        private readonly string testItemName3 = "Cold Banner";
        private readonly string testItemDescription3 = "Another cool banner, but cooler";
        private readonly float testItemPrice3 = 77;
        private readonly string testItemImagePath3 = "img3";

        private readonly MarketplaceService marketplaceService;
        private readonly Mock<IGameServiceProxy> gameServiceProxyMock;
        private readonly Mock<IUserInventoryServiceProxy> userInventoryServiceProxyMock;
        private readonly Mock<IUserServiceProxy> userServiceProxyMock;
        private readonly Mock<IItemServiceProxy> itemServiceProxyMock;

        private readonly User testUser;

        public MarketplaceServiceTests()
        {
            gameServiceProxyMock = new Mock<IGameServiceProxy>();
            userInventoryServiceProxyMock = new Mock<IUserInventoryServiceProxy>();
            userServiceProxyMock = new Mock<IUserServiceProxy>();
            itemServiceProxyMock = new Mock<IItemServiceProxy>();
            testUser = new User { UserId = 1, WalletBalance = 50f };
            marketplaceService = new MarketplaceService
            {
                GameServiceProxy = gameServiceProxyMock.Object,
                UserInventoryServiceProxy = userInventoryServiceProxyMock.Object,
                UserServiceProxy = userServiceProxyMock.Object,
                ItemServiceProxy = itemServiceProxyMock.Object,
                User = testUser,
            };
        }

        [Fact]
        public async Task GetListingsByGameAsync_ValidGame_ReturnsOnlyListedItemsForGame()
        {
            var game1 = new Game { GameId = 1, GameTitle = "Halo" };
            var game2 = new Game { GameId = 2, GameTitle = "Zelda" };

            var userInventory = new List<InventoryItemResponse>
            {
                new InventoryItemResponse
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
                new InventoryItemResponse
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
                new InventoryItemResponse
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath3,
                    GameId = game2.GameId,
                    GameName = game2.GameTitle
                },
            };

            var item1 = new ItemDetailedResponse
            {
                ItemId = testItemId,
                ItemName = testItemName,
                Description = testItemDescription,
                Price = testItemPrice,
                IsListed = testItemListed,
                ImagePath = testItemImagePath,
                GameId = game1.GameId
            };

            var item2 = new ItemDetailedResponse
            {
                ItemId = testItemId2,
                ItemName = testItemName2,
                Description = testItemDescription2,
                Price = testItemPrice2,
                IsListed = testItemNotListed,
                ImagePath = testItemImagePath2,
                GameId = game1.GameId
            };

            var item3 = new ItemDetailedResponse
            {
                ItemId = testItemId3,
                ItemName = testItemName3,
                Description = testItemDescription3,
                Price = testItemPrice3,
                IsListed = testItemListed,
                ImagePath = testItemImagePath3,
                GameId = game2.GameId
            };

            var gameResponse = new GameDetailedResponse
            {
                Name = "Halo",
                Description = "Sci-fi FPS",
                Price = 60
            };

            userInventoryServiceProxyMock
                .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
                .ReturnsAsync(new UserInventoryResponse { Items = userInventory });

            itemServiceProxyMock
                .Setup(proxy => proxy.GetItemByIdAsync(testItemId)).ReturnsAsync(item1);
            itemServiceProxyMock
                .Setup(proxy => proxy.GetItemByIdAsync(testItemId2)).ReturnsAsync(item2);
            itemServiceProxyMock
                .Setup(proxy => proxy.GetItemByIdAsync(testItemId3)).ReturnsAsync(item3);

            gameServiceProxyMock
                .Setup(proxy => proxy.GetGameByIdAsync(testItemId)).ReturnsAsync(gameResponse);

            var result = await marketplaceService.GetListingsByGameAsync(game1, testUser.UserId);

            Assert.Single(result);
            var returnedItem = result.First();
            Assert.Equal(1, returnedItem.ItemId);
            Assert.Equal("Normal Banner", returnedItem.ItemName);
            Assert.Equal("Halo", returnedItem.Game.GameTitle);
        }

        [Fact]
        public async Task GetListingsByGameAsync_NoMatchingItems_ReturnsEmptyList()
        {
            var game99 = new Game { GameId = 99, GameTitle = "Nonexistent Game" };
            var game1 = new Game { GameId = 1, GameTitle = "Halo" };

            var userInventory = new List<InventoryItemResponse>
            {
                new InventoryItemResponse
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
                new InventoryItemResponse
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
            };

            var item1 = new ItemDetailedResponse
            {
                ItemId = testItemId,
                IsListed = false,
                GameId = game1.GameId
            };

            var item2 = new ItemDetailedResponse
            {
                ItemId = testItemId2,
                IsListed = true,
                GameId = game1.GameId
            };

            userInventoryServiceProxyMock
                .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
                .ReturnsAsync(new UserInventoryResponse { Items = userInventory });

            itemServiceProxyMock.Setup(proxy => proxy.GetItemByIdAsync(testItemId)).ReturnsAsync(item1);
            itemServiceProxyMock.Setup(proxy => proxy.GetItemByIdAsync(testItemId2)).ReturnsAsync(item2);

            var result = await marketplaceService.GetListingsByGameAsync(game99, testUser.UserId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task BuyItemAsync_ItemNotListed_ThrowsInvalidOperationException()
        {
            var item = new Item
            {
                ItemId = testItemId,
                ItemName = testItemName,
                Description = testItemDescription,
                Price = testItemPrice,
                IsListed = testItemNotListed,
                ImagePath = testItemImagePath
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                marketplaceService.BuyItemAsync(item, testUser.UserId));
        }

        [Fact]
        public async Task BuyItemAsync_ValidItem_ExecutesExpectedOperations()
        {
            var game1 = new Game { GameId = 1, GameTitle = "Halo" };

            var itemToBuy = new Item
            {
                ItemId = testItemId,
                ItemName = testItemName,
                Description = testItemDescription,
                Price = testItemPrice,
                IsListed = testItemListed,
                ImagePath = testItemImagePath,
                Game = game1
            };

            var userInventory = new List<InventoryItemResponse>
            {
                new InventoryItemResponse
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
            };

            userInventoryServiceProxyMock
               .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
               .ReturnsAsync(new UserInventoryResponse { Items = userInventory });

            userInventoryServiceProxyMock
                .Setup(items => items.RemoveItemFromUserInventoryAsync(It.IsAny<ItemFromInventoryRequest>()))
                .Returns(Task.CompletedTask);

            userInventoryServiceProxyMock
                .Setup(items => items.AddItemToUserInventoryAsync(It.IsAny<ItemFromInventoryRequest>()))
                .Returns(Task.CompletedTask);

            itemServiceProxyMock
                .Setup(item => item.UpdateItemAsync(testItemId, It.IsAny<UpdateItemRequest>()))
                .Returns(Task.CompletedTask);

            var result = await marketplaceService.BuyItemAsync(itemToBuy, testUser.UserId);

            Assert.True(result);

            userInventoryServiceProxyMock.Verify(item =>
                item.RemoveItemFromUserInventoryAsync(It.IsAny<ItemFromInventoryRequest>()), Times.Once);

            userInventoryServiceProxyMock.Verify(item =>
                item.AddItemToUserInventoryAsync(It.IsAny<ItemFromInventoryRequest>()), Times.Once);

            itemServiceProxyMock.Verify(item =>
                item.UpdateItemAsync(testItemId, It.IsAny<UpdateItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task BuyItemAsync_NullItem_ThrowsArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                marketplaceService.BuyItemAsync(null, testUser.UserId));

            Assert.Equal("item", exception.ParamName);
        }

        [Fact]
        public async Task GetListingsByGameAsync_NullGame_ThrowsArgumentNullException()
        {
            string parameterGame = "game";

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                marketplaceService.GetListingsByGameAsync(null, testUser.UserId));

            Assert.Equal(parameterGame, exception.ParamName);
        }
    }
}
