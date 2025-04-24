// <copyright file="GamePageViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using SteamStore.Models;
using SteamStore.Pages;
using SteamStore.Services.Interfaces;

public class GamePageViewModel : INotifyPropertyChanged
{
    private const int MaxSimilarGamesToDisplay = 3;
    private const string CurrencySymbol = "$";
    private const string PriceFormat = "F2";
    private readonly ICartService cartService;
    private readonly IUserGameService userGameService;
    private readonly IGameService gameService;

    private Game game;
    private ObservableCollection<Game> similarGames;
    private bool isOwned;
    private ObservableCollection<string> gameTags;

    public GamePageViewModel(IGameService gameService, ICartService cartService, IUserGameService userGameService)
    {
        this.cartService = cartService;
        this.userGameService = userGameService;
        this.gameService = gameService;
        this.SimilarGames = new ObservableCollection<Game>();
        this.GameTags = new ObservableCollection<string>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public Game Game
    {
        get => this.game;
        set
        {
            this.game = value;
            this.OnPropertyChanged();
            this.UpdateIsOwnedStatus();
            this.UpdateGameTags();
        }
    }

    // public string FormattedPrice => this.Game != null ? $"${this.Game.Price:F2}" : string.Empty;
    public string FormattedPrice => this.Game != null ? $"{CurrencySymbol}{this.Game.Price.ToString(PriceFormat)}" : string.Empty;

    public ObservableCollection<string> GameTags
    {
        get => this.gameTags;
        private set
        {
            this.gameTags = value;
            this.OnPropertyChanged();
        }
    }

    public bool IsOwned
    {
        get => this.isOwned;
        private set
        {
            if (this.isOwned != value)
            {
                this.isOwned = value;
                this.OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Game> SimilarGames
    {
        get => this.similarGames;
        set
        {
            this.similarGames = value;
            this.OnPropertyChanged();
        }
    }

    public void LoadGame(Game game)
    {
        this.Game = game;
        this.LoadSimilarGames();
    }

    public void LoadGameById(int gameId)
    {
        if (this.gameService == null)
        {
            return;
        }

        this.Game = this.gameService.GetGameById(gameId);
        if (this.Game != null)
        {
            this.LoadSimilarGames();
        }
    }

    // Add game to cart - safely handle null CartService
    public void AddToCart()
    {
        if (this.Game != null && this.cartService != null)
        {
            try
            {
                this.cartService.AddGameToCart(this.Game);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }

    // Add game to wishlist - this will be implemented later
    public void AddToWishlist()
    {
        if (this.Game != null && this.userGameService != null)
        {
            try
            {
                this.userGameService.AddGameToWishlist(this.Game);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }

    public void GetSimilarGames(Game game, Frame frame)
    {
        if (frame != null)
        {
            var gamePage = new GamePage(this.gameService, this.cartService, this.userGameService, game);

            frame.Content = gamePage;
        }
        else
        {
            frame.Navigate(typeof(GamePage), game);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UpdateIsOwnedStatus()
    {
        if (this.Game == null || this.userGameService == null)
        {
            this.IsOwned = false;
            return;
        }

        try
        {
            this.IsOwned = this.userGameService.IsGamePurchased(this.Game);
        }
        catch (Exception)
        {
            this.IsOwned = false;
        }
    }

    private void UpdateGameTags()
    {
        if (this.Game == null || this.gameService == null)
        {
            this.GameTags.Clear();
            return;
        }

        try
        {
            var allTags = this.gameService.GetAllGameTags(this.Game);
            this.GameTags.Clear();
            foreach (var tag in allTags)
            {
                this.GameTags.Add(tag.Tag_name);
            }
        }
        catch (Exception)
        {
            this.GameTags.Clear();
        }
    }

    // Load similar games based on current game
    private void LoadSimilarGames()
    {
        if (this.Game == null || this.gameService == null)
        {
            return;
        }

        var similarGames = this.gameService.GetSimilarGames(this.Game.GameId);
        this.SimilarGames = new ObservableCollection<Game>(similarGames.Take(MaxSimilarGamesToDisplay));
    }
}