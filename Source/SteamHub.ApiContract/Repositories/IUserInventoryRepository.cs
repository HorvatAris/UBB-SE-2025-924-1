using SteamHub.ApiContract.Models.UserInventory;

namespace SteamHub.ApiContract.Repositories
{
    public interface IUserInventoryRepository
    {
        Task<UserInventoryResponse> GetUserInventoryAsync(int userId);
        Task<InventoryItemResponse?> GetItemFromUserInventoryAsync(int userId, int itemId);
        Task AddItemToUserInventoryAsync(ItemFromInventoryRequest request);
        Task RemoveItemFromUserInventoryAsync(ItemFromInventoryRequest request);
    }
}
