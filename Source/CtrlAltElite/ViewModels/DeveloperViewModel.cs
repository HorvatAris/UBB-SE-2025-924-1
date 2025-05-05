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

    public DeveloperViewModel(IDeveloperService developerService)
    {
        this.developerService = developerService;
        this.DeveloperGames = new ObservableCollection<Game>();
        this.UnvalidatedGames = new ObservableCollection<Game>();
        this.Tags = new ObservableCollection<Tag>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Game> DeveloperGames { get; set; }

    public ObservableCollection<Game> UnvalidatedGames { get; set; }

    public ObservableCollection<Tag> Tags { get; set; }

    public async Task InitAsync()
    {
        await this.LoadGamesAsync();
        await this.LoadTagsAsync();
    }

    public Game GetGameByIdInDeveloperGameList(int gameId)
    {
        return this.developerService.FindGameInObservableCollectionById(gameId, this.DeveloperGames);
    }

    public async Task LoadGamesAsync()
    {
        this.DeveloperGames.Clear();
        var games = await this.developerService.GetDeveloperGamesAsync();
        foreach (var game in games)
        {
            this.DeveloperGames.Add(game);
        }

        this.OnPropertyChanged();
    }

    public async Task ValidateGameAsync(int game_id)
    {
        await this.developerService.ValidateGameAsync(game_id);
    }

    public bool CheckIfUserIsADeveloper()
    {
        return this.developerService.GetCurrentUser().UserRole == User.Role.Developer;
    }

    public async Task CreateGameAsync(Game game, IList<Tag> selectedTags)
    {
        await this.developerService.CreateGameWithTagsAsync(game, selectedTags);
        this.DeveloperGames.Add(game);
    }

    public async Task UpdateGameAsync(Game game)
    {
        await this.developerService.UpdateGameAndRefreshListAsync(game, this.DeveloperGames);
    }

    public void UpdateGameWithTags(Game game, IList<Tag> selectedTags)
    {
    }

    public async Task DeleteGameAsync(int game_id)
    {
        await this.developerService.DeleteGameAsync(game_id, this.DeveloperGames);
    }

    public async Task RejectGameAsync(int game_id)
    {
        await this.developerService.RejectGameAndRemoveFromUnvalidatedAsync(game_id, this.UnvalidatedGames);
    }

    public async Task LoadUnvalidatedAsync()
    {
        this.UnvalidatedGames.Clear();
        var games = await this.developerService.GetUnvalidatedAsync();
        foreach (var game in games)
        {
            this.UnvalidatedGames.Add(game);
        }
    }

    public async Task<bool> IsGameIdInUseAsync(int gameId)
    {
        return await this.developerService.IsGameIdInUseAsync(gameId, this.DeveloperGames, this.UnvalidatedGames);
    }

    public async Task<int> GetGameOwnerCountAsync(int game_id)
    {
        return await this.developerService.GetGameOwnerCountAsync(game_id);
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
                await this.developerService.RejectGameWithMessageAsync(gameId, rejectionReason);
            }
            else
            {
                await this.RejectGameAsync(gameId);
            }

            await this.LoadUnvalidatedAsync();
        }
        catch (Exception exception)
        {
            await this.ShowErrorMessageAsync("Error", $"Failed to reject game: {exception.Message}");
        }
    }

    public async Task CreateGameAsync(
        string gameIdText,
        string name,
        string priceText,
        string description,
        string imageUrl,
        string trailerUrl,
        string gameplayUrl,
        string minimumRequirement,
        string recommendedRequirements,
        string discountText,
        IList<Tag> selectedTags)
    {
        // This can throw if any validation fails – and that’s okay
        Game game = await this.developerService.CreateValidatedGameAsync(
            gameIdText,
            name,
            priceText,
            description,
            imageUrl,
            trailerUrl,
            gameplayUrl,
            minimumRequirement,
            recommendedRequirements,
            discountText,
            selectedTags);
        this.DeveloperGames.Add(game);
        this.OnPropertyChanged(nameof(this.DeveloperGames));
    }

    public async Task UpdateGameAsync(
        string gameIdText,
        string name,
        string priceText,
        string description,
        string imageUrl,
        string trailerUrl,
        string gameplayUrl,
        string minimumRequirement,
        string recommendedRequirements,
        string discountText,
        IList<Tag> selectedTags)
    {
        Game game = this.developerService.ValidateInputForAddingAGame(
            gameIdText,
            name,
            priceText,
            description,
            imageUrl,
            trailerUrl,
            gameplayUrl,
            minimumRequirement,
            recommendedRequirements,
            discountText,
            selectedTags);
        await this.developerService.UpdateGameWithTagsAsync(game, selectedTags);
    }

    public async Task<string> GetRejectionMessageAsync(int gameId)
    {
        return await this.developerService.GetRejectionMessageAsync(gameId);
    }

    public async Task<List<Tag>> GetGameTagsAsync(int gameId)
    {
        return await this.developerService.GetGameTagsAsync(gameId);
    }

    public async Task<IList<Tag>> GetMatchingTagsAsync(int gameId, IList<Tag> allTags)
    {
        return await this.developerService.GetMatchingTagsForGameAsync(gameId, allTags);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task LoadTagsAsync()
    {
        this.Tags.Clear();
        var allTags = await this.developerService.GetAllTagsAsync();
        foreach (var tag in allTags)
        {
            this.Tags.Add(tag);
        }

        this.OnPropertyChanged();
    }

    private async Task ShowErrorMessageAsync(string title, string message)
    {
        ContentDialog errorDialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = ConfirmationDialogStrings.OKBUTTONTEXT,
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };
        await errorDialog.ShowAsync();
    }
}