// <copyright file="CartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtrlAltElite.ServiceProxies;
using CtrlAltElite.Services;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.UsersGames;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services.Interfaces;

public class CartService : ICartService
{
    private const int InitialZeroSum = 0;
    private ICartServiceProxy cartServiceProxy;
    private User user;
    private IGameServiceProxy gameServiceProxy;

    public CartService( ICartServiceProxy serviceProxy, User user,IGameServiceProxy gserviceProxy)
    {
        this.cartServiceProxy = serviceProxy;
        this.gameServiceProxy = gserviceProxy;

        this.user = user;
    }

    public async Task<List<Game>> GetCartGames()
    {
        try
        {
            var response = await this.cartServiceProxy.GetUserCartAsync(this.user.UserId);
            var userGamesResponses = response.UserGames; // Access the actual list her
            System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses.Count}");
            var gameIds = userGamesResponses
        .Select(g => g.GameId)
        .ToList();
            if (gameIds.Count == 0)
                return new List<Game>();
            var games = new List<Game>();
            foreach (var gameId in gameIds)
            {

                System.Diagnostics.Debug.WriteLine($"GameId: {gameId}");
                var game = GameMapper.MapToGame(await this.gameServiceProxy.GetGameByIdAsync(gameId));
                games.Add(game);
            }
            return games;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching user games: {ex.Message}");
            return new List<Game>();
        }
        //System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses}");

    }

    public async Task<List<int>> GetAllPurchasedGames()
    {
        try
        {
            var response = await this.cartServiceProxy.GetUserGamesAsync(this.user.UserId);
            var userGamesResponses = response.UserGames; // Access the actual list here
            var gameIds = userGamesResponses
        .Where(g => g.IsPurchased)
        .Select(g => g.GameId)
        .ToList();
            return gameIds;
            //System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {ex.Message}");
            return new List<int>();
        }
    }
    public async Task<List<int>> GetAllCartGamesIds()
    {
        try
        {
            var response = await this.cartServiceProxy.GetUserCartAsync(this.user.UserId);
            var userGamesResponses = response.UserGames; // Access the actual list here
            var gameIds = userGamesResponses
        .Where(g => g.IsInCart)
        .Select(g => g.GameId)
        .ToList();
            return gameIds;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {ex.Message}");
            return new List<int>();
        }
    }

    //public void RemoveGameFromCart(Game game)
    //{
    //    this.cartRepository.RemoveGameFromCart(game);
    //}
    public async Task RemoveGameFromCart(Game game)
    {
        try
        {
            var request = new UserGameRequest
            {
                UserId = this.user.UserId,
                GameId = game.GameId
            };

            await this.cartServiceProxy.RemoveFromCartAsync(request);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error removing game from cart: {ex.Message}");
        }
    }


    //public void AddGameToCart(Game game)
    //{
    //    this.cartRepository.AddGameToCart(game);
    //}
    //public async Task AddGameToCart(Game game)
    //{
    //    try
    //    {
    //        // Create the UserGameRequest object with userId and gameId
    //        System.Diagnostics.Debug.WriteLine($"UserId: {this.user.UserId}, GameId: {game.GameId}");

    //        var request = new UserGameRequest
    //        {
    //            UserId = this.user.UserId, // Get the userId from the current user object
    //            GameId = game.GameId, // GameId is the identifier for the game to be added
    //        };

    //        // Call the proxy method to add the game to the cart
    //        await this.cartServiceProxy.AddToCartAsync(request);
    //    }
    //    catch (Exception ex)
    //    {
    //        System.Diagnostics.Debug.WriteLine($"Error adding game to cart: {ex.Message}");
    //    }
    //}
    public async Task AddGameToCart(Game game)
    {
        var purchasedGamesIds = await this.GetAllPurchasedGames();
        var cartGamesIds = await this.GetAllCartGamesIds();
        foreach (var gameId in purchasedGamesIds)
        {
            if(game.GameId == gameId)
            {
                //System.Diagnostics.Debug.WriteLine("The game is already purchased.");
                throw new Exception("The game is already purchased.");
            }
        }
        foreach(var gameId in cartGamesIds)
        {
            if (game.GameId == gameId)
            {
                //System.Diagnostics.Debug.WriteLine("The game is already in the cart.");
                throw new Exception("The game is already in the cart.");
            }
        }


        //var userGames = games.UserGames; // Access the actual list here
        //foreach (var userGame in userGames)
        //{
        //    System.Diagnostics.Debug.WriteLine($" GameId: {userGame.GameId}, IsInCart: {userGame.IsInCart}, IsPurchased: {userGame.IsPurchased}");
        //    if (userGame.IsInCart)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Exception thrown!");
        //        throw new Exception("The game is already in the cart.");
        //    }
        //    if (userGame.IsPurchased)
        //    {
        //        throw new Exception("The game is already purchased.");
        //    }
        //}
        var request = new UserGameRequest
        {
            UserId = this.user.UserId,
            GameId = game.GameId,
        };

        await this.cartServiceProxy.AddToCartAsync(request);
    }

    public async Task RemoveGamesFromCart(List<Game> games)
    {
        foreach (var game in games)
        {

            this.RemoveGameFromCart(game);
        }
    }

    public float GetUserFunds()
    {
        //return this.cartRepository.GetUserFunds();
        return this.user.WalletBalance;
    }

    public async Task<decimal> GetTotalSumToBePaidAsync()
    {
        decimal totalSumToBePaid = InitialZeroSum;
        var cartGames = await this.GetCartGames();

        foreach (var game in cartGames)
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