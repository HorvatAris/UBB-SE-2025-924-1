﻿namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Services;
    using SteamHub.Utils;
    using Xunit;
    using SteamHub.ApiContract.Repositories;

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
        private readonly Mock<IUserInventoryRepository> userInventoryRepositoryMock;
        private readonly Mock<IItemRepository> itemRepositoryMock;
        private readonly Mock<IGameRepository> gameRepositoryMock;

        private readonly InventoryValidator inventoryValidator;

        private readonly User testUser;

        public InventoryServiceTests()
        {
            userInventoryRepositoryMock = new Mock<IUserInventoryRepository>();
            itemRepositoryMock = new Mock<IItemRepository>();
            gameRepositoryMock = new Mock<IGameRepository>();
            testUser = new User { UserId = 1, WalletBalance = 50f };
            inventoryService = new InventoryService(userInventoryRepositoryMock.Object, itemRepositoryMock.Object, gameRepositoryMock.Object, testUser);
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

            itemRepositoryMock.Setup(proxy => proxy.GetItemsAsync()).ReturnsAsync(new List<ItemDetailedResponse>
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

            itemRepositoryMock.Setup(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>())).Returns(Task.CompletedTask);

            var result = await inventoryService.SellItemAsync(item);

            Assert.True(result);
            Assert.True(item.IsListed);
            itemRepositoryMock.Verify(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>()), Times.Once);
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

            itemRepositoryMock.Setup(proxy => proxy.GetItemsAsync()).ReturnsAsync(new List<ItemDetailedResponse>
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

            itemRepositoryMock.Setup(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>()))
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

            itemRepositoryMock.Setup(proxy => proxy.GetItemsAsync()).ReturnsAsync(new List<ItemDetailedResponse>
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

            itemRepositoryMock.Setup(proxy => proxy.UpdateItemAsync(item.ItemId, It.IsAny<UpdateItemRequest>())).Returns(Task.CompletedTask);

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
        public void FilterInventoryItems_AllItems_OnlyReturnsUnlistedItems()
        {
            int resultCount = 2;

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

            Assert.Equal(resultCount, result.Count);
            Assert.All(result, item => Assert.False(item.IsListed));
        }

        [Fact]
        public void FilterInventoryItems_NotFilteredByGameName_ReturnAllItems()
        {
            int resultCount = 2;
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

            Assert.Equal(resultCount, result.Count);
        }

        [Fact]
        public void FilterInventoryItems_FiltersBySearchText_ReturnsMatchingItems()
        {
            int resultCount = 2;
            string comparingStringDescription = "cool";
            string comparingStringTitle = "Cool";
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

            var result = inventoryService.FilterInventoryItems(items, null, comparingStringDescription);

            Assert.Equal(resultCount, result.Count);
            Assert.Contains(result, item => item.ItemName.Contains(comparingStringTitle, StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result, item => item.Description.Contains(comparingStringDescription, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetAvailableGames_EmptyUserInventory_ReturnsOnlyAllGamesOption()
        {
            int firstItemIndex = 0;
            var items = new List<Item>();

            userInventoryRepositoryMock
                .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
                .ReturnsAsync(new UserInventoryResponse { Items = new List<InventoryItemResponse>() });

            gameRepositoryMock
                .Setup(proxy => proxy.GetGamesAsync(It.IsAny<GetGamesRequest>()))
                .ReturnsAsync(new List<GameDetailedResponse>()); // No games

            var result = await inventoryService.GetAvailableGamesAsync(items);

            Assert.Single(result);
            Assert.Equal("All Games", result[firstItemIndex].GameTitle);
        }

        [Fact]
        public async Task GetAvailableGames_UserInventoryHasGames_ReturnsMatchingGames()
        {
            int resultCount = 3;
            var items = new List<Item>();
            var game1 = new Game { GameTitle = "Zelda" };
            var game2 = new Game { GameTitle = "Halo" };
            var game3 = new Game { GameTitle = "OtherGame" };

            userInventoryRepositoryMock
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

            gameRepositoryMock
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
            Assert.Contains(result, game => game.GameTitle == "All Games");
            Assert.Contains(result, game => game.GameTitle == "Halo");
            Assert.Contains(result, game => game.GameTitle == "Zelda");
        }
    }
}