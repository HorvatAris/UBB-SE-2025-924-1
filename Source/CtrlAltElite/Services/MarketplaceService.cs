// <copyright file="MarketplaceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services.Interfaces;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserInventory;

    public class MarketplaceService : IMarketplaceService
    {
        // private readonly IMarketplaceRepository marketplaceRepository;
        public IGameServiceProxy GameServiceProxy { get; set; }

        public IUserInventoryServiceProxy UserInventoryServiceProxy { get; set; }

        public IUserServiceProxy UserServiceProxy { get; set; }

        public IItemServiceProxy ItemServiceProxy { get; set; }

        public User User { get; set; }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var returnUsers = new List<User>();
            var users = await this.UserServiceProxy.GetUsersAsync();
            foreach (var user in users.Users)
            {
                returnUsers.Add(
                    new User
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Email = user.Email,
                        WalletBalance = user.WalletBalance,
                        PointsBalance = user.PointsBalance,
                        UserRole = (user.Role == RoleEnum.User) ? User.Role.User : User.Role.Developer,
                    });
            }

            return returnUsers;
        }

        public async Task<List<Item>> GetAllListingsAsync()
        {
            var result = new List<Item>();
            var items = await this.ItemServiceProxy.GetItemsAsync();
            foreach (var item in items)
            {
                if (item.IsListed)
                {
                    var resultGame = GameMapper.MapToGame(await this.GameServiceProxy.GetGameByIdAsync(item.GameId));
                    var resultItem = new Item
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        IsListed = item.IsListed,
                        ImagePath = item.ImagePath,
                        Description = item.Description,
                        Price = item.Price,
                        Game = resultGame,
                    };
                    result.Add(resultItem);
                }
            }

            return result;
        }

        public async Task<List<Item>> GetListingsByGameAsync(Game game, int userId)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            var result = new List<Item>();

            var userItems = (await this.UserInventoryServiceProxy.GetUserInventoryAsync(userId)).Items;
            foreach (var u in userItems)
            {
                var item = await this.ItemServiceProxy.GetItemByIdAsync(u.ItemId);
                if (item.IsListed && item.GameId == game.GameId)
                {
                    var resultGame = GameMapper.MapToGame(await this.GameServiceProxy.GetGameByIdAsync(item.GameId));
                    var resultItem = new Item
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        IsListed = item.IsListed,
                        ImagePath = item.ImagePath,
                        Description = item.Description,
                        Price = item.Price,
                        Game = resultGame,
                    };
                    result.Add(resultItem);
                }
            }

            return result;
        }

        public async Task AddListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game, item);
        }

        public async Task RemoveListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game, item);
        }

        public async Task UpdateListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.ItemServiceProxy.UpdateItemAsync(
                item.ItemId,
                new UpdateItemRequest
                {
                    ItemName = item.ItemName,
                    IsListed = item.IsListed,
                    Description = item.Description,
                    GameId = item.Game.GameId,
                    Price = item.Price,
                    ImagePath = item.ImagePath,
                });
        }

        public async Task<bool> BuyItemAsync(Item item, int currentUser)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsListed)
            {
                throw new InvalidOperationException("Item is not listed for sale");
            }

            // await this.(item, transaction);
            var entries = await this.UserInventoryServiceProxy.GetUserInventoryAsync(this.User.UserId);
            foreach (var entry in entries.Items)
            {
                if (entry.ItemId == item.ItemId)
                {
                    await this.UserInventoryServiceProxy.RemoveItemFromUserInventoryAsync(
                        new ItemFromInventoryRequest
                        {
                            GameId = entry.GameId,
                            ItemId = item.ItemId,
                            UserId = this.User.UserId,
                        });
                }
            }

            // await this.userInventoryServiceProxy.RemoveItemFromUserInventoryAsync(
            //     new ItemFromInventoryRequest
            //     {
            //     });
            await this.UserInventoryServiceProxy.AddItemToUserInventoryAsync(
                new ItemFromInventoryRequest
                {
                    GameId = item.Game.GameId,
                    UserId = currentUser,
                    ItemId = item.ItemId,
                });

            await this.ItemServiceProxy.UpdateItemAsync(
                item.ItemId,
                new UpdateItemRequest
                {
                    Description = item.Description,
                    GameId = item.Game.GameId,
                    Price = item.Price,
                    ImagePath = item.ImagePath,
                    IsListed = false,
                    ItemName = item.ItemName,
                });

            // item.SetIsListed(false);
            // return await this.marketplaceRepository.BuyItemAsync(item, this.currentUser);
            return true;
        }

        private async Task SwitchListingStatusAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.ItemServiceProxy.UpdateItemAsync(
                item.ItemId,
                new UpdateItemRequest
                {
                    ItemName = item.ItemName,
                    Description = item.Description,
                    Price = item.Price,
                    IsListed = !item.IsListed,
                    GameId = item.Game.GameId,
                    ImagePath = item.ImagePath,
                });
        }
    }
}