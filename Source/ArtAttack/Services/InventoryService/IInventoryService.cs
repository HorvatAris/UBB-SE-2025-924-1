// <copyright file="IInventoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services.InventoryService.InventoryService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Interface for service that handles inventory-related operations.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Retrieves a list of items from the user's inventory from the given game.
        /// </summary>
        /// <param name="game"> Game for which the items are retrieved. </param>
        /// <returns> A list of items from the game, from the user's inventory. </returns>
        Task<List<Item>> GetItemsFromInventoryAsync(Game game);

        /// <summary>
        /// Retrieves all the items of the user from the inventory.
        /// </summary>
        /// <param name="user">The user whose inventory is to be retrieved.</param>
        /// <returns> A list of all items from the inventory. </returns>
        Task<List<Item>> GetAllItemsFromInventoryAsync(User user);

        /// <summary>
        /// Adds an item from a game to the inventory.
        /// </summary>
        /// <param name="game"> Game from which the item comes from. </param>
        /// <param name="item"> Item to be added to the inventory. </param>
        /// <param name="user">The user who is adding the item to their inventory.</param>
        /// <returns>AddInventoryItemAsync returns <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddItemToInventoryAsync(Game game, Item item, User user);

        /// <summary>
        /// Removes an item from a game from the inventory.
        /// </summary>
        /// <param name="game"> Game from which the item comes from. </param>
        /// <param name="item"> Item to be removed from the inventory. </param>
        /// <param name="user">The user whose inventory the item is being removed from.</param>
        /// <returns>RemoveInventoryItemAsync returns <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveItemFromInventoryAsync(Game game, Item item, User user);

        /// <summary>
        /// Get the inventory of a given User by it's userID Asynchronously.
        /// </summary>
        /// <param name="userId">The id of the user whose inventory items are to be retrieved.</param>
        /// <returns>A <see cref="Task"/> asynchronously resolving to a list of <see cref="Item"/> objects associated with the specified user.</returns>
        Task<List<Item>> GetUserInventoryAsync(int userId);

        /// <summary>
        /// Retrieves all users from the repository asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> asynchronously resolving to a list of <see cref="User"/> objects.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Sells an item asynchronously.
        /// </summary>
        /// <param name="item">The item who will be listed as for sale.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> SellItemAsync(Item item);

        /// <summary>
        /// Filters inventory items based on the selected game and search text.
        /// </summary>
        /// <param name="items">The list of inventory items to filter.</param>
        /// <param name="selectedGame">
        /// The game to filter by. Assumes the "All Games" option is identified by its title.
        /// </param>
        /// <param name="searchText">The text used to search item names and descriptions.</param>
        /// <returns>A filtered list of items.</returns>
        List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText);

        /// <summary>
        /// Retrieves a list of available games based on the provided inventory items.
        /// Includes a special "All Games" option as the first entry.
        /// </summary>
        /// <param name="items">The list of inventory items.</param>
        /// <returns>A list of games.</returns>
        List<Game> GetAvailableGames(List<Item> items);

        /// <summary>
        /// Retrieves the filtered inventory for a given user based on selected game and search text.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="selectedGame">The game filter.</param>
        /// <param name="searchText">The search text for item name/description.</param>
        /// <returns>A task that represents the asynchronous operation containing the filtered list of items.</returns>
        Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText);
    }
}