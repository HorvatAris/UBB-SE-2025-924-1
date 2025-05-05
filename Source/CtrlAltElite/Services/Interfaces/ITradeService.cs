using CtrlAltElite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.Services.Interfaces
{
    public interface ITradeService
    {
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        User GetCurrentUser();

        Task AddItemTradeAsync(ItemTrade trade);

        Task MarkTradeAsCompleted(int tradeId);

        Task DeclineTradeRequest();

        Task UpdateItemTradeAsync(ItemTrade trade);

        Task TransferItemAsync(int itemId, int fromUserId, int toUserId, int gameId);

        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        Task CreateTradeAsync(ItemTrade trade);

        Task UpdateTradeAsync(ItemTrade trade);

        Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser);

        void CompleteTrade(ItemTrade trade);

        Task<List<Item>> GetUserInventoryAsync(int userId);
    }
}
