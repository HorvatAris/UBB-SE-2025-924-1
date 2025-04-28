using SteamHub.Api.Models.UserPointShopItemInventory;

namespace SteamHub.Api.Context.Repositories
{
    public interface IUserPointShopItemInventoryRepository
    {
        Task<GetUserPointShopItemInventoryResponse> GetUserInventoryAsync(int userId);

        Task PurchaseItemAsync(PurchasePointShopItemRequest request);
        Task UpdateItemStatusAsync(UpdateUserPointShopItemInventoryRequest request);
        Task ResetUserInventoryAsync(int userId);

    }
}
