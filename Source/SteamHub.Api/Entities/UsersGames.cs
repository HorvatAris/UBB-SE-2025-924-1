using Microsoft.EntityFrameworkCore;

namespace SteamHub.Api.Entities
{
    [PrimaryKey(nameof(UserId), nameof(GameId))]
    public class UsersGames
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        public bool IsInWishlist { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsInCart { get; set; }
        public User User { get; set; }
        public Game Game { get; set; }
    }
}