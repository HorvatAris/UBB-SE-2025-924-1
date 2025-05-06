// <copyright file="HomePageViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using SteamHub.Constants;
using SteamHub.Models;
using SteamHub.Pages;
using SteamHub.Services.Interfaces;

public class HomePageViewModel : INotifyPropertyChanged
{
    private const int EmptyGameListLength = 0;
    private readonly IGameService gameService;
    private readonly IUserGameService userGameService;
    private readonly ICartService cartService;

    private string searchFilterText;
    private int ratingFilter;
    private int minPrice;
    private int maxPrice;
    private ObservableCollection<string> selectedTags;

    public HomePageViewModel(IGameService gameService, IUserGameService userGameService, ICartService cartService)
    {
        this.gameService = gameService;
        this.userGameService = userGameService;
        this.cartService = cartService;

        this.SearchedOrFilteredGames = new ObservableCollection<Game>();
        this.TrendingGames = new ObservableCollection<Game>();
        this.RecommendedGames = new ObservableCollection<Game>();
        this.DiscountedGames = new ObservableCollection<Game>();
        this.Tags = new ObservableCollection<Tag>();
        this.selectedTags = new ObservableCollection<string>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Game> SearchedOrFilteredGames { get; set; }

    public ObservableCollection<Game> TrendingGames { get; set; }

    public ObservableCollection<Game> RecommendedGames { get; set; }

    public ObservableCollection<Game> DiscountedGames { get; set; }

    public ObservableCollection<Tag> Tags { get; set; }

    public ObservableCollection<string> SelectedTags
    {
        get => this.selectedTags;
        set
        {
            this.selectedTags = value;
            this.OnPropertyChanged();
        }
    }

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

    public int RatingFilter
    {
        get => this.ratingFilter;
        set
        {
            this.ratingFilter = value;
            this.OnPropertyChanged();
        }
    }

    public int MinPrice
    {
        get => this.minPrice;
        set
        {
            this.minPrice = value;
            this.OnPropertyChanged();
        }
    }

    public int MaxPrice
    {
        get => this.maxPrice;
        set
        {
            this.maxPrice = value;
            this.OnPropertyChanged();
        }
    }

    public async Task InitAsync()
    {
        await this.LoadAllGames();
        await this.LoadTrendingGames();
        await this.LoadRecommendedGames();
        await this.LoadDiscountedGames();
        await this.LoadTags();
    }

    public async Task LoadAllGames()
    {
        this.SearchedOrFilteredGames.Clear();
        this.Search_filter_text = HomePageConstants.ALLGAMESFILTER;
        var games = await this.gameService.GetAllApprovedGamesAsync();
        foreach (var game in games)
        {
            this.SearchedOrFilteredGames.Add(game);
        }
    }

    public async Task SearchGames(string search_query)
    {
        this.SearchedOrFilteredGames.Clear();
        var filteredGames = await this.gameService.SearchGamesAsync(search_query);
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

    public async Task ApplyFilters()
    {
        this.SearchedOrFilteredGames.Clear();
        var games = await this.gameService.FilterGamesAsync(this.RatingFilter, this.MinPrice, this.MaxPrice, this.SelectedTags.ToArray());

        foreach (var game in games)
        {
            this.SearchedOrFilteredGames.Add(game);
        }

        this.Search_filter_text = games.Count == 0
            ? HomePageConstants.NOGAMESFOUND
            : HomePageConstants.FILTEREDGAMES;
    }

    public void ResetFilters()
    {
        this.RatingFilter = 0;
        this.MinPrice = 0;
        this.MaxPrice = 200;
        this.SelectedTags.Clear();
        this.LoadAllGames();
    }

    public void NavigateToGamePage(Frame parentFrame, Game selectedGame)
    {
        if (parentFrame != null && selectedGame != null)
        {
            var gamePage = new GamePage(this.gameService, this.cartService, this.userGameService, selectedGame);
            parentFrame.Content = gamePage;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task LoadTrendingGames()
    {
        this.TrendingGames.Clear();
        var trendingGames = await this.gameService.GetTrendingGamesAsync();
        foreach (var game in trendingGames)
        {
            this.TrendingGames.Add(game);
        }
    }

    private async Task LoadTags()
    {
        this.Tags.Clear();
        var tagsList = await this.gameService.GetAllTagsAsync();
        foreach (var tag in tagsList)
        {
            this.Tags.Add(tag);
        }
    }

    private async Task LoadRecommendedGames()
    {
        this.RecommendedGames.Clear();
        var reccomendedGames = await this.userGameService.GetRecommendedGamesAsync();
        foreach (var game in reccomendedGames)
        {
            this.RecommendedGames.Add(game);
        }
    }

    private async Task LoadDiscountedGames()
    {
        this.DiscountedGames.Clear();
        var discountedGames = await this.gameService.GetDiscountedGamesAsync();
        foreach (var game in discountedGames)
        {
            this.DiscountedGames.Add(game);
        }
    }
}