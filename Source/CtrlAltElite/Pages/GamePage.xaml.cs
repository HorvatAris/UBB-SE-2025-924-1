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
        public GamePage(IGameService gameService, ICartService cartService, IUserGameService userGameService, Game game = null)
        {
            this.InitializeComponent();

            this.ViewModel = new GamePageViewModel(gameService, cartService, userGameService);

            this.DataContext = this.ViewModel;

            if (game != null)
            {
                this.ViewModel.LoadGame(game);
            }
        }

        private GamePageViewModel ViewModel { get; }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);

            if (navigationEventArguments.Parameter is Game selectedGame)
            {
                this.ViewModel.LoadGame(selectedGame);
            }
            else if (navigationEventArguments.Parameter is int gameId)
            {
                this.ViewModel.LoadGameById(gameId);
            }
        }

        private void BuyButton_Click(object buyButtonSender, RoutedEventArgs buyClickEventArgument)
        {
            try
            {
                this.ViewModel.AddToCart();
                this.ShowNotification(NotificationStrings.AddToCartSuccessTitle, string.Format(NotificationStrings.AddToCartSuccessMessage, this.ViewModel.Game.GameTitle));
            }
            catch (Exception exception)
            {
                this.ShowNotification(NotificationStrings.AddToCartErrorTitle, $"{NotificationStrings.AddToCartErrorMessage} {exception.Message}");
            }
        }

        private void WishlistButton_Click(object wishListButtonSender, RoutedEventArgs wishListClickEventArgument)
        {
            try
            {
                this.ViewModel.AddToWishlist();
                this.ShowNotification(NotificationStrings.AddToWishlistSuccessTitle, string.Format(NotificationStrings.AddToWishlistSuccessMessage, this.ViewModel.Game.GameTitle));
            }
            catch (Exception exception)
            {
                var message = exception.Message.Contains(ErrorStrings.SQLNONQUERYFAILUREINDICATOR)
                    ? string.Format(ErrorStrings.ADDTOWISHLISTALREADYEXISTSMESSAGE, this.ViewModel.Game.GameTitle)
                    : exception.Message;

                this.ShowNotification(NotificationStrings.AddToWishlistErrorTitle, message);
            }
        }

        private void SimilarGame_Click(object similarGameButton, RoutedEventArgs similarGamesClickEventArgument)
        {
            if (similarGameButton is Button button && button.Tag is Game game)
            {
                Frame frame = this.Parent as Frame;
                this.ViewModel.GetSimilarGames(game, frame);
            }
        }

        private void ShowNotification(string title, string subtitle)
        {
            this.NotificationTip.Title = title;
            this.NotificationTip.Subtitle = subtitle;
            this.NotificationTip.IsOpen = true;
        }
    }
}