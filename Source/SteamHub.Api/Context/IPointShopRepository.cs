using SteamHub.Api.Entities;

namespace SteamHub.Api.Context
{
    public interface IPointShopRepository
    {
        Task ActivateItemAsync(User user, PointShopItem item);
        Task DeactivateItemAsync(User user, PointShopItem item);
        Task<List<PointShopItem>> GetAllItemsAsync();
        Task<List<PointShopItem>> GetUserItemsAsync(int userId);
        Task PurchaseItemAsync(User user, PointShopItem item);
        Task UpdateUserPointBalanceAsync(User user);
    }
}