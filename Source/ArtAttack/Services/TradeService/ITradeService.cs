// <copyright file="ITradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services.TradeService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Interface for TradeService.
    /// </summary>
    public interface ITradeService
    {
        /// <summary>
        /// Asynchronously accepts the trade and completes it if accepted by both parts.
        /// </summary>
        /// <param name="trade">trade to be accepted.</param>
        /// <param name="isSourceUser">true if the user is source, false is they are destination.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser);

        /// <summary>
        /// Adds the trade to the database asynchronously.
        /// </summary>
        /// <param name="trade">The trade.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateTradeAsync(ItemTrade trade);

        /// <summary>
        /// Declines the trade asynchronously.
        /// </summary>
        /// <param name="trade">The trade.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeclineTradeAsync(ItemTrade trade);

        /// <summary>
        /// Retrieves the active trades for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        /// <summary>
        /// Retrieves the trade history for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">id of the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        /// <summary>
        /// Updates the trade in the database asynchronously.
        /// </summary>
        /// <param name="trade">The modified trade.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateTradeAsync(ItemTrade trade);

        /// <summary>
        /// Gets the current user from the database asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<User?> GetCurrentUserAsync();

        /// <summary>
        /// Gets the inventory of the given user from the database asynchronously.
        /// </summary>
        /// <param name="userId">The userID for which to fetch the inventory.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<Item>> GetUserInventoryAsync(int userId);
    }
}