// <copyright file="ICartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICartService
    {
        Task<List<int>> GetAllCartGamesIds();

        Task<List<Game>> GetAllPurchasedGames();

        Task<decimal> GetTotalSumToBePaidAsync();

        Task<List<Game>> GetCartGames();

        Task RemoveGameFromCart(Game game);

        Task AddGameToCart(Game game);

        Task RemoveGamesFromCart(List<Game> games);

        float GetUserFunds();

        public float GetTheTotalSumOfItemsInCart(List<Game> cartGames);
    }
}
