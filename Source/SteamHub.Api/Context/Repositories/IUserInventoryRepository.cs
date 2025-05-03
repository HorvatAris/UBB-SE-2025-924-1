using SteamHub.Api.Models.UserInventory;

namespace SteamHub.Api.Context.Repositories
{
    public interface IUserInventoryRepository
    {
        Task<UserInventoryResponse> GetUserInventoryAsync(int userId);
        Task<InventoryItemResponse?> GetItemFromUserInventoryAsync(int userId, int itemId);
        Task AddItemToUserInventoryAsync(ItemFromInventoryRequest request);
        Task RemoveItemFromUserInventoryAsync(ItemFromInventoryRequest request);
    }
}
