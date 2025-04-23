// <copyright file="PointShopViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Controls;
    using SteamStore.Constants;
    using SteamStore.Data;
    using SteamStore.Models;
    using SteamStore.Services;
    using SteamStore.Services.Interfaces;

    public class PointShopViewModel : INotifyPropertyChanged
    {
        private const int NoPointsBalance = 0;
        private readonly IPointShopService pointShopService;
        private User user;

        // Collections
        private ObservableCollection<PointShopItem> shopItems;
        private ObservableCollection<PointShopItem> userItems;
        private ObservableCollection<PointShopTransaction> transactionHistory;

        // Filter properties
        private string filterType = PointShopConstants.FILTERTYPEALL;
        private string searchText = PointShopConstants.INITIALSEARCHSTRING;
        private double minimumPrice = PointShopConstants.MINIMUMPRICE;
        private double maximumPrice = PointShopConstants.MAXIMUMPRICE;

        // Selected item
        private PointShopItem selectedItem;

        private CancellationTokenSource searchCancellationTokenSource;
        private int nextTransactionId = PointShopConstants.TRANSACTIONIDENTIFIER;
        private bool isDetailPanelVisible;

         // public PointShopViewModel(User currentUser, IDataLink dataLink)
         // {
        //    // Store the current user reference
        //    this.user = currentUser;

        // // Initialize service and collections
        //    this.pointShopService = new PointShopService(currentUser, dataLink);
        //    this.ShopItems = new ObservableCollection<PointShopItem>();
        //    this.UserItems = new ObservableCollection<PointShopItem>();
        //    this.TransactionHistory = new ObservableCollection<PointShopTransaction>();

        // // Load initial data
        //    this.LoadItems();
        //    this.LoadUserItems();
        // }
        public PointShopViewModel(IPointShopService pointShopService)
        {
            // Initialize with existing service
            this.pointShopService = pointShopService;

            // Get the user reference from the service's internal repository
            this.user = this.pointShopService.GetCurrentUser();

            // Initialize collections
            this.ShopItems = new ObservableCollection<PointShopItem>();
            this.UserItems = new ObservableCollection<PointShopItem>();
            this.TransactionHistory = new ObservableCollection<PointShopTransaction>();

            // Load initial data
            this.LoadItems();
            this.LoadUserItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedItemImageUri { get; private set; }

        public bool IsDetailPanelVisible
        {
            get => this.isDetailPanelVisible;
            set
            {
                this.isDetailPanelVisible = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<PointShopItem> ShopItems
        {
            get => this.shopItems;
            set
            {
                this.shopItems = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<PointShopItem> UserItems
        {
            get => this.userItems;
            set
            {
                this.userItems = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<PointShopTransaction> TransactionHistory
        {
            get => this.transactionHistory;
            set
            {
                this.transactionHistory = value;
                this.OnPropertyChanged();
            }
        }

        public string FilterType
        {
            get => this.filterType;
            set
            {
                if (this.filterType != value)
                {
                    this.filterType = value;
                    this.OnPropertyChanged();
                    this.ApplyFilters();
                }
            }
        }

        public string SearchText
        {
            get => this.searchText;
            set
            {
                if (this.searchText != value)
                {
                    this.searchText = value;
                    this.OnPropertyChanged();

                    // Cancel any existing search operation
                    this.searchCancellationTokenSource?.Cancel();
                    this.searchCancellationTokenSource = new CancellationTokenSource();

                    // Apply search with a small delay to avoid too many updates
                    this.DelayedSearch(this.searchCancellationTokenSource.Token);
                }
            }
        }

        public double MinimumPrice
        {
            get => this.minimumPrice;
            set
            {
                if (this.minimumPrice != value)
                {
                    this.minimumPrice = value;
                    this.OnPropertyChanged();
                    this.ApplyFilters();
                }
            }
        }

        public double MaximumPrice
        {
            get => this.maximumPrice; set
            {
                if (this.maximumPrice != value)
                {
                    this.maximumPrice = value;
                    this.OnPropertyChanged();
                    this.ApplyFilters();
                }
            }
        }

        public PointShopItem SelectedItem
        {
            get => this.selectedItem;
            set
            {
                this.selectedItem = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CanPurchase));
            }
        }

        public float UserPointBalance => this.user?.PointsBalance ?? NoPointsBalance;

        public bool CanPurchase
        {
            get
            {
                return this.pointShopService.CanUserPurchaseItem(this.user, this.selectedItem, this.UserItems);
            }
        }

        public void LoadItems()
        {
            try
            {
                var availableItems = this.pointShopService.GetAvailableItems(this.user);
                this.ShopItems.Clear();
                foreach (var item in availableItems)
                {
                    this.ShopItems.Add(item);
                }

                this.ApplyFilters();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading items: {exception.Message}");
            }
        }

        public void LoadUserItems()
        {
            try
            {
                var items = this.pointShopService.GetUserItems();
                this.UserItems.Clear();
                foreach (var item in items)
                {
                    this.UserItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error loading user items: {exception.Message}");
            }
        }

        public async Task<bool> PurchaseSelectedItem()
        {
            if (this.SelectedItem == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot purchase: SelectedItem is null");
                return false;
            }

            try
            {
                // Store a local copy of the item to prevent issues after state changes
                var itemToPurchase = this.SelectedItem;

                this.pointShopService.PurchaseItem(itemToPurchase);

                // Add transaction to history
                var transaction = new PointShopTransaction(
                    this.nextTransactionId++,
                    itemToPurchase.Name,
                    itemToPurchase.PointPrice,
                    itemToPurchase.ItemType);
                this.TransactionHistory.Add(transaction);

                // Point balance is updated in the repository
                this.OnPropertyChanged(nameof(this.UserPointBalance));
                this.OnPropertyChanged(nameof(this.CanPurchase));

                // Reload user items to show the new purchase
                this.LoadUserItems();

                // Reload shop items to remove the purchased item
                this.LoadItems();

                return true;
            }
            catch (Exception exception)
            {
                // Rethrow for handling in the UI
                throw new Exception($"Failed to purchase item: {exception.Message}", exception);
            }
        }

        public async Task<bool> ActivateItem(PointShopItem item)
        {
            if (item == null)
            {
                return false;
            }

            try
            {
                this.pointShopService.ActivateItem(item);

                // Refresh user items to reflect the activation change
                this.LoadUserItems();

                return true;
            }
            catch (Exception exception)
            {
                // Rethrow for handling in the UI
                throw new Exception($"Failed to activate item: {exception.Message}", exception);
            }
        }

        public async Task<bool> DeactivateItem(PointShopItem item)
        {
            if (item == null)
            {
                return false;
            }

            try
            {
                this.pointShopService.DeactivateItem(item);

                // Refresh user items to reflect the deactivation change
                this.LoadUserItems();

                return true;
            }
            catch (Exception exception)
            {
                // Rethrow for handling in the UI
                throw new Exception($"Failed to deactivate item: {exception.Message}", exception);
            }
        }

        public bool HandleItemSelection()
        {
            if (this.SelectedItem != null)
            {
                this.SelectedItemImageUri = this.SelectedItem.ImagePath;
                this.IsDetailPanelVisible = true;
            }
            else
            {
                this.IsDetailPanelVisible = false;
            }

            this.OnPropertyChanged(nameof(this.SelectedItemImageUri));
            this.OnPropertyChanged(nameof(this.IsDetailPanelVisible));
            return this.IsDetailPanelVisible;
        }

        public void ClearSelection()
        {
            this.SelectedItem = null;
            this.IsDetailPanelVisible = false;
            this.OnPropertyChanged(nameof(this.IsDetailPanelVisible));
        }

        public (string Name, string Type, string Description, string Price, string ImageUri) GetSelectedItemDetails()
        {
            try
            {
                if (this.SelectedItem == null)
                {
                    return (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                }

                return (
                    Name: this.SelectedItem.Name,
                    Type: this.SelectedItem.ItemType,
                    Description: this.SelectedItem.Description,
                    Price: $"{this.SelectedItem.PointPrice} Points",
                    ImageUri: this.SelectedItem.ImagePath);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error preparing item details: {exception.Message}");
                return (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }

        public async Task<bool> TryPurchaseSelectedItemAsync()
        {
            if (this.SelectedItem == null)
            {
                await this.ShowDialog("Error", "No item selected");
                return false;
            }

            string itemName = this.SelectedItem.Name;
            double pointPrice = this.SelectedItem.PointPrice;
            string itemType = this.SelectedItem.ItemType;

            try
            {
                bool success = await this.PurchaseSelectedItem();

                if (success)
                {
                    bool transactionSuccess = this.pointShopService.TryPurchaseItem(this.SelectedItem, this.transactionHistory, this.user, out PointShopTransaction newTransaction);
                    if (transactionSuccess)
                    {
                        this.TransactionHistory.Add(newTransaction);
                    }

                    this.LoadUserItems();
                    this.LoadItems();

                    await this.ShowDialog("Congrats!", $"You have successfully purchased {itemName}. Check your inventory to view it.");
                    return true;
                }

                await this.ShowDialog("Error", "Purchase failed. Please try again.");
                return false;
            }
            catch (Exception exception)
            {
                await this.ShowDialog("Error", $"Purchase Failed: {exception.Message}");
                return false;
            }
        }

        public async Task ToggleActivationForItemWithMessage(int itemId)
        {
            try
            {
                var item = this.pointShopService.ToggleActivationForItem(itemId, this.UserItems);
                if (item == null)
                {
                    await this.ShowDialog("Item Not Found", "The selected item could not be found.");
                }

                if (item.IsActive)
                {
                    await this.DeactivateItem(item);
                    await this.ShowDialog("Item Deactivated", $"{item.Name} has been deactivated.");
                }
                else
                {
                    await this.ActivateItem(item);
                    await this.ShowDialog("Item Activated", $"{item.Name} has been activated.");
                }
            }
            catch (Exception exception)
            {
                await this.ShowDialog("Error", $"An error occurred while updating the item: {exception.Message}");
            }
        }

        public bool ShouldShowPointsEarnedNotification()
        {
            return Microsoft.UI.Xaml.Application.Current.Resources.TryGetValue("RecentEarnedPoints", out object pointsObj)
                && pointsObj is int earnedPoints && earnedPoints > 0;
        }

        public string GetPointsEarnedMessage()
        {
            if (Microsoft.UI.Xaml.Application.Current.Resources["RecentEarnedPoints"] is int earnedPoints && earnedPoints > NoPointsBalance)
            {
                return $"You earned {earnedPoints} points from your recent purchase!";
            }

            return string.Empty;
        }

        public void ResetEarnedPoints()
        {
            Microsoft.UI.Xaml.Application.Current.Resources["RecentEarnedPoints"] = NoPointsBalance;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async System.Threading.Tasks.Task ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = ConfirmationDialogStrings.OKBUTTONTEXT,
                XamlRoot = App.MainWindow.Content.XamlRoot,
            };

            await dialog.ShowAsync();
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = this.pointShopService.GetFilteredItems(this.filterType, this.searchText, this.minimumPrice, this.maximumPrice);
                this.ShopItems.Clear();
                foreach (var item in filteredItems)
                {
                    this.ShopItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filters: {exception.Message}");
            }
        }

        private async void DelayedSearch(CancellationToken cancellationToken)
        {
            try
            {
                // Wait a bit before searching to avoid excessive updates while typing
                await Task.Delay(PointShopConstants.DELAYTIMESEARCH, cancellationToken);

                // Only apply if not cancelled
                if (!cancellationToken.IsCancellationRequested)
                {
                    this.ApplyFilters();
                }
            }
            catch (TaskCanceledException)
            {
                // Search was cancelled, do nothing
            }
        }
    }
}