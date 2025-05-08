// <copyright file="CartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamHub.Models;
using SteamHub.ServiceProxies;
using SteamHub.Services;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.Services.Interfaces;
using SteamHub.ApiContract.Repositories;

public class CartService : ICartService
{
    private const int InitialZeroSum = 0;
    private IUsersGamesRepository userGameRepository;
    private User user;
    private IGameRepository gameRepository;

    public CartService(IUsersGamesRepository userGameRepository, User user, IGameRepository gameRepository)
    {
        this.userGameRepository = userGameRepository;
        this.gameRepository = gameRepository;

        this.user = user;
    }

    public async Task<List<Game>> GetCartGamesAsync()
    {
        try
        {
            var response = await this.userGameRepository.GetUserCartAsync(this.user.UserId);
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

    public async Task<List<Game>> GetAllPurchasedGamesAsync()
    {
        var purchasedGames = new List<Game>();
        try
        {
            var response = await this.userGameRepository.GetUserPurchasedGamesAsync(this.user.UserId);
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

    public async Task<List<int>> GetAllCartGamesIdsAsync()
    {
        try
        {
            var response = await this.userGameRepository.GetUserCartAsync(this.user.UserId);
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

    public async Task RemoveGameFromCartAsync(Game game)
    {
        try
        {
            var request = new UserGameRequest
            {
                UserId = this.user.UserId,
                GameId = game.GameId,
            };

            await this.userGameRepository.RemoveFromCartAsync(request);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error removing game from cart: {exception.Message}");
        }
    }

    public async Task AddGameToCartAsync(Game game)
    {
        var purchasedGames = await this.GetAllPurchasedGamesAsync();
        var cartGamesIds = await this.GetAllCartGamesIdsAsync();
        foreach (var purchasedGame in purchasedGames)
        {
            if (game.GameId == purchasedGame.GameId)
            {
                // System.Diagnostics.Debug.WriteLine("The game is already purchased.");
                throw new Exception("The game is already purchased.");
            }
        }

        foreach (var gameId in cartGamesIds)
        {
            if (game.GameId == gameId)
            {
                // System.Diagnostics.Debug.WriteLine("The game is already in the cart.");
                throw new Exception("The game is already in the cart.");
            }
        }

        var request = new UserGameRequest
        {
            UserId = this.user.UserId,
            GameId = game.GameId,
        };

        await this.userGameRepository.AddToCartAsync(request);
    }

    public async Task RemoveGamesFromCartAsync(List<Game> games)
    {
        foreach (var game in games)
        {
            await this.RemoveGameFromCartAsync(game);
        }
    }

    public float GetUserFunds()
    {
        return this.user.WalletBalance;
    }

    public async Task<decimal> GetTotalSumToBePaidAsync()
    {
        decimal totalSumToBePaid = InitialZeroSum;
        var cartGames = await this.GetCartGamesAsync();

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