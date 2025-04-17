// <copyright file="CartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services.Interfaces;

public class CartService : ICartService
{
    private const int InitialZeroSum = 0;
    private ICartRepository cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public List<Game> GetCartGames()
    {
        return this.cartRepository.GetCartGames();
    }

    public void RemoveGameFromCart(Game game)
    {
        this.cartRepository.RemoveGameFromCart(game);
    }

    public void AddGameToCart(Game game)
    {
        this.cartRepository.AddGameToCart(game);
    }

    public void RemoveGamesFromCart(List<Game> games)
    {
        foreach (var game in games)
        {
            this.cartRepository.RemoveGameFromCart(game);
        }
    }

    public float GetUserFunds()
    {
        return this.cartRepository.GetUserFunds();
    }

    public decimal GetTotalSumToBePaid()
    {
        decimal totalSumToBePaid = InitialZeroSum;

        foreach (var game in this.cartRepository.GetCartGames())
        {
            totalSumToBePaid += (decimal)game.Price;
        }

        return totalSumToBePaid;
    }

    public float GetTheTotalSumOfItemsInCart(List<Game> cartGames)
    {
        float totalSum = InitialZeroSum;
        foreach (var game in cartGames)
        {
            totalSum += (float)game.Price;
        }

        return totalSum;
    }
}