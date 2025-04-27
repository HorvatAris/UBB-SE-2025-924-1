using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamStore.Services
{
    using SteamStore.Models;
    using SteamStore.Repositories.Interfaces;
    using SteamStore.Services.Interfaces;
    using SteamStore.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using System.Threading.Tasks;

    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly InventoryValidator inventoryValidator;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));

            // Instantiate the validator with enriched logic.
            this.inventoryValidator = new InventoryValidator();
        }

        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game)
        {
            // Validate the game.
            this.inventoryValidator.ValidateGame(game);
            return await this.inventoryRepository.GetItemsFromInventoryAsync(game);
        }

        public async Task<List<Item>> GetAllItemsFromInventoryAsync(User user)
        {
            // Validate the user.
            this.inventoryValidator.ValidateUser(user);
            return await this.inventoryRepository.GetAllItemsFromInventoryAsync(user);
        }

        public async Task AddItemToInventoryAsync(Game game, Item item, User user)
        {
            // Validate the inventory operation.
            this.inventoryValidator.ValidateInventoryOperation(game, item, user);
            await this.inventoryRepository.AddItemToInventoryAsync(game, item, user);
        }

        public async Task RemoveItemFromInventoryAsync(Game game, Item item, User user)
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

            return await this.inventoryRepository.GetUserInventoryAsync(userId);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            
            return await this.inventoryRepository.GetAllUsersAsync();
        }

        public async Task<bool> SellItemAsync(Item item)
        {
            // Validate that the item is sellable.
            this.inventoryValidator.ValidateSellableItem(item);
            return await this.inventoryRepository.SellItemAsync(item);
        }

        public List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            // Exclude items that are already listed.
            // var filteredItems = items.Where(item => !item.IsListed);
            var filteredItems = items.Where(item => item.IsListed);
            // If a specific game is selected (not the "All Games" option), filter by that game.
            if (selectedGame != null && selectedGame.GameTitle != "All Games")
            {
                filteredItems = filteredItems.Where(item =>
                    item.Game != null && item.Game.GameId == selectedGame.GameId);
            }

            // Apply search text filter on item name or description (case-insensitive).
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredItems = filteredItems.Where(item =>
                    (!string.IsNullOrEmpty(item.ItemName) &&
                     item.ItemName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(item.Description) &&
                     item.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }

            return filteredItems.ToList();
        }

        /// <inheritdoc/>
        public List<Game> GetAvailableGames(List<Item> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            // Create the special "All Games" option.
            //var game = new Game
            //{
            //    GameId = reader.GetInt32(reader.GetOrdinal("GameId")),
            //    GameTitle = reader.GetString(reader.GetOrdinal("GameTitle")),
            //    //Genre = reader.GetString(reader.GetOrdinal("Genre")),
            //    GameDescription = reader.GetString(reader.GetOrdinal("GameDescription")),
            //    Price = (decimal)reader.GetDouble(reader.GetOrdinal("GamePrice")),
            //    Status = reader.GetString(reader.GetOrdinal("GameStatus")),
            //};
            var allGamesOption = new Game
            {
                GameTitle = "All Games",
                Price = 0,
                GameDescription = "Show items from all games",
            };

            // Start with the "All Games" option.
            var availableGames = new List<Game> { allGamesOption };

            // Add unique games from the inventory.
            var uniqueGames = items
                .Select(item => item.Game)
                .Where(game => game != null)
                .Distinct(new GameComparer());

            availableGames.AddRange(uniqueGames);
            return availableGames;
        }

        /// <inheritdoc/>
        public async Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText)
        {
            var allItems = await this.GetUserInventoryAsync(userId);
            return this.FilterInventoryItems(allItems, selectedGame, searchText);
        }

        /// <summary>
        /// Provides a custom comparer for <see cref="Game"/> objects based on the GameId.
        /// </summary>
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
