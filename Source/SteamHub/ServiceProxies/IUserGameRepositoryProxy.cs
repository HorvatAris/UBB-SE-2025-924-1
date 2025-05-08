// <copyright file="IUserGameServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ServiceProxies
{
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Repositories;

    public interface IUserGameRepositoryProxy : IUsersGamesRepository
    {
        [Get("/api/UsersGames/{userId}")]
        Task<GetUserGamesResponse> GetUserGamesAsync(int userId); // GetAllUserGames

        [Get("/api/UsersGames/Wishlist/{userId}")]
        Task<GetUserGamesResponse> GetUserWishlistAsync(int userId); // GetWishlistGames

        [Post("/api/UsersGames/AddToWishlist")]
        Task AddToWishlistAsync(UserGameRequest usersGames); // AddGameToWishlist

        [Patch("/api/UsersGames/RemoveFromWishlist")]
        Task RemoveFromWishlistAsync(UserGameRequest usersGames); // RemoveGameFromWishlist

        [Post("/api/UsersGames/Purchased")]
        Task PurchaseGameAsync(UserGameRequest usersGames); // AddGameToPurchased

        [Get("/api/UsersGames/Cart/{userId}")]
        Task<GetUserGamesResponse> GetUserCartAsync(int userId);

        [Post("/api/UsersGames/AddToCart")]
        Task AddToCartAsync([Body] UserGameRequest request);

        [Patch("/api/UsersGames/RemoveFromCart")]
        Task RemoveFromCartAsync([Body] UserGameRequest request);

        [Get("/api/UsersGames/Purchased/{userId}")]
        Task<GetUserGamesResponse> GetUserPurchasedGamesAsync(int userId);
    }
}
