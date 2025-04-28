// <copyright file="MarketplacePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Views
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Steampunks.Domain.Entities;
    using Steampunks.Services.MarketplaceService;
    using Steampunks.ViewModels;
    using static Steampunks.ViewModels.MarketplaceViewModel;

    /// <summary>
    /// Represents the page that displays items available in the marketplace.
    /// </summary>
    public sealed partial class MarketplacePage : Page
    {
        private const string ErrorDialogTitle = "Error";
        private const string SelectUserErrorMessage = "Please select a user before buying items.";
        private const string OkButtonText = "OK";
        private const string SuccessDialogTitle = "Success";
        private const string ItemPurchasedMessage = "Item purchased successfully!";
        private const string UnexpectedErrorMessage = "An unexpected error occurred. Please try again.";
        private readonly MarketplaceViewModel marketplaceViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketplacePage"/> class.
        /// </summary>
        public MarketplacePage()
        {
            this.InitializeComponent();

            var marketplaceServiceInstance = new MarketplaceService(new Repository.Marketplace.MarketplaceRepository());
            this.marketplaceViewModel = new MarketplaceViewModel(marketplaceServiceInstance);
            this.DataContext = this.marketplaceViewModel;
            this.Loaded += this.OnMarketplacePageLoaded;
        }

        private async void OnMarketplacePageLoaded(object sender, RoutedEventArgs e)
        {
            await this.marketplaceViewModel.InitializeViewModelAsync();
        }

        /// <summary>
        /// Handles item click events in the GridView, opening a dialog and processing item purchase.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The item click event arguments.</param>
        private async void OnMarketplaceGridViewItemClicked(object sender, ItemClickEventArgs eventArgs)
        {
            if (eventArgs.ClickedItem is Item clickedMarketplaceItem)
            {
                var marketplaceViewModel = (MarketplaceViewModel)this.DataContext;
                marketplaceViewModel.SelectedItem = clickedMarketplaceItem;

                this.ItemDetailsDialog.XamlRoot = this.XamlRoot;
                var dialogResult = await this.ItemDetailsDialog.ShowAsync();

                if (dialogResult == ContentDialogResult.Secondary)
                {
                    var purchaseResult = await marketplaceViewModel.RequestPurchaseSelectedItemAsync();

                    switch (purchaseResult)
                    {
                        case PurchaseResult.MissingUser:
                            await this.ShowSimpleDialogAsync(ErrorDialogTitle, SelectUserErrorMessage);
                            break;

                        case PurchaseResult.Success:
                            await this.ShowSimpleDialogAsync(SuccessDialogTitle, ItemPurchasedMessage);
                            break;

                        case PurchaseResult.Error:
                            await this.ShowSimpleDialogAsync(ErrorDialogTitle, UnexpectedErrorMessage);
                            break;
                    }
                }
            }
        }

        private async Task ShowSimpleDialogAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = OkButtonText,
                XamlRoot = this.XamlRoot,
            };

            await dialog.ShowAsync();
        }
    }
}