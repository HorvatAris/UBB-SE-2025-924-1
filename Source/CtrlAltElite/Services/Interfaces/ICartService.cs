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
        List<Game> GetCartGames();

        void RemoveGameFromCart(Game game);

        void AddGameToCart(Game game);

        void RemoveGamesFromCart(List<Game> games);

        float GetUserFunds();

        decimal GetTotalSumToBePaid();

        public float GetTheTotalSumOfItemsInCart(List<Game> cartGames);
    }
}
