// <copyright file="ICartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UsersGames;

    public interface ICartService
    {
        Task<List<int>> GetAllCartGamesIdsAsync(int userId);

        Task<List<Game>> GetAllPurchasedGamesAsync(int userId);

        Task<decimal> GetTotalSumToBePaidAsync();

        Task<List<Game>> GetCartGamesAsync(int userId);

        Task RemoveGameFromCartAsync(UserGameRequest gameRequest);

        Task AddGameToCartAsync(UserGameRequest gameRequest);

        Task RemoveGamesFromCartAsync(List<Game> games);

        float GetUserFunds();

        public float GetTheTotalSumOfItemsInCart(List<Game> cartGames);

        User GetUser();
    }
}
