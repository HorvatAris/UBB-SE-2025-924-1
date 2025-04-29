// <copyright file="TradeView.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Views
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Steampunks.Domain.Entities;
    using Steampunks.ViewModels;

    /// <summary>
    /// Represents the page used for creating, sending, and managing trade offers between users.
    /// </summary>
    public sealed partial class TradeView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeView"/> class.
        /// </summary>
        public TradeView()
        {
            this.InitializeComponent();
            this.ViewModel = App.GetService<ITradeViewModel>();
            this.DataContext = this;
        }

        /// <summary>
        /// Gets the ViewModel used for handling trade logic and data binding.
        /// </summary>
        public ITradeViewModel ViewModel { get; }

        /// <summary>
        /// Handles page load event and initializes user and game lists.
        /// </summary>
        private async void OnTradeViewPageLoaded(object sender, RoutedEventArgs eventArguments)
        {
            await this.ViewModel.LoadUsersAsync();
            await this.ViewModel.LoadGamesAsync();
        }

        /// <summary>
        /// Adds selected items from source list to the trade offer.
        /// </summary>
        private void OnAddSourceItemButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            this.ViewModel.AddSelectedSourceItems(this.SourceItemsListView.SelectedItems);
            this.SourceItemsListView.SelectedItems.Clear();
        }

        /// <summary>
        /// Adds selected items from destination list to the trade offer.
        /// </summary>
        private void OnAddDestinationItemButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            this.ViewModel.AddSelectedDestinationItems(this.DestinationItemsListView.SelectedItems);
            this.DestinationItemsListView.SelectedItems.Clear();
        }

        /// <summary>
        /// Removes an item from the source items list.
        /// </summary>
        private void OnRemoveSourceItemButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button clickedButton && clickedButton.Tag is Item selectedItemToRemove)
            {
                this.ViewModel.RemoveSourceItem(selectedItemToRemove);
            }
        }

        /// <summary>
        /// Removes an item from the destination items list.
        /// </summary>
        private void OnRemoveDestinationItemButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button clickedButton && clickedButton.Tag is Item selectedItemToRemove)
            {
                this.ViewModel.RemoveDestinationItem(selectedItemToRemove);
            }
        }

        /// <summary>
        /// Sends a trade offer after showing a confirmation dialog.
        /// </summary>
        private async void OnSendTradeOfferButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            await this.ViewModel.TrySendTradeAsync(this.XamlRoot);
        }

        /// <summary>
        /// Accepts a trade offer after showing a confirmation dialog.
        /// </summary>
        private async void OnAcceptTradeButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button clickedButton && clickedButton.Tag is ItemTrade tradeToAccept)
            {
                await this.ViewModel.TryAcceptTradeAsync(tradeToAccept, this.XamlRoot);
            }
        }

        /// <summary>
        /// Declines a trade offer after showing a confirmation dialog.
        /// </summary>
        private async void OnDeclineTradeButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button clickedButton && clickedButton.Tag is ItemTrade tradeToDecline)
            {
                await this.ViewModel.TryDeclineTradeAsync(tradeToDecline, this.XamlRoot);
            }
        }
    }
}
