using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.Configuration;
using SteamStore.Data;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services;
using SteamStore.ViewModels;
using SteamStore.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamStore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InventoryPage : Microsoft.UI.Xaml.Controls.Page
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
        public InventoryPage(User user)
        {
            this.InitializeComponent();
            IDataLink databaseConnector = new DataLink(new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build());

            IInventoryRepository inventoryRepository = new InventoryRepository(databaseConnector, user);
            var inventoryService = new InventoryService(inventoryRepository);
            this.ViewModel = new InventoryViewModel(inventoryService);
            this.DataContext = this;

            this.Loaded += this.InventoryPage_Loaded;
        }
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

        private void OnCreateTradeOfferButtonClicked(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(TradingPage));
        }

        private void OnInventoryItemClicked(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Item selectedItem && this.ViewModel != null)
            {
                this.ViewModel.SelectedItem = selectedItem;
            }
        }

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
