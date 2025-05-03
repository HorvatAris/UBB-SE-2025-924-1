namespace SteamHub.ApiContract.Models.UsersGames
{
    public class UpdateUserGamesRequest
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        public bool IsInWishlist { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsInCart { get; set; }
    }
}