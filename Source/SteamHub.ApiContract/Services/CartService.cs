// <copyright file="CartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;

public class CartService : ICartService
{
    private const int InitialZeroSum = 0;
    private IUsersGamesRepository userGameRepository;
    private IGameRepository gameRepository;

    public CartService(IUsersGamesRepository userGameRepository, IGameRepository gameRepository)
    {
        this.userGameRepository = userGameRepository;
        this.gameRepository = gameRepository;
    }

    public async Task<List<Game>> GetCartGamesAsync(int userId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"UserId: {userId}");
            var response = await this.userGameRepository.GetUserCartAsync(userId);
            var userGamesResponses = response.UserGames; // Access the actual list her
            System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses.Count}");
            var gameIds = userGamesResponses
        .Select(game => game.GameId)
        .ToList();
            if (gameIds.Count == 0)
            {
                return new List<Game>();
            }

            var games = new List<Game>();
            foreach (var gameId in gameIds)
            {
                System.Diagnostics.Debug.WriteLine($"GameId: {gameId}");
                var game = GameMapper.MapToGame(await this.gameRepository.GetGameByIdAsync(gameId));
                games.Add(game);
            }

            return games;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
            return new List<Game>();
        }
    }

    public async Task<List<Game>> GetAllPurchasedGamesAsync(int userId)
    {
        var purchasedGames = new List<Game>();
        try
        {
            var response = await this.userGameRepository.GetUserPurchasedGamesAsync(userId);
            var userGamesResponses = response.UserGames; // Access the actual list here
            foreach (var userGame in userGamesResponses)
            {
                var actualGame = await this.gameRepository.GetGameByIdAsync(userGame.GameId);
                var game = GameMapper.MapToGame(actualGame);
                purchasedGames.Add(game);
            }

            return purchasedGames;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
            return new List<Game>();
        }
    }

    public async Task<List<int>> GetAllCartGamesIdsAsync(int userId)
    {
        try
        {
            var response = await this.userGameRepository.GetUserCartAsync(userId);
            var userGamesResponses = response.UserGames; // Access the actual list here
            var gameIds = userGamesResponses
        .Where(currentGame => currentGame.IsInCart)
        .Select(currentGame => currentGame.GameId)
        .ToList();
            return gameIds;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
            return new List<int>();
        }
    }

    public async Task RemoveGameFromCartAsync(UserGameRequest gameRequest)
    {
        try
        {
            await this.userGameRepository.RemoveFromCartAsync(gameRequest);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error removing game from cart: {exception.Message}");
        }
    }

    public async Task AddGameToCartAsync(UserGameRequest gameRequest)
    {
        var purchasedGames = await this.GetAllPurchasedGamesAsync(gameRequest.UserId);
        var cartGames = await this.GetCartGamesAsync(gameRequest.UserId);

        foreach (var purchasedGame in purchasedGames)
        {
            if (gameRequest.GameId == purchasedGame.GameId)
            {
                // System.Diagnostics.Debug.WriteLine("The game is already purchased.");
                throw new Exception("The game is already purchased.");
            }
        }

        foreach (var cartGame in cartGames)
        {
            if (gameRequest.GameId == cartGame.GameId)
            {
                // System.Diagnostics.Debug.WriteLine("The game is already in the cart.");
                throw new Exception("The game is already in the cart.");
            }
        }

        System.Diagnostics.Debug.WriteLine("user id for adding to cart" + gameRequest.UserId);

        await this.userGameRepository.AddToCartAsync(gameRequest);
    }

    public Task RemoveGamesFromCartAsync(List<Game> games)
    {
        throw new NotImplementedException();
    }

    public float GetUserFunds()
    {
        throw new NotImplementedException();
    }

    public async Task<decimal> GetTotalSumToBePaidAsync()
    {
        throw new NotImplementedException();
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
    public User GetUser()
    {
        throw new NotImplementedException();
    }
}