// <copyright file="ITradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services.TradeService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    public interface ITradeService
    {
        Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser);

        Task CreateTradeAsync(ItemTrade trade);

        Task DeclineTradeAsync(ItemTrade trade);

        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        Task UpdateTradeAsync(ItemTrade trade);

        Task<User?> GetCurrentUserAsync();

        Task<List<Item>> GetUserInventoryAsync(int userId);
    }
}