// <copyright file="IMarketplaceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Repository.Marketplace
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides repository methods for interacting with marketplace items.
    /// Allows listing, unlisting, updating, and retrieving item listings associated with games.
    /// </summary>
    public interface IMarketplaceRepository
    {
        /// <summary>
        /// Handles the purchase of an item by removing former ownership, adding the item to the new owner's inventory,
        /// and marking the item as unlisted.
        /// </summary>
        /// <param name="item"> Item to be purchased. </param>
        /// <param name="currentUser"> User that makes the item purchase. </param>
        /// <returns> True upon successful completion. </returns>
        Task<bool> BuyItemAsync(Item item, User currentUser);

        /// <summary>
        /// Retrieves all listed items available for sale from the database.
        /// </summary>
        /// <returns>
        /// A list of Item objects that are currently listed for sale.
        /// </returns>
        Task<List<Item>> GetAllListedItemsAsync();

        /// <summary>
        /// Retrieves all currently listed items associated with a specific game.
        /// </summary>
        /// <param name="game">The Game entity for which to retrieve listed items.</param>
        /// <returns>
        /// A list of Item objects linked to the specified game and currently marked as listed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="game"/> is null.
        /// </exception>
        Task<List<Item>> GetListedItemsByGameAsync(Game game);

        /// <summary>
        /// Marks an existing item as listed for the specified game.
        /// </summary>
        /// <param name="game">The Game associated with the item to be listed.</param>
        /// <param name="item">The Item to be marked as listed and updated with a new price.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="game"/> or <paramref name="item"/> is null.
        /// </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        Task MakeItemListableAsync(Game game, Item item);

        /// <summary>
        /// Marks an item as not listable in the specified game by updating its listing status.
        /// </summary>
        /// <param name="game">The game in which the item exists.</param>
        /// <param name="item">The item to be marked as not listable.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="game"/> or <paramref name="item"/> is null.
        /// </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        Task MakeItemNotListableAsync(Game game, Item item);

        /// <summary>
        /// Updates the price of an item in the specified game.
        /// </summary>
        /// <param name="game">The game in which the item exists.</param>
        /// <param name="item">The item whose price are being updated.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="game"/> or <paramref name="item"/> is null.
        /// </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        Task UpdateItemPriceAsync(Game game, Item item);

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns> The current User. </returns>
        User? GetCurrentUser();

        /// <summary>
        /// Gets a list of all users.
        /// </summary>
        /// <returns> A list of users. </returns>
        Task<List<User>> GetAllUsersAsync();
    }
}
