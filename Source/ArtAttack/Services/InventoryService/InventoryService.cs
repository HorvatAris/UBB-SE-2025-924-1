// <copyright file="InventoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services.InventoryService.InventoryService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;
    using Steampunks.Repository.Inventory;
    using Steampunks.Validators.InventoryValidator.InventoryValidator;
    using Steampunks.Validators.InventoryValidators;

    /// <summary>
    /// Service that handles inventory-related operations.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly IInventoryValidator inventoryValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        /// <param name="inventoryRepository">The inventory repository.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the repository is null.
        /// </exception>
        public InventoryService(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));

            // Instantiate the validator with enriched logic.
            this.inventoryValidator = new InventoryValidator();
        }

        /// <inheritdoc/>
        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game)
        {
            // Validate the game.
            this.inventoryValidator.ValidateGame(game);
            return await this.inventoryRepository.GetItemsFromInventoryAsync(game);
        }

        /// <inheritdoc/>
        public async Task<List<Item>> GetAllItemsFromInventoryAsync(User user)
        {
            // Validate the user.
            this.inventoryValidator.ValidateUser(user);
            return await this.inventoryRepository.GetAllItemsFromInventoryAsync(user);
        }

        /// <inheritdoc/>
        public async Task AddItemToInventoryAsync(Game game, Item item, User user)
        {
            // Validate the inventory operation.
            this.inventoryValidator.ValidateInventoryOperation(game, item, user);
            await this.inventoryRepository.AddItemToInventoryAsync(game, item, user);
        }

        /// <inheritdoc/>
        public async Task RemoveItemFromInventoryAsync(Game game, Item item, User user)
        {
            // Validate the inventory operation.
            this.inventoryValidator.ValidateInventoryOperation(game, item, user);
            await this.inventoryRepository.RemoveItemFromInventoryAsync(game, item, user);
        }

        /// <inheritdoc/>
        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be positive.", nameof(userId));
            }

            return await this.inventoryRepository.GetUserInventoryAsync(userId);
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.inventoryRepository.GetAllUsersAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> SellItemAsync(Item item)
        {
            // Validate that the item is sellable.
            this.inventoryValidator.ValidateSellableItem(item);
            return await this.inventoryRepository.SellItemAsync(item);
        }

        /// <inheritdoc/>
        public List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            // Exclude items that are already listed.
            var filteredItems = items.Where(item => !item.IsListed);

            // If a specific game is selected (not the "All Games" option), filter by that game.
            if (selectedGame != null && selectedGame.Title != "All Games")
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
            var allGamesOption = new Game("All Games", 0.0f, string.Empty, "Show items from all games");

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
