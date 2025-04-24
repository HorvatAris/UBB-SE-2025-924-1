// <copyright file="GamePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Media.Imaging;
    using Microsoft.UI.Xaml.Navigation;
    using SteamStore.Constants;
    using SteamStore.Services.Interfaces;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    public sealed partial class GamePage : Page
    {
        private const int MaxNumberOfSimilarGamesToDisplay = 3;
        private const int InitialIndex = 0;
        private const string SpaceString = " ";
        private GamePageViewModel viewModel;

        public GamePage()
        {
            this.InitializeComponent();
        }

        public GamePage(IGameService gameService, ICartService cartService, IUserGameService userGameService, Game game)
        {
            this.InitializeComponent();
            this.viewModel = new GamePageViewModel(gameService, cartService, userGameService);
            this.DataContext = this.viewModel;
            if (game != null)
            {
                this.viewModel.LoadGame(game);
                this.LoadGameUi();
            }
        }

        public void LoadGameUi()
        {
            try
            {
                if (this.viewModel.Game == null)
                {
                    Debug.WriteLine("Error: Game not found");
                    return;
                }

                this.GameTitle.Text = this.viewModel.Game.GameTitle;
                this.GamePrice.Text = this.GamePrice.Text = this.viewModel.FormattedPrice;

                this.GameDescription.Text = this.viewModel.Game.GameDescription;

                this.GameDeveloper.Text = LabelStrings.DEVELOPERPREFIX + this.viewModel.Game.GameTitle;

                if (!string.IsNullOrEmpty(this.viewModel.Game.ImagePath))
                {
                    try
                    {
                        this.GameImage.Source = new BitmapImage(new Uri(this.viewModel.Game.ImagePath));
                    }
                    catch
                    {
                    }
                }

                this.MinimumRequirements.Text = this.viewModel.Game.MinimumRequirements;
                this.RecommendedRequirements.Text = this.viewModel.Game.RecommendedRequirements;
                this.AddMediaLinks(this.viewModel.Game);
                this.LoadSimilarGamesUi();
                this.GameRating.Value = Convert.ToDouble(this.viewModel.Game.Rating);
                this.OwnedStatus.Text = this.viewModel.IsOwned ? LabelStrings.OWNED : LabelStrings.NOTOWNED;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error loading game UI: {exception.Message}");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);

            try
            {
                if (this.viewModel == null)
                {
                    Debug.WriteLine("Error: Services not available");
                    return;
                }

                if (navigationEventArguments.Parameter is Game selectedGame)
                {
                    this.viewModel.LoadGame(selectedGame);
                    this.LoadGameUi();
                }
                else if (navigationEventArguments.Parameter is int gameId)
                {
                    this.viewModel.LoadGameById(gameId);
                    this.LoadGameUi();
                }
            }
            catch (Exception exception)
            {
                // Handle any errors
                Debug.WriteLine($"Error in OnNavigatedTo: {exception.Message}");
            }
        }

        private void AddMediaLinks(Game game)
        {
            this.MediaLinksPanel.Children.Clear();
            this.AddMediaLink(MediaLinkLabels.OFFICIALTRAILER, game.TrailerPath);
            this.AddMediaLink(MediaLinkLabels.GAMEPLAYVIDEO, game.GameplayPath);
        }

        private void AddMediaLink(string title, string url)
        {
            HyperlinkButton link = new HyperlinkButton
            {
                Content = title,
                NavigateUri = new Uri(url),
            };
            this.MediaLinksPanel.Children.Add(link);
        }

        private void LoadSimilarGamesUi()
        {
            try
            {
                this.SimilarGame1.Visibility = Visibility.Collapsed;
                this.SimilarGame2.Visibility = Visibility.Collapsed;
                this.SimilarGame3.Visibility = Visibility.Collapsed;
                var similarGameButtons = new[] { this.SimilarGame1, this.SimilarGame2, this.SimilarGame3 };
                var similarGameImages = new[] { this.SimilarGame1Image, this.SimilarGame2Image, this.SimilarGame3Image };
                var similarGameTitles = new[] { this.SimilarGame1Title, this.SimilarGame2Title, this.SimilarGame3Title };
                var similarGameRatings = new[] { this.SimilarGame1Rating, this.SimilarGame2Rating, this.SimilarGame3Rating };

                var similarGames = this.viewModel.SimilarGames.ToList();
                for (int similarGameIndex = InitialIndex; similarGameIndex < similarGames.Count; similarGameIndex++)
                {
                    this.DisplaySimilarGame(similarGameButtons[similarGameIndex], similarGameImages[similarGameIndex], similarGameTitles[similarGameIndex], similarGameRatings[similarGameIndex], similarGames[similarGameIndex]);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error loading similar games: {exception.Message}");
            }
        }

        private void DisplaySimilarGame(Button button, Image image, TextBlock title, TextBlock rating, Game game)
        {
            button.Visibility = Visibility.Visible;
            button.Tag = game;
            if (!string.IsNullOrEmpty(game.ImagePath))
            {
                try
                {
                    image.Source = new BitmapImage(new Uri(game.ImagePath));
                }
                catch
                {
                }
            }

            title.Text = game.GameTitle;
            rating.Text = string.Format(LabelStrings.RATINGFORMAT, game.Rating);
        }

        private void BuyButton_Click(object buyButtonSender, RoutedEventArgs buyClickEventArgument)
        {
            try
            {
                this.viewModel.AddToCart();
                this.ShowSuccessNotificationForBuy();
            }
            catch (Exception exception)
            {
                this.NotificationTip.Title = NotificationStrings.AddToCartErrorTitle;
                this.NotificationTip.Subtitle = string.Format(NotificationStrings.AddToCartErrorMessage, this.viewModel.Game.GameTitle) + SpaceString + exception.Message;
                this.NotificationTip.IsOpen = true;
            }
        }

        private void ShowSuccessNotificationForBuy()
        {
            this.NotificationTip.Title = NotificationStrings.AddToCartSuccessTitle;
            this.NotificationTip.Subtitle = string.Format(NotificationStrings.AddToCartSuccessMessage, this.viewModel.Game.GameTitle);
            this.NotificationTip.IsOpen = true;
        }

        private void WishlistButton_Click(object wishListButtonSender, RoutedEventArgs wishListClickEventArgument)
        {
            try
            {
                this.viewModel.AddToWishlist();
                this.NotificationTip.Title = NotificationStrings.AddToWishlistSuccessTitle;
                this.NotificationTip.Subtitle = string.Format(NotificationStrings.AddToWishlistSuccessMessage, this.viewModel.Game.GameTitle);
                this.NotificationTip.IsOpen = true;
            }
            catch (Exception exception)
            {
                this.NotificationTip.Title = NotificationStrings.AddToWishlistErrorTitle;
                string errorMessage = exception.Message;
                if (errorMessage.Contains(ErrorStrings.SQLNONQUERYFAILUREINDICATOR))
                {
                    errorMessage = string.Format(ErrorStrings.ADDTOWISHLISTALREADYEXISTSMESSAGE, this.viewModel.Game.GameTitle);
                }

                this.NotificationTip.Subtitle = errorMessage;
                this.NotificationTip.IsOpen = true;
            }
        }

        private void SimilarGame_Click(object similarGameButton, RoutedEventArgs similarGamesClickEventArgument)
        {
            if (similarGameButton is Button button && button.Tag is Game game)
            {
                Frame frame = this.Parent as Frame;
                this.viewModel.GetSimilarGames(game, frame);
            }
        }
    }
}