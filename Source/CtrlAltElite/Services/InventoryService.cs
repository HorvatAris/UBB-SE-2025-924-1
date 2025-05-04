using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamStore.Services
{
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Repositories;
    using SteamStore.Models;
    using SteamStore.Repositories.Interfaces;
    using SteamStore.Services.Interfaces;
    using SteamStore.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using System.Threading.Tasks;
    using Windows.Security.Authentication.OnlineId;

    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly InventoryValidator inventoryValidator;

        private readonly IUserInventoryServiceProxy userInventoryServiceProxy;
        private readonly IItemServiceProxy itemServiceProxy;
        private readonly IGameServiceProxy gameServiceProxy;
        private User user;

        public InventoryService(IUserInventoryServiceProxy userInventoryServiceProxy, IItemServiceProxy itemServiceProxy, IGameServiceProxy gameServiceProxy, User user)
        {
            this.userInventoryServiceProxy = userInventoryServiceProxy;
            this.itemServiceProxy = itemServiceProxy;
            this.gameServiceProxy = gameServiceProxy;

            // Instantiate the validator with enriched logic.
            this.inventoryValidator = new InventoryValidator();
            this.user = user;
        }

        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game)
        {
            // Validate the game.
            this.inventoryValidator.ValidateGame(game);
            int userId = this.user.UserId;
            var userInventoryResponse = await this.userInventoryServiceProxy.GetUserInventoryAsync(userId);
            var filteredItems = userInventoryResponse.Items
                .Where(item => item.GameName == game.GameTitle)
                .Select(item => new Item
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Description = item.Description,
                    IsListed = item.IsListed,
                })
                .ToList();

            return filteredItems;
        }

        public async Task<List<Item>> GetAllItemsFromInventoryAsync()
        {
            // Validate the user.
            this.inventoryValidator.ValidateUser(this.user);

            int userId = this.user.UserId;
            var userInventoryResponse = await this.userInventoryServiceProxy.GetUserInventoryAsync(userId);
            var filteredItems = userInventoryResponse.Items
                .Select(item => new Item
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Description = item.Description,
                    IsListed = item.IsListed,
                })
                .ToList();

            return filteredItems;
        }

        public async Task AddItemToInventoryAsync(Game game, Item item)
        {
            // Validate the inventory operation.
            this.inventoryValidator.ValidateInventoryOperation(game, item, this.user);

            var itemFromInventoryRequest = new ItemFromInventoryRequest
            {
                UserId = this.user.UserId,
                ItemId = item.ItemId,
                GameId = game.GameId,
            };
            await this.userInventoryServiceProxy.AddItemToUserInventoryAsync(itemFromInventoryRequest);
        }

        public async Task RemoveItemFromInventoryAsync(Game game, Item item)
        {
            // Validate the inventory operation.
            this.inventoryValidator.ValidateInventoryOperation(game, item, user);
            await this.inventoryRepository.RemoveItemFromInventoryAsync(game, item, user);
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be positive.", nameof(userId));
            }
            var userInventoryResponse = await this.userInventoryServiceProxy.GetUserInventoryAsync(userId);
            var filteredItems = userInventoryResponse.Items
                .Select(item => new Item
                {
                    ItemId = item.ItemId,
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

        public User GetAllUsersAsync()
        {
            return this.user;
        }

        public async Task<bool> SellItemAsync(Item item)
        {
            // Validate that the item is sellable.
            this.inventoryValidator.ValidateSellableItem(item);

            // set isListed to 1
            item.IsListed = true;
            var allItems = await this.itemServiceProxy.GetItemsAsync();
            var foundItem = allItems.FirstOrDefault(currentItem => currentItem.ItemId == item.ItemId);
            var foundItemGameId = allItems.FirstOrDefault(currentItem => currentItem.ItemId == item.ItemId).GameId;

            // Create a request object for the item.
            var itemFromInventoryRequest = new UpdateItemRequest
            {
                ItemName = foundItem.ItemName,
                GameId = foundItemGameId,
                Price = foundItem.Price,
                Description = foundItem.Description,
                IsListed = item.IsListed,
                ImagePath = foundItem.ImagePath,
            };

            // Call the repository method to sell the item.
            try
            {
                await this.itemServiceProxy.UpdateItemAsync(item.ItemId, itemFromInventoryRequest);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them).
                Console.WriteLine($"Error selling item: {ex.Message}");
                return false;
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

        // Fix for CS1061: Replace the incorrect usage of 'GameId' with 'GameName' since 'InventoryItemResponse' does not have a 'GameId' property.
        // Update the relevant code block in the `GetAvailableGames` method.

        public async Task<List<Game>> GetAvailableGames(List<Item> items)
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
            var userItems = await this.userInventoryServiceProxy.GetUserInventoryAsync(this.user.UserId);
            List<string> gameNames = userItems.Items
                .Where(item => item.GameName != null)
                .Select(item => item.GameName)
                .Distinct()
                .ToList();

            // Get the game objects from the game names.
            List<Game> games = new List<Game> { allGamesOption };
            var allGames = await this.gameServiceProxy.GetGamesAsync(new GetGamesRequest());
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

        /// <inheritdoc/>
        public async Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText)
        {
            var allItems = await this.GetUserInventoryAsync(userId);
            return this.FilterInventoryItems(allItems, selectedGame, searchText);
        }

        private class GameComparer : IEqualityComparer<Game>
        {
            public bool Equals(Game x, Game y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                return x.GameId == y.GameId;
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