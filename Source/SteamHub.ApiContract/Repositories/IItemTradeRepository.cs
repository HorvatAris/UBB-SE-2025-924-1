using SteamHub.ApiContract.Models.ItemTrade;

namespace SteamHub.ApiContract.Repositories
{
    public interface IItemTradeRepository
    {
        Task<CreateItemTradeResponse> CreateItemTradeAsync(CreateItemTradeRequest request);
        Task<GetItemTradesResponse?> GetItemTradesAsync();
        Task<ItemTradeResponse?> GetItemTradeByIdAsync(int id);
        Task UpdateItemTradeAsync(int tradeId, UpdateItemTradeRequest request);
        Task DeleteItemTradeAsync(int tradeId);
    }
}
