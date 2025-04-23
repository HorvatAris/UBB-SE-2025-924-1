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
        private const string CannotSendTradeTitle = "Cannot Send Trade";
        private const string CannotSendTradeMessage = "Please select a user to trade with, add items to trade, and provide a trade description.";

        private const string ConfirmTradeTitle = "Confirm Trade";
        private const string ConfirmTradeMessage = "Are you sure you want to send this trade offer?";

        private const string AcceptTradeTitle = "Accept Trade";
        private const string AcceptTradeMessage = "Are you sure you want to accept this trade?";

        private const string DeclineTradeTitle = "Decline Trade";
        private const string DeclineTradeMessage = "Are you sure you want to decline this trade?";

        private const string PrimaryButtonSendText = "Send";
        private const string PrimaryButtonAcceptText = "Accept";
        private const string PrimaryButtonDeclineText = "Decline";
        private const string CloseButtonCancelText = "Cancel";
        private const string CloseButtonOkText = "OK";

        private const int MinimumSelectedItemsCount = 1;

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
            if (this.SourceItemsListView.SelectedItems.Count >= MinimumSelectedItemsCount)
            {
                foreach (var selectedObject in this.SourceItemsListView.SelectedItems)
                {
                    if (selectedObject is Item selectedItem)
                    {
                        this.ViewModel.AddSourceItem(selectedItem);
                    }
                }

                this.SourceItemsListView.SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Adds selected items from destination list to the trade offer.
        /// </summary>
        private void OnAddDestinationItemButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (this.DestinationItemsListView.SelectedItems.Count >= MinimumSelectedItemsCount)
            {
                foreach (var selectedObject in this.DestinationItemsListView.SelectedItems)
                {
                    if (selectedObject is Item selectedItem)
                    {
                        this.ViewModel.AddDestinationItem(selectedItem);
                    }
                }

                this.DestinationItemsListView.SelectedItems.Clear();
            }
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
            if (!this.ViewModel.CanSendTradeOffer)
            {
                var cannotSendTradeDialog = new ContentDialog
                {
                    Title = CannotSendTradeTitle,
                    Content = CannotSendTradeMessage,
                    CloseButtonText = CloseButtonOkText,
                    XamlRoot = this.XamlRoot,
                };

                await cannotSendTradeDialog.ShowAsync();
                return;
            }

            var confirmSendTradeDialog = new ContentDialog
            {
                Title = ConfirmTradeTitle,
                Content = ConfirmTradeMessage,
                PrimaryButtonText = PrimaryButtonSendText,
                CloseButtonText = CloseButtonCancelText,
                XamlRoot = this.XamlRoot,
            };

            var userDialogResult = await confirmSendTradeDialog.ShowAsync();

            if (userDialogResult == ContentDialogResult.Primary)
            {
                await this.ViewModel.CreateTradeOffer();
            }
        }

        /// <summary>
        /// Accepts a trade offer after showing a confirmation dialog.
        /// </summary>
        private async void OnAcceptTradeButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button clickedButton && clickedButton.Tag is ItemTrade tradeToAccept)
            {
                var confirmAcceptTradeDialog = new ContentDialog
                {
                    Title = AcceptTradeTitle,
                    Content = AcceptTradeMessage,
                    PrimaryButtonText = PrimaryButtonAcceptText,
                    CloseButtonText = CloseButtonCancelText,
                    XamlRoot = this.XamlRoot,
                };

                var userDialogResult = await confirmAcceptTradeDialog.ShowAsync();

                if (userDialogResult == ContentDialogResult.Primary)
                {
                    this.ViewModel.AcceptTrade(tradeToAccept);
                }
            }
        }

        /// <summary>
        /// Declines a trade offer after showing a confirmation dialog.
        /// </summary>
        private async void OnDeclineTradeButtonClicked(object sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button clickedButton && clickedButton.Tag is ItemTrade tradeToDecline)
            {
                var confirmDeclineTradeDialog = new ContentDialog
                {
                    Title = DeclineTradeTitle,
                    Content = DeclineTradeMessage,
                    PrimaryButtonText = PrimaryButtonDeclineText,
                    CloseButtonText = CloseButtonCancelText,
                    XamlRoot = this.XamlRoot,
                };

                var userDialogResult = await confirmDeclineTradeDialog.ShowAsync();

                if (userDialogResult == ContentDialogResult.Primary)
                {
                    await this.ViewModel.DeclineTradeAsync(tradeToDecline);
                }
            }
        }
    }
}
