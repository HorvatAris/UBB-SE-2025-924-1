using SteamHub.Api.Models;

namespace SteamHub.Api.Context
{
    public interface IPointShopItemRepository
    {
        Task<CreatePointShopItemResponse> CreatePointShopItemAsync(CreatePointShopItemRequest request);
        Task DeletePointShopItemAsync(int id);
        Task<PointShopItemResponse?> GetPointShopItemByIdAsync(int id);
        Task<GetPointShopItemsResponse?> GetPointShopItemsAsync();
        Task UpdatePointShopItemAsync(int itemId, UpdatePointShopItemRequest request);
    }
}