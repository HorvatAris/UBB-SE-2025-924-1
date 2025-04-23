// <copyright file="ITradeRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Repository.Trade
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Interface for the TradeRepository.
    /// </summary>
    public interface ITradeRepository
    {
        /// <summary>
        /// Adds the trade to the database asynchronously.
        /// </summary>
        /// <param name="trade">the trade to be added.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AddItemTradeAsync(ItemTrade trade);

        /// <summary>
        /// Fetches the active trades list for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">the userID.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        /// <summary>
        /// Fetches the current user from the database asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<User?> GetCurrentUserAsync();

        /// <summary>
        /// Fetches the trade history for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">the userID.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        /// <summary>
        /// Fetches the inventory list for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">The userID.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<Item>> GetUserInventoryAsync(int userId);

        /// <summary>
        /// Performs the item trade between the users and persists the changes to the database asynchronously.
        /// </summary>
        /// <param name="itemId">the traded itemID.</param>
        /// <param name="fromUserId">the source userID.</param>
        /// <param name="toUserId">the destination userID.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task TransferItemAsync(int itemId, int fromUserId, int toUserId);

        /// <summary>
        /// Updates the trade in the database asynchronously.
        /// </summary>
        /// <param name="trade">the modified trade.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateItemTradeAsync(ItemTrade trade);
    }
}