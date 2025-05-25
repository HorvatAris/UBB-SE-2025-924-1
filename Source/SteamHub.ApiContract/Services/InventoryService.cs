// <copyright file="InventoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using SteamHub.ApiContract.Models.Item;

namespace SteamHub.ApiContract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Proxies;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ApiContract.Utils;

    public class InventoryService : IInventoryService
    {
        private readonly InventoryValidator inventoryValidator;

        private readonly IUserRepository userRepository;
        private readonly IUserInventoryRepository userInventoryRepository;
        private readonly IItemRepository itemRepository;
        private readonly IGameRepository gameRepository;
        

        public InventoryService(IUserRepository userRepository, IUserInventoryRepository userInventoryRepository, IItemRepository itemRepository, IGameRepository gameRepository)
        {
            this.userRepository = userRepository;
            this.userInventoryRepository = userInventoryRepository;
            this.itemRepository = itemRepository;
            this.gameRepository = gameRepository;

            // Instantiate the validator with enriched logic.
            this.inventoryValidator = new InventoryValidator();
            
        }

        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game, int userId)
        {
            // Validate the game.
            this.inventoryValidator.ValidateGame(game);
            var userInventoryResponse = await this.userInventoryRepository.GetUserInventoryAsync(userId);
            var filteredItems = userInventoryResponse.Items
                .Where(item => item.GameName == game.GameTitle)
                .Select(item => new Item
                {
                    ItemId = item.ItemId,
                    GameName = item.GameName,
                    ImagePath = item.ImagePath,
                    Game = game,
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Description = item.Description,
                    IsListed = item.IsListed,
                })
                .ToList();

            return filteredItems;
        }

        public async Task<List<Item>> GetAllItemsFromInventoryAsync(int userId)
        {
            // Validate the user.
            var games = await this.gameRepository.GetGamesAsync(new GetGamesRequest());

            var userInventoryResponse = await this.userInventoryRepository.GetUserInventoryAsync(userId);
            var filteredItems = userInventoryResponse.Items
                .Select(item => new Item
                {
                    ItemId = item.ItemId,
                    ImagePath = item.ImagePath,
                    GameName = item.GameName,
                    Game = GameMapper.MapToGame(games.FirstOrDefault(game => game.Name == item.GameName)),
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Description = item.Description,
                    IsListed = item.IsListed,
                })
                .ToList();

            return filteredItems;
        }

        public async Task AddItemToInventoryAsync(Game game, Item item, int userId)
        {
            // Validate the inventory operation.
            //this.inventoryValidator.ValidateInventoryOperation(game, item, this.user);

            var itemFromInventoryRequest = new ItemFromInventoryRequest
            {
                UserId = userId,
                ItemId = item.ItemId,
                GameId = game.GameId,
            };
            await this.userInventoryRepository.AddItemToUserInventoryAsync(itemFromInventoryRequest);
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be positive.", nameof(userId));
            }

            var userInventoryResponse = await this.userInventoryRepository.GetUserInventoryAsync(userId);
            var allGames = await this.gameRepository.GetGamesAsync(new GetGamesRequest());

            var filteredItems = userInventoryResponse.Items
                .Select(item => new Item
                {
                    ItemId = item.ItemId,
                    Game = GameMapper.MapToGame(allGames.FirstOrDefault(game => game.Name == item.GameName)),
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Description = item.Description,
                    IsListed = item.IsListed,
                    ImagePath = item.ImagePath,
                    GameName = item.GameName,
                    
                })
                .ToList();

            return filteredItems;
        }

        public IUserDetails GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<IUserDetails> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }


        public async Task<bool> SellItemAsync(Item item, int userId)
        {
            // Validate that the item is sellable.
            this.inventoryValidator.ValidateSellableItem(item);

            // set isListed to 1
            item.IsListed = true;
            var allItems = await this.itemRepository.GetItemsAsync();
            var foundItem = allItems.FirstOrDefault(currentItem => currentItem.ItemId == item.ItemId);

            if (foundItem == null)
            {
                Console.WriteLine($"Item with ID {item.ItemId} not found.", nameof(item));
                throw new Exception("The item can't be found.");
            }

            var foundItemGameId = allItems.FirstOrDefault(currentItem => currentItem.ItemId == item.ItemId).GameId;
            var user = await this.userRepository.GetUserByIdAsync(userId);

            // Create a request object for the item.
            var itemUpdateRequest = new UpdateItemRequest
            {
                ItemName = foundItem.ItemName,
                GameId = foundItemGameId,
                Price = foundItem.Price,
                Description = foundItem.Description,
                IsListed = item.IsListed,
                ImagePath = foundItem.ImagePath,
            };

            var itemFromInventoryRequest = new ItemFromInventoryRequest
            {
                UserId = userId,
                ItemId = item.ItemId,
                GameId = foundItemGameId,
            };

            var userUpdateRequest = new UpdateUserRequest
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                WalletBalance = user.WalletBalance + item.Price,
                PointsBalance = user.PointsBalance,
            };

            // Call the repository method to sell the item.
            try
            {
                await this.itemRepository.UpdateItemAsync(item.ItemId, itemUpdateRequest);
                await this.userInventoryRepository.RemoveItemFromUserInventoryAsync(itemFromInventoryRequest);
                await this.userRepository.UpdateUserAsync(userId, userUpdateRequest);
            }
            catch (Exception exception)
            {
                // Handle exceptions (e.g., log them).
                Console.WriteLine($"Error selling item: {exception.Message}");
                throw new Exception("Item couldn't be updated as for sale.");
            }

            // return await this.inventoryRepository.SellItemAsync(item);
            return true;
        }

        public List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            IEnumerable<Item> filtered = items;

            // Filter out listed items (only show unlisted ones)
            filtered = filtered.Where(item => !item.IsListed);

            // Filter by selected game if it's not null and not the "All Games" option
            if (selectedGame != null && selectedGame.GameTitle != "All Games")
            {
                filtered = filtered.Where(item =>
                    string.Equals(item.GameName, selectedGame.GameTitle, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by search text if provided
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                filtered = filtered.Where(item =>
                    (!string.IsNullOrEmpty(item.ItemName) &&
                     item.ItemName.Trim().Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(item.Description) &&
                     item.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }

            return filtered.ToList();
        }

        public async Task<List<Game>> GetAvailableGamesAsync(List<Item> items,int userId)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            // Create the special "All Games" option.
            var allGamesOption = new Game
            {
                GameTitle = "All Games",
                Price = 0,
                GameDescription = "Show items from all games",
            };

            // Create a list to hold the available games.
            var userItems = await this.userInventoryRepository.GetUserInventoryAsync(userId);
            List<string> gameNames = userItems.Items
                .Where(item => item.GameName != null)
                .Select(item => item.GameName)
                .Distinct()
                .ToList();

            // Get the game objects from the game names.
            List<Game> games = new List<Game> { allGamesOption };
            var allGames = await this.gameRepository.GetGamesAsync(new GetGamesRequest());
            foreach (var gameName in gameNames)
            {
                var foundGame = allGames.FirstOrDefault(currentGame => currentGame.Name == gameName);
                if (foundGame != null)
                {
                    games.Add(GameMapper.MapToGame(foundGame));
                }
            }

            return games;
        }

        public async Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText)
        {
            var allItems = await this.GetUserInventoryAsync(userId);

            return this.FilterInventoryItems(allItems, selectedGame, searchText);
        }

        private class GameComparer : IEqualityComparer<Game>
        {
            public bool Equals(Game firstGame, Game secondGame)
            {
                if (firstGame == null || secondGame == null)
                {
                    return false;
                }

                return firstGame.GameId == secondGame.GameId;
            }

            public int GetHashCode(Game objectTGetHashCodeFrom)
            {
                if (objectTGetHashCodeFrom == null)
                {
                    return 0;
                }

                return objectTGetHashCodeFrom.GameId.GetHashCode();
            }
        }
    }
}