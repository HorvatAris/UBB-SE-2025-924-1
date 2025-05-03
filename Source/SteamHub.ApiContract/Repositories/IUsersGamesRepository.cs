using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.UsersGames;
namespace SteamHub.ApiContract.Repositories
{
    public interface IUsersGamesRepository
    {
        Task<GetUserGamesResponse> GetUserGamesAsync(int userId);
        Task<GetUserGamesResponse> GetUserWishlistAsync(int userId);
        Task<GetUserGamesResponse> GetUserCartAsync(int userId);
        Task<GetUserGamesResponse> GetUserPurchasedGamesAsync(int userId);
        Task AddToWishlistAsync(UserGameRequest usersGames);
        Task AddToCartAsync(UserGameRequest usersGames);
        Task PurchaseGameAsync(UserGameRequest usersGames);
        Task RemoveFromWishlistAsync(UserGameRequest usersGames);
        Task RemoveFromCartAsync(UserGameRequest usersGames);
    }
}
