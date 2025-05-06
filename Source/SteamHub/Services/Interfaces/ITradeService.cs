// <copyright file="ITradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.Models;

    public interface ITradeService
    {
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        User GetCurrentUser();

        Task AddItemTradeAsync(ItemTrade trade);

        Task MarkTradeAsCompletedAsync(int tradeId);

        void DeclineTradeRequest();

        Task UpdateItemTradeAsync(ItemTrade trade);

        Task TransferItemAsync(int itemId, int fromUserId, int toUserId, int gameId);

        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        Task CreateTradeAsync(ItemTrade trade);

        Task UpdateTradeAsync(ItemTrade trade);

        Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser);

        Task CompleteTradeAsync(ItemTrade trade);

        Task<List<Item>> GetUserInventoryAsync(int userId);
    }
}
