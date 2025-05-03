namespace SteamHub.ApiContract.Repositories
{
    using SteamHub.ApiContract.Models.PointShopItem;

    public interface IPointShopItemRepository
    {
        Task<CreatePointShopItemResponse> CreatePointShopItemAsync(CreatePointShopItemRequest request);
        Task DeletePointShopItemAsync(int id);
        Task<PointShopItemResponse?> GetPointShopItemByIdAsync(int id);
        Task<GetPointShopItemsResponse?> GetPointShopItemsAsync();
        Task UpdatePointShopItemAsync(int itemId, UpdatePointShopItemRequest request);
    }
}