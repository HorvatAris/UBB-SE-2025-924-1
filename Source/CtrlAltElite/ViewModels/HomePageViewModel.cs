// <copyright file="HomePageViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using SteamStore.Constants;
using SteamStore.Models;
using SteamStore.Pages;
using SteamStore.Services.Interfaces;

public class HomePageViewModel : INotifyPropertyChanged
{
    private const int EmptyGameListLength = 0;
    private readonly IGameService gameService;
    private readonly IUserGameService userGameService;
    private readonly ICartService cartService;
    private string searchFilterText;

    public HomePageViewModel(IGameService gameService, IUserGameService userGameService, ICartService cartService)
    {
        this.gameService = gameService;
        this.userGameService = userGameService;
        this.cartService = cartService;
        this.GameService = gameService; // Assign to public property
        this.SearchedOrFilteredGames = new ObservableCollection<Game>();
        this.TrendingGames = new ObservableCollection<Game>();
        this.RecommendedGames = new ObservableCollection<Game>();
        this.DiscountedGames = new ObservableCollection<Game>();
        this.Tags = new ObservableCollection<Tag>();
    }

    public async Task InitAsync()
    {
        await this.LoadAllGames();
        await this.LoadTrendingGames();
        await this.LoadRecommendedGames();
        await this.LoadDiscountedGames();
        await this.LoadTags();

    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Game> SearchedOrFilteredGames { get; set; }

    public ObservableCollection<Game> TrendingGames { get; set; }

    public ObservableCollection<Game> RecommendedGames { get; set; }

    public ObservableCollection<Game> DiscountedGames { get; set; }

    public ObservableCollection<Tag> Tags { get; set; }

    public string Search_filter_text
    {
        get => this.searchFilterText;
        set
        {
            if (this.searchFilterText != value)
            {
                this.searchFilterText = value;
                this.OnPropertyChanged();
            }
        }
    }

    // Expose GameService so it can be accessed by the view
    public IGameService GameService { get; private set; }

    public async Task LoadAllGames()
    {
        this.SearchedOrFilteredGames.Clear();
        this.Search_filter_text = HomePageConstants.ALLGAMESFILTER;
        var games = await this.gameService.GetAllGames();
        foreach (var game in games)
        {
            this.SearchedOrFilteredGames.Add(game);
        }
    }

    public async Task SearchGames(string search_query)
    {
        this.SearchedOrFilteredGames.Clear();
        var filteredGames = await this.gameService.SearchGames(search_query);
        foreach (var game in filteredGames)
        {
            this.SearchedOrFilteredGames.Add(game);
        }

        if (search_query == string.Empty)
        {
            this.Search_filter_text = HomePageConstants.ALLGAMESFILTER;
            return;
        }

        if (filteredGames.Count == EmptyGameListLength)
        {
            this.Search_filter_text = HomePageConstants.NOGAMESFOUND + search_query;
            return;
        }

        this.Search_filter_text = HomePageConstants.SEARCHRESULTSFOR + search_query;
    }

    public async Task FilterGames(int minimumRating, int minimumPrice, int maximumPrice, string[] tags)
    {
        this.SearchedOrFilteredGames.Clear();
        var games = await this.gameService.FilterGames(minimumRating, minimumPrice, maximumPrice, tags);
        foreach (var game in games)
        {
            this.SearchedOrFilteredGames.Add(game);
        }

        if (games.Count == EmptyGameListLength)
        {
            this.Search_filter_text = HomePageConstants.NOGAMESFOUND;
            return;
        }

        this.Search_filter_text = HomePageConstants.FILTEREDGAMES;
    }

    public void SwitchToGamePage(Microsoft.UI.Xaml.DependencyObject parent, Game selectedGame)
    {
        if (parent is Frame frame)
        {
            var gamePage = new GamePage(this.GameService, this.cartService, this.userGameService, selectedGame);
            frame.Content = gamePage;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task LoadTrendingGames()
    {
        this.TrendingGames.Clear();
        var games = await this.gameService.GetTrendingGames();
        foreach (var game in games)
        {
            this.TrendingGames.Add(game);
        }
    }

    private async Task LoadTags()
    {
        var tagsList = await this.gameService.GetAllTags();
        System.Diagnostics.Debug.WriteLine("$TAGS" + tagsList);
        foreach (var tag in tagsList)
        {
            this.Tags.Add(tag);
        }
    }

    private async Task LoadRecommendedGames()
    {
        this.RecommendedGames.Clear();
        var reccomendedGames = await this.userGameService.GetRecommendedGames();
        foreach (var game in reccomendedGames)
        {
            this.RecommendedGames.Add(game);
        }
    }

    private async Task LoadDiscountedGames()
    {
        this.DiscountedGames.Clear();
        var discountedGames = await this.gameService.GetDiscountedGames();
        foreach (var game in discountedGames)
        {
            this.DiscountedGames.Add(game);
        }
    }
}