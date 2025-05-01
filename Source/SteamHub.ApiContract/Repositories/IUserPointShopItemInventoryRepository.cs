namespace SteamHub.ApiContract.Context.Repositories
{
    using SteamHub.ApiContract.Models.UserPointShopItemInventory;

    public interface IUserPointShopItemInventoryRepository
    {
        Task<GetUserPointShopItemInventoryResponse> GetUserInventoryAsync(int userId);

        Task PurchaseItemAsync(PurchasePointShopItemRequest request);
        Task UpdateItemStatusAsync(UpdateUserPointShopItemInventoryRequest request);
        Task ResetUserInventoryAsync(int userId);
    }
}
