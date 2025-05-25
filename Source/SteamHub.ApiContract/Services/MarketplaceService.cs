// <copyright file="MarketplaceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Proxies;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services.Interfaces;

    public class MarketplaceService : IMarketplaceService
    {
        public IGameRepository GameRepository { get; set; }

        public IUserInventoryRepository UserInventoryRepository { get; set; }

        public IUserRepository UserRepository { get; set; }

        public IItemRepository ItemRepository { get; set; }

        public IUserDetails User { get; set; }

        public MarketplaceService(IUserRepository userRepository, IGameRepository gameRepository, IItemRepository itemRepository,
                                  IUserInventoryRepository userInventoryRepository)
        {
            this.UserRepository = userRepository;
            this.GameRepository = gameRepository;
            this.ItemRepository = itemRepository;
            this.UserInventoryRepository = userInventoryRepository;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var returnUsers = new List<User>();
            var users = await this.UserRepository.GetUsersAsync();
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
                        UserRole = (user.Role == RoleEnum.User) ? UserRole.User : UserRole.Developer,
                    });
            }

            return returnUsers;
        }

        public async Task<List<Item>> GetAllListingsAsync()
        {
            var result = new List<Item>();
            var items = await this.ItemRepository.GetItemsAsync();
            foreach (var item in items)
            {
                if (item.IsListed)
                {
                    var resultGame = GameMapper.MapToGame(await this.GameRepository.GetGameByIdAsync(item.GameId));
                    var resultItem = new Item
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        GameName = resultGame.GameTitle,
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

        public async Task<List<Item>> GetListingsByGameAsync(int gameId, int userId)
        {
            var game = await this.GameRepository.GetGameByIdAsync(gameId);
            var result = new List<Item>();

            var userItems = (await this.UserInventoryRepository.GetUserInventoryAsync(userId)).Items;
            foreach (var userItem in userItems)
            {
                var item = await this.ItemRepository.GetItemByIdAsync(userItem.ItemId);
                if (item.IsListed && item.GameId == game.Identifier)
                {
                    var resultGame = GameMapper.MapToGame(await this.GameRepository.GetGameByIdAsync(item.GameId));
                    var resultItem = new Item
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        GameName = resultGame.GameTitle,
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
            await this.SwitchListingStatusAsync(game.GameId, item.ItemId);
        }

        public async Task RemoveListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game.GameId, item.ItemId);
        }

        public async Task UpdateListingAsync(int gameId, int itemId)
        {
            var item = await this.ItemRepository.GetItemByIdAsync(itemId);
            await this.ItemRepository.UpdateItemAsync(
                item.ItemId,
                new UpdateItemRequest
                {
                    ItemName = item.ItemName,
                    IsListed = item.IsListed,
                    Description = item.Description,
                    GameId = item.GameId,
                    Price = item.Price,
                    ImagePath = item.ImagePath,
                });
        }

        public async Task<bool> BuyItemAsync(Item item, int currentUserId)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsListed)
            {
                throw new InvalidOperationException("Item is not listed for sale");
            }

            var currentUser = await this.UserRepository.GetUserByIdAsync(currentUserId);

            await this.UserInventoryRepository.AddItemToUserInventoryAsync(
                new ItemFromInventoryRequest
                {
                    GameId = item.Game.GameId,
                    UserId = currentUserId,
                    ItemId = item.ItemId,
                });

            await this.ItemRepository.UpdateItemAsync(
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

            await this.UserRepository.UpdateUserAsync(
                currentUserId,
                new UpdateUserRequest
                {
                    UserName = currentUser.UserName,
                    Email = currentUser.Email,
                    WalletBalance = currentUser.WalletBalance - item.Price,
                    PointsBalance = currentUser.PointsBalance,
                    Role = currentUser.Role,
                });

            return true;
        }

        public async Task SwitchListingStatusAsync(int gameId, int itemId)
        {
            var game = await this.GameRepository.GetGameByIdAsync(gameId);
            var item = await this.ItemRepository.GetItemByIdAsync(itemId);
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.ItemRepository.UpdateItemAsync(
                item.ItemId,
                new UpdateItemRequest
                {
                    ItemName = item.ItemName,
                    Description = item.Description,
                    Price = item.Price,
                    IsListed = !item.IsListed,
                    GameId = item.GameId,
                    ImagePath = item.ImagePath,
                });
        }
    }
}