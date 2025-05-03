using Microsoft.EntityFrameworkCore;

namespace SteamHub.Api.Entities
{ 
    [PrimaryKey(nameof(UserId), nameof(ItemId), nameof(GameId))]
    public class UserInventory
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public int GameId { get; set; }
        public DateTime AcquiredDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = false;
        public User User { get; set; }
        public Item Item { get; set; }
        public Game Game { get; set; }
    }
}