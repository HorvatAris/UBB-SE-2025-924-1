using Refit;
using SteamHub.ApiContract.Models.UsersGames;
using SteamStore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ServiceProxies
{
    public interface ICartServiceProxy : ICartRepository
    {
        [Get("/api/UsersGames/Cart/{userId}")]
        Task<GetUserGamesResponse> GetUserCartAsync(int userId);

        [Get("/api/UsersGames/all/{userId}")]
        Task<GetUserGamesResponse> GetUserGamesAsync(int userId);

        [Post("/api/UsersGames/AddToCart")]
        Task AddToCartAsync([Body] UserGameRequest request);

        [Patch("/api/UsersGames/RemoveFromCart")]
        Task RemoveFromCartAsync([Body] UserGameRequest request);

    }
}
