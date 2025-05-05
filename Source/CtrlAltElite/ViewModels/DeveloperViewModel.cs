// <copyright file="DeveloperViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CtrlAltElite.Models;
using Microsoft.UI.Xaml.Controls;
using SteamStore;
using SteamStore.Constants;
using SteamStore.Models;
using SteamStore.Services.Interfaces;
using Windows.Gaming.Input;

public class DeveloperViewModel : INotifyPropertyChanged
{
    private readonly IDeveloperService developerService;
    private string editGameId;
    private string editGameName;
    private string editGameDescription;
    private string editGamePrice;
    private string editGameImageUrl;
    private string editGameplayUrl;
    private string editTrailerUrl;
    private string editGameMinReq;
    private string editGameRecReq;
    private string editGameDiscount;
    private string addGameId;
    private string addGameName;
    private string addGameDescription;
    private string addGamePrice;
    private string addGameImageUrl;
    private string addGameplayUrl;
    private string addTrailerUrl;
    private string addGameMinReq;
    private string addGameRecReq;
    private string addGameDiscount;
    private string ownerCountText;
    private string rejectReason;

    private string rejectionMessage;
    private string pageTitle;

    public DeveloperViewModel(IDeveloperService developerService)
    {
        this.developerService = developerService;
        this.DeveloperGames = new ObservableCollection<Game>();
        this.UnvalidatedGames = new ObservableCollection<Game>();
        this.Tags = new ObservableCollection<Tag>();
        this.LoadGames();
        this.LoadTags();
        this.PageTitle = DeveloperPageTitles.MYGAMES;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Game> DeveloperGames { get; set; }

    public ObservableCollection<Game> UnvalidatedGames { get; set; }

    public ObservableCollection<Tag> Tags { get; set; }

    public string EditGameId
    {
        get => this.editGameId;
        set
        {
            this.editGameId = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameName
    {
        get => this.editGameName;
        set
        {
            this.editGameName = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameDescription
    {
        get => this.editGameDescription;
        set
        {
            this.editGameDescription = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGamePrice
    {
        get => this.editGamePrice;
        set
        {
            this.editGamePrice = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameImageUrl
    {
        get => this.editGameImageUrl;
        set
        {
            this.editGameImageUrl = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameplayUrl
    {
        get => this.editGameplayUrl;
        set
        {
            this.editGameplayUrl = value;
            this.OnPropertyChanged();
        }
    }

    public string EditTrailerUrl
    {
        get => this.editTrailerUrl;
        set
        {
            this.editTrailerUrl = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameMinReq
    {
        get => this.editGameMinReq;
        set
        {
            this.editGameMinReq = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameRecReq
    {
        get => this.editGameRecReq;
        set
        {
            this.editGameRecReq = value;
            this.OnPropertyChanged();
        }
    }

    public string EditGameDiscount
    {
        get => this.editGameDiscount;
        set
        {
            this.editGameDiscount = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameId
    {
        get => this.addGameId;
        set
        {
            this.addGameId = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameName
    {
        get => this.addGameName;
        set
        {
            this.addGameName = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameDescription
    {
        get => this.addGameDescription;
        set
        {
            this.addGameDescription = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGamePrice
    {
        get => this.addGamePrice;
        set
        {
            this.addGamePrice = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameImageUrl
    {
        get => this.addGameImageUrl;
        set
        {
            this.addGameImageUrl = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameplayUrl
    {
        get => this.addGameplayUrl;
        set
        {
            this.addGameplayUrl = value;
            this.OnPropertyChanged();
        }
    }

    public string AddTrailerUrl
    {
        get => this.addTrailerUrl;
        set
        {
            this.addTrailerUrl = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameMinimumRequirement
    {
        get => this.addGameMinReq;
        set
        {
            this.addGameMinReq = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameRecommendedRequirement
    {
        get => this.addGameRecReq;
        set
        {
            this.addGameRecReq = value;
            this.OnPropertyChanged();
        }
    }

    public string AddGameDiscount
    {
        get => this.addGameDiscount;
        set
        {
            this.addGameDiscount = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current user is a developer. Used for view binding.
    /// </summary>
    public bool IsDeveloper
    {
        get => this.CheckIfUserIsADeveloper();
    }

    /// <summary>
    /// Gets or sets reason for rejecting a game. Used for view binding.
    /// </summary>
    public string RejectReason
    {
        get => this.rejectReason;
        set
        {
            if (this.rejectReason != value)
            {
                this.rejectReason = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string RejectionMessage
    {
        get => this.rejectionMessage;
        set
        {
            if (this.rejectionMessage != value)
            {
                this.rejectionMessage = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string OwnerCountText
    {
        get => this.ownerCountText;
        set
        {
            if (this.ownerCountText != value)
            {
                this.ownerCountText = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string PageTitle
    {
        get => this.pageTitle;
        set
        {
            if (this.pageTitle != value)
            {
                this.pageTitle = value;
                this.OnPropertyChanged();
            }
        }
    }

    public Game GetGameByIdInDeveloperGameList(int gameId)
    {
        return this.developerService.FindGameInObservableCollectionById(gameId, this.DeveloperGames);
    }

    public async Task LoadGames()
    {
        this.DeveloperGames.Clear();
        var games = await this.developerService.GetDeveloperGames();
        foreach (var game in games)
        {
            this.DeveloperGames.Add(game);
        }

        this.OnPropertyChanged();
    }

    public void ValidateGame(int game_id)
    {
        this.developerService.ValidateGame(game_id);
    }

    public bool CheckIfUserIsADeveloper()
    {
        return this.developerService.GetCurrentUser().UserRole == User.Role.Developer;
    }

    public void CreateGame(Game game, IList<Tag> selectedTags)
    {
        this.developerService.CreateGameWithTags(game, selectedTags);
        this.DeveloperGames.Add(game);
    }

    public void UpdateGame(Game game)
    {
        this.developerService.UpdateGameAndRefreshList(game, this.DeveloperGames);
    }

    public void UpdateGameWithTags(Game game, IList<Tag> selectedTags)
    {
    }

    public void DeleteGame(int game_id)
    {
        this.developerService.DeleteGame(game_id, this.DeveloperGames);
    }

    public void RejectGame(int game_id)
    {
        this.developerService.RejectGameAndRemoveFromUnvalidated(game_id, this.UnvalidatedGames);
    }

    public async Task LoadUnvalidated()
    {
        this.UnvalidatedGames.Clear();
        var games = await this.developerService.GetUnvalidated();
        foreach (var game in games)
        {
            this.UnvalidatedGames.Add(game);
        }
    }

    public async Task<bool> IsGameIdInUse(int gameId)
    {
        return await this.developerService.IsGameIdInUse(gameId, this.DeveloperGames, this.UnvalidatedGames);
    }

    public async Task<int> GetGameOwnerCount(int game_id)
    {
        return await this.developerService.GetGameOwnerCount(game_id);
    }

    public void RejectGameWithMessage(int game_id, string rejectionMessage)
    {
    }

    public async Task HandleRejectGameAsync(int gameId, string rejectionReason)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(rejectionReason))
            {
               await this.developerService.RejectGameWithMessage(gameId, rejectionReason);
            }
            else
            {
                this.RejectGame(gameId);
            }

            await this.LoadUnvalidated();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task CreateGameAsync(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string recommendedRequirements, string discountText, IList<Tag> selectedTags)
    {
        // This can throw if any validation fails – and that’s okay
        Game game = await this.developerService.CreateValidatedGame(
            gameIdText, name, priceText, description, imageUrl, trailerUrl, gameplayUrl, minimumRequirement, recommendedRequirements, discountText, selectedTags);
        this.DeveloperGames.Add(game);
        this.OnPropertyChanged(nameof(this.DeveloperGames));
    }

    public async Task UpdateGameAsync(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string recommendedRequirements, string discountText, IList<Tag> selectedTags)
    {
        Game game = this.developerService.ValidateInputForAddingAGame(gameIdText, name, priceText, description, imageUrl, trailerUrl, gameplayUrl, minimumRequirement, recommendedRequirements, discountText, selectedTags);
        await this.developerService.UpdateGameWithTags(game, selectedTags);
    }

    public async Task<string> GetRejectionMessage(int gameId)
    {
        return await this.developerService.GetRejectionMessage(gameId);
    }

    public async Task<List<Tag>> GetGameTags(int gameId)
    {
        return await this.developerService.GetGameTags(gameId);
    }

    public async Task<IList<Tag>> GetMatchingTags(int gameId, IList<Tag> allTags)
    {
        return await this.developerService.GetMatchingTagsForGame(gameId, allTags);
    }

    public void PopulateEditForm(Game game)
    {
        this.EditGameId = game.GameId.ToString();
        this.EditGameName = game.GameTitle;
        this.EditGameDescription = game.GameDescription;
        this.EditGamePrice = game.Price.ToString();
        this.EditGameImageUrl = game.ImagePath;
        this.EditGameplayUrl = game.GameplayPath ?? string.Empty;
        this.EditTrailerUrl = game.TrailerPath ?? string.Empty;
        this.EditGameMinReq = game.MinimumRequirements;
        this.EditGameRecReq = game.RecommendedRequirements;
        this.EditGameDiscount = game.Discount.ToString();
    }

    public void ClearFieldsForAddingAGame()
    {
        this.AddGameId = string.Empty;
        this.AddGameName = string.Empty;
        this.AddGamePrice = string.Empty;
        this.AddGameDescription = string.Empty;
        this.AddGameImageUrl = string.Empty;
        this.AddGameplayUrl = string.Empty;
        this.AddTrailerUrl = string.Empty;
        this.AddGameMinimumRequirement = string.Empty;
        this.AddGameRecommendedRequirement = string.Empty;
        this.AddGameDiscount = string.Empty;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task LoadTags()
    {
        this.Tags.Clear();
        var allTags = await this.developerService.GetAllTags();
        foreach (var tag in allTags)
        {
            this.Tags.Add(tag);
        }

        this.OnPropertyChanged();
    }
}