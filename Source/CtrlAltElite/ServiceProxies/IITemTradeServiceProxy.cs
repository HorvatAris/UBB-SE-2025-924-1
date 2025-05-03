using Refit;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ServiceProxies
{
    public interface IITemTradeServiceProxy :IItemTradeRepository
    {
        [Get("/api/ItemTrades")]
        Task<GetItemTradesResponse> GetAllItemTradesAsync();

        [Get("/api/ItemTrades/{id}")]
        Task<ItemTradeResponse?> GetItemTradeByIdAsync(int id);

        [Post("/api/ItemTrades")]
        Task<CreateItemTradeResponse> CreateItemTradeAsync([Body] CreateItemTradeRequest request);

        [Put("/api/ItemTrades/{id}")]
        Task UpdateItemTradeAsync(int id, [Body] UpdateItemTradeRequest request);

        [Delete("/api/ItemTrades/{id}")]
        Task DeleteItemTradeAsync(int id);
    }
}
