using SteamHub.ApiContract.Models.UsersGames;

namespace SteamHub.ApiContract.Models.UsersGames
{
    public class GetUserGamesResponse
    {
        public IList<UserGamesResponse> UserGames { get; set; }
    }
}