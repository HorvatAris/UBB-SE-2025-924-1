using CtrlAltElite.Models;
using CtrlAltElite.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamStore.Models;
using SteamStore.Services.Interfaces;
using SteamStore.ViewModels;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CtrlAltElite.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InventoryPage : Page
    {
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
                    Title = "Confirm Sale",
                    Content = $"Are you sure you want to sell {selectedItem.ItemName}?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
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
