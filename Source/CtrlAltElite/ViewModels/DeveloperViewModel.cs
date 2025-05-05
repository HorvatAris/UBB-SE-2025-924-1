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

    public async Task InitAsync()
    {
        await this.LoadGames();
        this.LoadTags();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Game> DeveloperGames { get; set; }

    public ObservableCollection<Game> UnvalidatedGames { get; set; }

    public ObservableCollection<Tag> Tags { get; set; }

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

    public async Task ValidateGame(int game_id)
    {
        await this.developerService.ValidateGame(game_id);
    }

    public bool CheckIfUserIsADeveloper()
    {
        return this.developerService.GetCurrentUser().UserRole == User.Role.Developer;
    }

    public async Task CreateGame(Game game, IList<Tag> selectedTags)
    {
        await this.developerService.CreateGameWithTags(game, selectedTags);
        this.DeveloperGames.Add(game);
    }

    public async Task UpdateGame(Game game)
    {
        await this.developerService.UpdateGameAndRefreshList(game, this.DeveloperGames);
    }

    public void UpdateGameWithTags(Game game, IList<Tag> selectedTags)
    {
    }

    public async Task DeleteGame(int game_id)
    {
        await this.developerService.DeleteGame(game_id, this.DeveloperGames);
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
        catch (Exception exception)
        {
            await this.ShowErrorMessage("Error", $"Failed to reject game: {exception.Message}");
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
        Game game = await this.developerService.CreateValidatedGame(
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

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void LoadTags()
    {
        this.Tags.Clear();
        var allTags = await this.developerService.GetAllTags();
        foreach (var tag in allTags)
        {
            this.Tags.Add(tag);
        }

        this.OnPropertyChanged();
    }

    private async Task ShowErrorMessage(string title, string message)
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