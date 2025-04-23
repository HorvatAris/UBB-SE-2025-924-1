// <copyright file="InventoryPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Views
{
    using System;
    using System.Linq;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;
    using Steampunks.Repository.Inventory;
    using Steampunks.Services.InventoryService.InventoryService;
    using Steampunks.ViewModels;

    /// <summary>
    /// Page responsible for displaying and managing a user's inventory.
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
        public InventoryPage()
        {
            this.InitializeComponent();

            // Ideally, use a dependency injection container to resolve these services.
            IDatabaseConnector databaseConnector = new DatabaseConnector();
            IInventoryRepository inventoryRepository = new InventoryRepository(databaseConnector);
            var inventoryService = new InventoryService(inventoryRepository);
            this.ViewModel = new InventoryViewModel(inventoryService);
            this.DataContext = this;

            this.Loaded += this.InventoryPage_Loaded;

            // Subscribe to user selection changes.
            this.UserComboBox.SelectionChanged += this.OnUserSelectionChanged;
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
            }
        }

        private async void OnUserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Delegate business logic to the view-model.
            if (this.ViewModel?.SelectedUser is User)
            {
                await this.ViewModel.LoadInventoryItemsAsync();
            }
        }

        /// <summary>
        /// Navigates to the trading page when the "Create Trade Offer" button is clicked.
        /// </summary>
        private void OnCreateTradeOfferButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TradingPage));
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
        /// Replaces a failed image load with a default image.
        /// </summary>
        private void OnItemImageLoadFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image itemImage && itemImage.Parent is Grid parentGrid)
            {
                var defaultImage = parentGrid.Children
                    .OfType<Image>()
                    .FirstOrDefault(image => image.Name == "DefaultImage");

                if (defaultImage != null)
                {
                    itemImage.Visibility = Visibility.Collapsed;
                    defaultImage.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Handles the sell button click, displaying a confirmation dialog and delegating the sale logic to the view-model.
        /// </summary>
        private async void OnSellItemButtonClicked(object sender, RoutedEventArgs e)
        {
            // Retrieve the selected item from the button's DataContext.
            if (sender is Button sellButton && sellButton.DataContext is Item selectedItem)
            {
                // Show a confirmation dialog.
                var confirmationDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = ConfirmSaleTitle,
                    Content = string.Format(ConfirmSaleMessageFormat, selectedItem.ItemName),
                    PrimaryButtonText = YesButtonText,
                    CloseButtonText = NoButtonText,
                    DefaultButton = ContentDialogButton.Close,
                };

                var userResponse = await confirmationDialog.ShowAsync();

                if (userResponse == ContentDialogResult.Primary && this.ViewModel != null)
                {
                    // Delegate the sale operation to the view-model.
                    bool success = await this.ViewModel.SellItemAsync(selectedItem);

                    // Refresh inventory after selling.
                    await this.ViewModel.LoadInventoryItemsAsync();

                    // Prepare the result dialog.
                    ContentDialog resultDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = success ? SuccessDialogTitle : ErrorDialogTitle,
                        Content = success
                            ? string.Format(SuccessDialogMessageFormat, selectedItem.ItemName)
                            : ErrorDialogMessage,
                        CloseButtonText = OkButtonText,
                    };

                    await resultDialog.ShowAsync();
                }
            }
        }
    }
}
