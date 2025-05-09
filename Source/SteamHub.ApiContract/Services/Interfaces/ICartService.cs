﻿// <copyright file="ICartService.cs" company="PlaceholderCompany">
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

    public interface ICartService
    {
        Task<List<int>> GetAllCartGamesIdsAsync();

        Task<List<Game>> GetAllPurchasedGamesAsync();

        Task<decimal> GetTotalSumToBePaidAsync();

        Task<List<Game>> GetCartGamesAsync();

        Task RemoveGameFromCartAsync(Game game);

        Task AddGameToCartAsync(Game game);

        Task RemoveGamesFromCartAsync(List<Game> games);

        float GetUserFunds();

        public float GetTheTotalSumOfItemsInCart(List<Game> cartGames);

        User GetUser();
    }
}
