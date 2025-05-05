// <copyright file="PointsShopPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Microsoft.UI.Dispatching;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Media.Imaging;
    using Microsoft.UI.Xaml.Navigation;
    using SteamStore.Models;
    using SteamStore.Services;
    using SteamStore.Services.Interfaces;
    using SteamStore.ViewModels;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    public sealed partial class PointsShopPage : Page
    {
        private const int TimerSeconds = 15;

        public PointsShopPage(IPointShopService pointShopService)
        {
            this.InitializeComponent();

            try
            {
                // Initialize the ViewModel with the PointShopService
                this.ViewModel = new PointShopViewModel(pointShopService);
                this.DataContext = this.ViewModel;

                // Check for earned points
                if (this.ViewModel.ShouldShowPointsEarnedNotification())
                {
                    this.ViewModel.ShowNotification(this.ViewModel.GetPointsEarnedMessage());
                    this.ViewModel.ResetEarnedPoints();
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error initializing PointsShopPage: {exception.Message}");
            }
        }

        private PointShopViewModel ViewModel { get; set; }

        private void ItemsGridView_SelectionChanged(object itemsGridView, SelectionChangedEventArgs itemGridViewArguments)
        {
            if (this.ViewModel.HandleItemSelection())
            {
                this.ViewModel.ItemDetailVisibility = Visibility.Visible;
                var details = this.ViewModel.GetSelectedItemDetails();
                this.ViewModel.SelectedItemName = details.Name;
                this.ViewModel.SelectedItemType = details.Type;
                this.ViewModel.SelectedItemDescription = details.Description;
                this.ViewModel.SelectedItemPrice = details.Price;
                this.ViewModel.SelectedItemImageUri = details.ImageUri;
            }
            else
            {
                this.ViewModel.ItemDetailVisibility = Visibility.Collapsed;
            }
        }

        private void CloseDetailButton_Click(object closeDetailButton, RoutedEventArgs closeButtoClickEventArgument)
        {
            // Hide the item detail panel and clear the selection
            this.ViewModel.ClearSelection();
        }

        private async void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.TryPurchaseSelectedItemAsync();
        }


        private void ViewInventoryButton_Click(object viewInventoryButton, RoutedEventArgs viewInventoryClickEventArgument)
        {
            this.ViewModel.InventoryPanelVisibility = Visibility.Visible;
        }

        private void CloseInventoryButton_Click(object closeInventoryButton, RoutedEventArgs closeInventoryClickEventArgument)
        {
            this.ViewModel.InventoryPanelVisibility = Visibility.Collapsed;
        }

        private async void RemoveButtons_Click(object removeButton, RoutedEventArgs removeClickEventArgument)
        {
            if (removeButton is Button button && int.TryParse(button.Tag?.ToString(), out int itemId))
            {
                await this.ViewModel.ToggleActivationForItemWithMessage(itemId);
            }
        }

        private void Image_ImageFailed(object imageControlSender, ExceptionRoutedEventArgs imageLoadFailEventArgument)
        {
            // Set a default image when loading fails
            if (imageControlSender is Image image)
            {
                // Set a placeholder or default image
                image.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                    new Uri("https://via.placeholder.com/200x200?text=Image+Not+Found"));
            }
        }

        private void ItemTypeFilter_SelectionChanged(object filterComboBoxSender, SelectionChangedEventArgs filterComboBoxArgument)
        {
            if (filterComboBoxSender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string filterType = selectedItem.Content.ToString();
                if (this.ViewModel != null)
                {
                    this.ViewModel.FilterType = filterType;
                    this.ViewModel.ApplyItemTypeFilter(filterType);
                }
            }
        }

        private void CloseNotification_Click(object closeNotificationButton, RoutedEventArgs closeNotificationEventArgument)
        {
            this.ViewModel.HideNotification();

        }

        private void ViewTransactionHistoryButton_Click(object viewTransactionHistoryButton, RoutedEventArgs viewTransactionHistoryClickEventArgument)
        {
            this.ViewModel.TransactionHistoryPanelVisibility = Visibility.Visible;
        }

        private void CloseTransactionHistoryButton_Click(object closeTransactionHistoryButton, RoutedEventArgs closeTransactionHistoryEventArgument)
        {
            this.ViewModel.TransactionHistoryPanelVisibility = Visibility.Collapsed;
        }
    }
}
