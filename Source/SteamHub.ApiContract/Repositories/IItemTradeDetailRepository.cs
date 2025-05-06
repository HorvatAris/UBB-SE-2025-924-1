using SteamHub.ApiContract.Models.ItemTradeDetails;

namespace SteamHub.ApiContract.Repositories;
public interface IItemTradeDetailRepository
{
    Task<GetItemTradeDetailsResponse?> GetItemTradeDetailsAsync();

    Task<ItemTradeDetailResponse?> GetItemTradeDetailAsync(int tradeId, int itemId);

    Task<CreateItemTradeDetailResponse> CreateItemTradeDetailAsync(CreateItemTradeDetailRequest request);

    Task DeleteItemTradeDetailAsync(int tradeId, int itemId);
}