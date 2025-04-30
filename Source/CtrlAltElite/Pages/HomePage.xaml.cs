// <copyright file="HomePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace SteamStore.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Microsoft.Extensions.Configuration;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using SteamStore.Models;
    using SteamStore.Services.Interfaces;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private const int MINIMUMPRICEFILTERVALUE = 0;
        private const int MAXIMUMPRICEFILTERVALUE = 200;
        private const int RATINGFILTERVALUE = 0;

        public HomePage(
            IGameService gameService,
            ICartService cartService,
            IUserGameService userGameService)
        {
            this.InitializeComponent();
            this.HomePageViewModel = new HomePageViewModel(gameService, userGameService, cartService);
            this.DataContext = this.HomePageViewModel;
            this.Loaded += async (_, __) => await this.HomePageViewModel.InitAsync();
        }

        private HomePageViewModel HomePageViewModel { get; set; }

        private async void SearchBox_TextChanged(object searchBox, TextChangedEventArgs textChangedEventArgument)
        {
            string user_input = this.SearchBox.Text;
            if (this.DataContext is HomePageViewModel viewModel)
            {
                await viewModel.SearchGames(user_input);
            }

            this.GameListView.UpdateLayout();
        }

        private void FilterButton_Click(object filterButton, RoutedEventArgs filterClickEventArgument)
        {
            this.FilterPopup.IsOpen = true;
        }

        private async void ApplyFilters_Click(object applyFiltersButton, RoutedEventArgs applyFiltersArgument)
        {
            // You can access the filter values from PopupRatingSlider, MinPriceSlider, MaxPriceSlider here.
            int ratingFilter = (int)this.PopupRatingSlider.Value;
            int minimumPrice = (int)this.MinPriceSlider.Value;
            int maximumPrice = (int)this.MaxPriceSlider.Value;
            var selectedTags = new List<string>();
            foreach (var item in this.TagListView.SelectedItems)
            {
                if (item is Tag tag)
                {
                    selectedTags.Add(tag.Tag_name);
                }
            }

            if (this.DataContext is HomePageViewModel viewModel)
            {
                await viewModel.FilterGames(ratingFilter, minimumPrice, maximumPrice, selectedTags.ToArray());
            }

            // Close the popup
            this.FilterPopup.IsOpen = false;
        }

        private async void ResetFilters_Click(object resetFiltersButton, RoutedEventArgs resetFiltersClickEventArgument)
        {
            this.PopupRatingSlider.Value = RATINGFILTERVALUE;
            this.MinPriceSlider.Value = MINIMUMPRICEFILTERVALUE;
            this.MaxPriceSlider.Value = MAXIMUMPRICEFILTERVALUE;
            this.TagListView.SelectedItems.Clear();
            if (this.DataContext is HomePageViewModel viewModel)
            {
                await viewModel.LoadAllGames();
            }

            this.FilterPopup.IsOpen = false;
        }

        // Navigation to GamePage
        private void ListView_SelectionChanged(object gameListView, SelectionChangedEventArgs selectionChangedEventArgument)
        {
            if (gameListView is ListView listView && listView.SelectedItem is Game selectedGame)
            {
                // Get the services from DataContext
                if (this.DataContext is HomePageViewModel viewModel)
                {
                    viewModel.SwitchToGamePage(this.Parent, selectedGame);
                }

                // Clear selection
                listView.SelectedItem = null;
            }
        }
    }
}
