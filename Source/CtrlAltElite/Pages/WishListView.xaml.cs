// <copyright file="WishListView.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using SteamStore;
    using SteamStore.Models;
    using SteamStore.Pages;
    using SteamStore.Services.Interfaces;
    using SteamStore.ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WishListView : Page
    {
        private WishListViewModel viewModel;

        public WishListView(IUserGameService userGameService, IGameService gameService, ICartService cartService)
        {
            this.InitializeComponent();
            this.viewModel = new WishListViewModel(userGameService, gameService, cartService);
            this.DataContext = this.viewModel;
        }

        private void ViewDetails_Click(object viewDetailsButton, RoutedEventArgs viewDetailsEventArgument)
        {
            if (viewDetailsButton is Button button && button.DataContext is Game game)
            {
                if (this.Parent is Frame frame)
                {
                    this.viewModel.ViewGameDetails(frame, game);
                }
            }
        }

        private void BackButton_Click(object backButton, RoutedEventArgs backButtonClickEventArgument)
        {
            if (this.Parent is Frame frame)
            {
                this.viewModel.BackToHomePage(frame);
            }
        }

        private async void SearchBox_TextChanged(object searchTextBox, TextChangedEventArgs textChangedEventArguments)
        {
            if (this.viewModel != null)
            {
                await this.viewModel.HandleSearchWishListGames();
            }
        }
    }
}