namespace SteamHub.Api.Models.UserPointShopItemInventory
{
    public class UpdateUserPointShopItemInventoryRequest
    {
        public int UserId { get; set; }
        public int PointShopItemId { get; set; }

        public bool IsActive { get; set; } 
    }
}
