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
        Task<GetUserGamesResponse> GetUserGamesAsync(int userId); // GetAllUserGames
        Task<GetUserGamesResponse> GetUserWishlistAsync(int userId); // GetWishlistGames
        Task<GetUserGamesResponse> GetUserCartAsync(int userId);
        Task<GetUserGamesResponse> GetUserPurchasedGamesAsync(int userId);
        Task AddToWishlistAsync(UserGameRequest usersGames); // AddGameToWishlist
        Task AddToCartAsync(UserGameRequest usersGames);
        Task PurchaseGameAsync(UserGameRequest usersGames); // AddGameToPurchased
        Task RemoveFromWishlistAsync(UserGameRequest usersGames); // RemoveGameFromWishlist
        Task RemoveFromCartAsync(UserGameRequest usersGames);
    }
}
