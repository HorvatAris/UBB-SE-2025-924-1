// <copyright file="InventoryPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using System.Linq;
    using SteamHub.ApiContract.Models;
    using SteamHub.ViewModels;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ApiContract.Models.Item;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InventoryPage : Page
    {
        private const string ConfirmSaleTitle = "Confirm Sale";
        private const string ConfirmSaleMessageFormat = "Are you sure you want to sell {0}?";
        private const string SuccessDialogTitle = "Success";
        private const string SuccessDialogMessageFormat = "{0} has been successfully listed for sale!";
        private const string ErrorDialogTitle = "Error";
        private const string ErrorDialogMessage = "Failed to sell the item. Please try again.";
        private const string OkButtonText = "OK";
        private const string YesButtonText = "Yes";
        private const string NoButtonText = "No";

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryPage"/> class.
        /// </summary>
        public InventoryPage(IInventoryService inventoryService)
        {
            this.InitializeComponent();
            this.ViewModel = new InventoryViewModel(inventoryService);
            this.DataContext = this;
            this.Loaded += this.InventoryPage_Loaded;
        }

        /// <summary>
        /// Gets the view model for this page.
        /// </summary>
        public InventoryViewModel? ViewModel { get; private set; }

        private async void InventoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                await this.ViewModel.InitializeAsync();
                await this.ViewModel.LoadInventoryItemsAsync();
            }
        }

        /// <summary>
        /// Updates the view model with the selected inventory item.
        /// </summary>
        private void OnInventoryItemClicked(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Item selectedItem && this.ViewModel != null)
            {
                this.ViewModel.SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// Handles the sell button click, displaying a confirmation dialog and delegating the sale logic to the view-model.
        /// </summary>
        private async void OnSellItemButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button sellButton && sellButton.DataContext is Item selectedItem)
            {
                var confirmationDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = ConfirmSaleTitle,
                    Content = string.Format(ConfirmSaleMessageFormat, selectedItem.ItemName),
                    PrimaryButtonText = YesButtonText,
                    CloseButtonText = NoButtonText,
                    DefaultButton = ContentDialogButton.Close,
                };

                var result = await confirmationDialog.ShowAsync();
                if (result == ContentDialogResult.Primary && this.ViewModel != null)
                {
                    var (isSuccess, message) = await this.ViewModel.TrySellItemAsync(selectedItem);
                    await this.ViewModel.LoadInventoryItemsAsync();

                    var resultDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = isSuccess ? "Success" : "Error",
                        Content = message,
                        CloseButtonText = "OK",
                    };

                    await resultDialog.ShowAsync();
                }
            }
        }
    }
}
