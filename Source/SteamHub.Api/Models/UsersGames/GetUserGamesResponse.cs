using SteamHub.Api.Models.UserPointShopItemInventory;

namespace SteamHub.Api.Models.UsersGames
{
    public class GetUserGamesResponse
    {
        public IList<UserGamesResponse> UserGames { get; set; }
    }
}