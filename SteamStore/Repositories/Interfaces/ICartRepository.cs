// <copyright file="ICartRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICartRepository
    {
        List<Game> GetCartGames();

        void AddGameToCart(Game game);

        void RemoveGameFromCart(Game game);

        float GetUserFunds();
    }
}
