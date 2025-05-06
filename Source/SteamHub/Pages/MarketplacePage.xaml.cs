// <copyright file="MarketplacePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using SteamHub.Models;
    using SteamHub.Services.Interfaces;
    using SteamHub.ViewModels;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using SteamHub.Models;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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

        public MarketplacePage(IMarketplaceService marketplaceService)
        {
            this.InitializeComponent();

            // var marketplaceServiceInstance = new MarketplaceService(new Repository.Marketplace.MarketplaceRepository());
            this.marketplaceViewModel = new MarketplaceViewModel(marketplaceService);
            this.DataContext = this.marketplaceViewModel;
            this.Loaded += this.OnMarketplacePageLoaded;
        }

        private async void OnMarketplacePageLoaded(object sender, RoutedEventArgs e)
        {
            await this.marketplaceViewModel.InitializeViewModelAsync();
        }

        private void OnMarketplaceGridViewItemClicked(object sender, ItemClickEventArgs eventArgs)
        {
            if (eventArgs.ClickedItem is Item clickedMarketplaceItem)
            {
                var marketplaceViewModel = (MarketplaceViewModel)this.DataContext;
                marketplaceViewModel.SelectedItem = clickedMarketplaceItem;

                this.ItemDetailsDialog.XamlRoot = this.XamlRoot;

                this.ItemDetailsDialog.ShowAsync().Completed = async (asyncOperation, asyncStatus) =>
                {
                    var dialogResult = (ContentDialogResult)asyncOperation.GetResults();
                    if (dialogResult == ContentDialogResult.Secondary)
                    {
                        try
                        {
                            if (marketplaceViewModel.CurrentUser == null)
                            {
                                var missingUserDialog = new ContentDialog
                                {
                                    Title = ErrorDialogTitle,
                                    Content = SelectUserErrorMessage,
                                    CloseButtonText = OkButtonText,
                                    XamlRoot = this.XamlRoot,
                                };
                                await missingUserDialog.ShowAsync();
                                return;
                            }

                            bool purchaseSuccess = await marketplaceViewModel.BuyItemAsync();
                            if (purchaseSuccess)
                            {
                                var purchaseSuccessDialog = new ContentDialog
                                {
                                    Title = SuccessDialogTitle,
                                    Content = ItemPurchasedMessage,
                                    CloseButtonText = OkButtonText,
                                    XamlRoot = this.XamlRoot,
                                };
                                await purchaseSuccessDialog.ShowAsync();
                            }
                        }
                        catch (InvalidOperationException operationException)
                        {
                            var operationErrorDialog = new ContentDialog
                            {
                                Title = ErrorDialogTitle,
                                Content = operationException.Message,
                                CloseButtonText = OkButtonText,
                                XamlRoot = this.XamlRoot,
                            };
                            await operationErrorDialog.ShowAsync();
                        }
                        catch (Exception generalException)
                        {
                            var generalErrorDialog = new ContentDialog
                            {
                                Title = ErrorDialogTitle,
                                Content = UnexpectedErrorMessage,
                                CloseButtonText = OkButtonText,
                                XamlRoot = this.XamlRoot,
                            };
                            await generalErrorDialog.ShowAsync();

                            System.Diagnostics.Debug.WriteLine($"Error in OnMarketplaceGridViewItemClicked: {generalException.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {generalException.StackTrace}");
                        }
                    }
                };
            }
        }
    }
}