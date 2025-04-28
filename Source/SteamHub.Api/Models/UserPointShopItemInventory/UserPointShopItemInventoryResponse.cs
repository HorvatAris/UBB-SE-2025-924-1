namespace SteamHub.Api.Models.UserPointShopItemInventory
{
    public class UserPointShopItemInventoryResponse
    {
        public int PointShopItemId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsActive { get; set; }
    }
}
