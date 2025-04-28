using Microsoft.EntityFrameworkCore;

namespace SteamHub.Api.Entities
{
    [PrimaryKey(nameof(UserId), nameof(PointShopItemId))]
    public class UserPointShopItemInventory
    {
        public int UserId { get; set; }

        public int PointShopItemId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = false;

        public User User { get; set; }
        public PointShopItem PointShopItem { get; set; }
    }
}
