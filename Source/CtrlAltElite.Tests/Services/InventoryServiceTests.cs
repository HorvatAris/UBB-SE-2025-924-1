namespace SteamStore.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamStore.Services;
    using SteamStore.Utils;
    using Xunit;

    public class InventoryServiceTests
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

        private readonly InventoryService inventoryService;
        private readonly Mock<IUserInventoryServiceProxy> userInventoryServiceProxyMock;
        private readonly Mock<IItemServiceProxy> itemServiceProxyMock;
        private readonly Mock<IGameServiceProxy> gameServiceProxyMock;

        private readonly InventoryValidator inventoryValidator;

        private readonly User testUser;

        public InventoryServiceTests()
        {
            userInventoryServiceProxyMock = new Mock<IUserInventoryServiceProxy>();
            itemServiceProxyMock = new Mock<IItemServiceProxy>();
            gameServiceProxyMock = new Mock<IGameServiceProxy>();
            testUser = new User { UserId = 1, WalletBalance = 50f };
            inventoryService = new InventoryService(userInventoryServiceProxyMock.Object, itemServiceProxyMock.Object, gameServiceProxyMock.Object, testUser);
            inventoryValidator = new InventoryValidator();
        }

        [Fact]
        public async Task SellItemAsync_WhenItemIsValidAndUpdateSucceeds_ReturnsTrue()
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

            itemServiceProxyMock.Setup(proxy => proxy.GetItemsAsync()).ReturnsAsync(new List<ItemDetailedResponse>
            {
                new ItemDetailedResponse
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Description = item.Description,
                    Price = item.Price,
                    IsListed = item.IsListed,
                    ImagePath = item.ImagePath
                },
                new ItemDetailedResponse
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath2
                }
            });

            itemServiceProxyMock.Setup(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>())).Returns(Task.CompletedTask);

            var result = await inventoryService.SellItemAsync(item);

            Assert.True(result);
            Assert.True(item.IsListed);
            itemServiceProxyMock.Verify(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task SellItemAsync_WhenUpdateFails_ReturnsFalse()
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

            itemServiceProxyMock.Setup(proxy => proxy.GetItemsAsync()).ReturnsAsync(new List<ItemDetailedResponse>
            {
                new ItemDetailedResponse
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Description = item.Description,
                    Price = item.Price,
                    IsListed = item.IsListed,
                    ImagePath = item.ImagePath
                },
                new ItemDetailedResponse
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath2
                }
            });

            itemServiceProxyMock.Setup(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>()))
                                .ThrowsAsync(new Exception("Update failed"));

            var result = await inventoryService.SellItemAsync(item);

            Assert.False(result);
        }

        [Fact]
        public async Task SellItemAsync_WhenItemNotFound_ReturnsFalse()
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

            itemServiceProxyMock.Setup(proxy => proxy.GetItemsAsync()).ReturnsAsync(new List<ItemDetailedResponse>
            {
                new ItemDetailedResponse
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath2
                }
            });

            itemServiceProxyMock.Setup(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>())).Returns(Task.CompletedTask);

            var result = await inventoryService.SellItemAsync(item);

            Assert.False(result);
        }

        [Fact]
        public void FilterInventoryItems_NullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                inventoryService.FilterInventoryItems(null, null, null));
        }

        [Fact]
        public void FilterInventoryItems_OnlyReturnsUnlistedItems()
        {
            var items = new List<Item>
            {
                new Item
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath
                },
                new Item
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2
                },
                new Item
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath3
                },
            };

            var result = inventoryService.FilterInventoryItems(items, null, null);

            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.False(item.IsListed));
        }

        [Fact]
        public void FilterInventoryItems_FiltersBySelectedGame()
        {
            var game1 = new Game { GameTitle = "Zelda" };
            var game2 = new Game { GameTitle = "Halo" };

            var items = new List<Item>
            {
                new Item
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath3,
                    Game = game2
                },
            };

            var selectedGame = game1;

            var result = inventoryService.FilterInventoryItems(items, selectedGame, null);

            Assert.Single(result);
            Assert.All(result, item => Assert.Equal("Zelda", item.Game.GameTitle, ignoreCase: true));
        }

        [Fact]
        public void FilterInventoryItems_AllGames_NotFilteredByGameName()
        {
            var game1 = new Game { GameTitle = "Zelda" };
            var game2 = new Game { GameTitle = "Halo" };

            var items = new List<Item>
            {
                new Item
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath3,
                    Game = game2
                },
            };

            var selectedGame = new Game { GameTitle = "All Games" };

            var result = inventoryService.FilterInventoryItems(items, selectedGame, null);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void FilterInventoryItems_FiltersBySearchText()
        {
            var game1 = new Game { GameTitle = "Zelda" };
            var game2 = new Game { GameTitle = "Halo" };

            var items = new List<Item>
            {
                new Item
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath3,
                    Game = game2
                },
            };

            var result = inventoryService.FilterInventoryItems(items, null, "cool");

            Assert.Equal(2, result.Count);
            Assert.Contains(result, item => item.ItemName.Contains("Cool", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result, item => item.Description.Contains("cool", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void FilterInventoryItems_FiltersByGameAndSearchText()
        {
            var game1 = new Game { GameTitle = "Zelda" };
            var game2 = new Game { GameTitle = "Halo" };

            var items = new List<Item>
            {
                new Item
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    Game = game1
                },
                new Item
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath3,
                    Game = game2
                },
            };

            var selectedGame = game1;
            var result = inventoryService.FilterInventoryItems(items, selectedGame, "cool");

            Assert.Single(result);
            Assert.All(result, item => Assert.Equal("Zelda", item.Game.GameTitle, ignoreCase: true));
            Assert.All(result, item => Assert.Contains("cool", item.ItemName, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetAvailableGames_EmptyUserInventory_ReturnsOnlyAllGamesOption()
        {
            var items = new List<Item>();

            userInventoryServiceProxyMock
                .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
                .ReturnsAsync(new UserInventoryResponse { Items = new List<InventoryItemResponse>() });

            gameServiceProxyMock
                .Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(new List<GameDetailedResponse>()); // No games

            var result = await inventoryService.GetAvailableGamesAsync(items);

            Assert.Single(result);
            Assert.Equal("All Games", result[0].GameTitle);
        }

        [Fact]
        public async Task GetAvailableGames_UserInventoryHasGames_ReturnsMatchingGames()
        {
            var items = new List<Item>();
            var game1 = new Game { GameTitle = "Zelda" };
            var game2 = new Game { GameTitle = "Halo" };
            var game3 = new Game { GameTitle = "OtherGame" };

            userInventoryServiceProxyMock
                .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
                .ReturnsAsync(new UserInventoryResponse
                {
                    Items = new List<InventoryItemResponse>
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
                            IsListed = testItemNotListed,
                            ImagePath = testItemImagePath3,
                            GameId = game2.GameId,
                            GameName = game2.GameTitle
                        },
                    }
                });

            gameServiceProxyMock
                .Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(new List<GameDetailedResponse>
                {
                    new GameDetailedResponse
                    {
                        Name = game1.GameTitle,
                    },
                    new GameDetailedResponse
                    {
                        Name = game2.GameTitle
                    },
                    new GameDetailedResponse
                    {
                        Name = game3.GameTitle
                    }
                });

            var result = await inventoryService.GetAvailableGamesAsync(items);

            Assert.Equal(3, result.Count);
            Assert.Contains(result, g => g.GameTitle == "All Games");
            Assert.Contains(result, g => g.GameTitle == "Halo");
            Assert.Contains(result, g => g.GameTitle == "Zelda");
        }
    }
}