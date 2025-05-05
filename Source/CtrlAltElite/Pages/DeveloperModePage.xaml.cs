// <copyright file="DeveloperModePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using CtrlAltElite.Constants;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamStore.Constants;
    using SteamStore.Models;
    using SteamStore.Services.Interfaces;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeveloperModePage : Page
    {
        private DeveloperViewModel viewModel;

        public DeveloperModePage(IDeveloperService developerService)
        {
            this.InitializeComponent();
            this.viewModel = new DeveloperViewModel(developerService);
            this.DataContext = this.viewModel;
        }

        private async void DeveloperModePage_Loaded(object developerModePage, RoutedEventArgs developerPageLoadedArguments)
        {
            await this.viewModel.InitAsync();

            // Check if user is a developer
            if (!this.viewModel.CheckIfUserIsADeveloper())
            {
                // Show error message dialog
                this.ShowNotDeveloperMessage();
            }
        }

        private async void ReviewGamesButton_Click(object reviewGamesButton, RoutedEventArgs reviewGamesEventArgument)
        {
            await this.viewModel.LoadUnvalidatedAsync();
            this.DeveloperGamesList.Visibility = Visibility.Collapsed;
            this.ReviewGamesList.Visibility = Visibility.Visible;
            this.viewModel.PageTitle = DeveloperPageTitles.REVIEWGAMES;
        }

        private async void MyGamesButton_Click(object myGamesButton, RoutedEventArgs myGamesClickEventArgument)
        {
            await this.viewModel.LoadGamesAsync();
            this.DeveloperGamesList.Visibility = Visibility.Visible;
            this.ReviewGamesList.Visibility = Visibility.Collapsed;
            this.viewModel.PageTitle = DeveloperPageTitles.MYGAMES;
        }

        private async void AcceptButton_Click(object acceptButton, RoutedEventArgs acceptClickEventArgument)
        {
            if (acceptButton is Button button && button.CommandParameter is int gameId)
            {
                await this.viewModel.ValidateGameAsync(gameId);
                await this.viewModel.LoadUnvalidatedAsync();
            }
        }

        private async void RejectButton_Click(object rejectButton, RoutedEventArgs rejectClickEventArgument)
        {
            if (rejectButton is Button button && button.CommandParameter is int gameId)
            {
                this.RejectGameDialog.XamlRoot = this.Content.XamlRoot;
                this.viewModel.RejectReason = string.Empty;

                var result = await this.RejectGameDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        await this.viewModel.HandleRejectGameAsync(gameId, this.viewModel.RejectReason);
                    }
                    catch (Exception exception)
                    {
                        await this.ShowErrorMessage("Error", $"Failed to reject game: {exception.Message}");
                    }
                }
            }
        }

        private async void AddGameButton_Click(object addGameButton, RoutedEventArgs addGameEventArgument)
        {
            var result = await this.AddGameDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await this.viewModel.CreateGameAsync(
                        this.viewModel.AddGameId,
                        this.viewModel.AddGameName,
                        this.viewModel.AddGamePrice,
                        this.viewModel.AddGameDescription,
                        this.viewModel.AddGameImageUrl,
                        this.viewModel.AddGameplayUrl,
                        this.viewModel.AddTrailerUrl,
                        this.viewModel.AddGameMinimumRequirement,
                        this.viewModel.AddGameRecommendedRequirement,
                        this.viewModel.AddGameDiscount,
                        this.AddGameTagList.SelectedItems.Cast<Tag>().ToList());

                    this.viewModel.ClearFieldsForAddingAGame();
                    this.AddGameTagList.SelectedItems.Clear();
                }
                catch (Exception exception)
                {
                    await this.ShowErrorMessage("Error", exception.Message);
                }
            }
        }

        private async Task ShowErrorMessage(string title, string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = ConfirmationDialogStrings.OKBUTTONTEXT,
                XamlRoot = this.Content.XamlRoot,
            };
            await errorDialog.ShowAsync();
        }

        private async void ShowNotDeveloperMessage()
        {
            if (this.Content == null || this.Content.XamlRoot == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot show developer access dialog: XamlRoot is null");
                return;
            }

            ContentDialog notDeveloperDialog = new ContentDialog
            {
                Title = NotDeveloperDialogStrings.ACCESSDENIEDTITLE,
                Content = NotDeveloperDialogStrings.ACCESSDENIEDMESSAGE,
                CloseButtonText = NotDeveloperDialogStrings.CLOSEBUTTONTEXT,
                XamlRoot = this.Content.XamlRoot,
            };

            try
            {
                await notDeveloperDialog.ShowAsync();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing developer access dialog: {exception.Message}");
            }
        }

        private async void RejectionButton_Click(object rejectButton, RoutedEventArgs rejectionClickEventArgument)
        {
            if (rejectButton is Button button && button.CommandParameter is int gameId)
            {
                try
                {
                    string rejectionMessage = await this.viewModel.GetRejectionMessageAsync(gameId);
                    if (!string.IsNullOrWhiteSpace(rejectionMessage))
                    {
                        this.viewModel.RejectionMessage = rejectionMessage;
                        this.RejectionMessageDialog.XamlRoot = this.Content.XamlRoot;
                        await this.RejectionMessageDialog.ShowAsync();
                    }
                    else
                    {
                        await this.ShowErrorMessage(DeveloperDialogStrings.INFOTITLE, DeveloperDialogStrings.NOREJECTIONMESSAGE);
                    }
                }
                catch (Exception exception)
                {
                    await this.ShowErrorMessage("Error", $"Failed to retrieve rejection message: {exception.Message}");
                }
            }
        }

        private async void RemoveButton_Click(object removeButton, RoutedEventArgs removeClickEventArgument)
        {
            if (removeButton is Button button && button.CommandParameter is int gameId)
            {
                try
                {
                    // Check if the game is owned by any users
                    int ownerCount = await this.viewModel.GetGameOwnerCountAsync(gameId);
                    ContentDialogResult result;
                    if (ownerCount > DeveloperModePageConstants.NoOwnersCount)
                    {
                        // Game is owned by users, show warning dialog
                        this.DeleteWarningDialog.XamlRoot = this.Content.XamlRoot;

                        // OwnerCountText.Text = $"This game is currently owned by {ownerCount} user{(ownerCount == 1 ? "" : "s")}.";
                        this.viewModel.OwnerCountText = string.Format(DeveloperDialogStrings.DELETECONFIRMATIONOWNED, ownerCount, ownerCount == DeveloperModePageConstants.OneOwnerCount ? string.Empty : DeveloperModePageConstants.StringPlural);
                        result = await this.DeleteWarningDialog.ShowAsync();
                    }
                    else
                    {
                        // Game is not owned by any users, show standard confirmation dialog
                        this.DeleteConfirmationDialog.XamlRoot = this.Content.XamlRoot;
                        result = await this.DeleteConfirmationDialog.ShowAsync();
                    }

                    if (result == ContentDialogResult.Primary)
                    {
                        await this.viewModel.DeleteGameAsync(gameId);

                        // Refresh the games list
                        await this.viewModel.LoadGamesAsync();
                    }
                }
                catch (Exception exception)
                {
                    await this.ShowErrorMessage(DeveloperDialogStrings.ERRORTITLE, string.Format(DeveloperDialogStrings.FAILEDTODELETE, exception.Message));
                }
            }
        }

        private async void EditButton_Click(object editButton, RoutedEventArgs editClickEventArgument)
        {
            if (editButton is Button button && button.CommandParameter is int gameId)
            {
                Game gameToEdit = this.viewModel.GetGameByIdInDeveloperGameList(gameId);
                if (gameToEdit != null)
                {
                    // System.Diagnostics.Debug.WriteLine("Im in edit");
                    try
                    {
                        this.viewModel.PopulateEditForm(gameToEdit);
                        await this.LoadGameTags(gameToEdit);
                        this.EditGameDialog.XamlRoot = this.Content.XamlRoot;
                        var result = await this.EditGameDialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            await this.SaveUpdatedGameAsync();

                            // Reload games after the update
                            await this.viewModel.LoadGamesAsync();
                        }
                    }
                    catch (Exception exception)
                    {
                        await this.ShowErrorMessage("Error", exception.Message);
                    }
                }
            }
        }

        private async Task SaveUpdatedGameAsync()
        {
            try
            {
                await this.viewModel.UpdateGameAsync(
                    this.viewModel.EditGameId,
                    this.viewModel.EditGameName,
                    this.viewModel.EditGamePrice,
                    this.viewModel.EditGameDescription,
                    this.viewModel.EditGameImageUrl,
                    this.viewModel.EditGameplayUrl,
                    this.viewModel.EditTrailerUrl,
                    this.viewModel.EditGameMinReq,
                    this.viewModel.EditGameRecReq,
                    this.viewModel.EditGameDiscount,
                    this.EditGameTagList.SelectedItems.Cast<Tag>().ToList());
            }
            catch (Exception exception)
            {
                await this.ShowErrorMessage("Error", exception.Message);
            }
        }

        private async Task PopulateEditForm(Game game)
        {
            this.EditGameId.Text = game.GameId.ToString();
            this.EditGameId.IsEnabled = false;
            this.EditGameName.Text = game.GameTitle;
            this.EditGameDescription.Text = game.GameDescription;
            this.EditGamePrice.Text = game.Price.ToString();
            this.EditGameImageUrl.Text = game.ImagePath;
            this.EditGameplayUrl.Text = game.GameplayPath ?? string.Empty;
            this.EditTrailerUrl.Text = game.TrailerPath ?? string.Empty;
            this.EditGameMinReq.Text = game.MinimumRequirements;
            this.EditGameRecReq.Text = game.RecommendedRequirements;
            this.EditGameDiscount.Text = game.Discount.ToString();
            await this.LoadGameTags(game);
        }

        private async Task LoadGameTags(Game game)
        {
            this.EditGameTagList.SelectedItems.Clear();

            try
            {
                var availableTags = this.EditGameTagList.Items.Cast<object>().OfType<Tag>().ToList(); // Safe cast
                var matchingTags = await this.viewModel.GetMatchingTagsAsync(game.GameId, availableTags);
                foreach (Tag tag in matchingTags)
                {
                    this.EditGameTagList.SelectedItems.Add(tag);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error loading game tags: {exception.Message}");
            }
        }
    }
}
