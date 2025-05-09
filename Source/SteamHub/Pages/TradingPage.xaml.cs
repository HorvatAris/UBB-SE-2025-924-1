// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using ABI.System;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ViewModels;
    using Microsoft.UI;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.ItemTrade;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    public sealed partial class TradingPage : Page
    {
        // Constants (avoid magic strings)
        private const string DisplayMemberUsername = "UserName";

        private const string GameDisplayMemberPath = "GameTitle";
        private const string CurrentUserNullMessage = "Current user is null. Cannot load active trades.";
        private const string LoadTradeHistoryErrorMessage = "Error loading trade history. Please try again later.";
        private const string LoadTradeHistoryDebugMessagePrefix = "Error loading trade history: ";
        private const string LoadActiveTradesErrorMessage = "Error loading active trades. Please try again later.";
        private const string LoadActiveTradesDebugMessagePrefix = "Error loading active trades: ";
        private const string CurrentUserNullMessageForHistory = "Current user is null. Cannot load trade history.";
        private const string TradeStatusCompleted = "Completed";
        private const string TradeDateTimeDisplayFormat = "MMM dd, yyyy HH:mm";

        private const int NoSelectionIndex = -1;

        public TradingPage(ITradeService tradeService, IUserService userService, IGameService gameService)
        {
            this.InitializeComponent();
            this.ViewModel = new TradeViewModel(tradeService, userService, gameService);

            this.ActiveTrades = new ObservableCollection<ItemTrade>();
            this.TradeHistory = new ObservableCollection<TradeHistoryViewModel>();

            this.Loaded += this.TradinPage_Loaded;
        }

        private TradeViewModel ViewModel { get; set; }

        private ObservableCollection<ItemTrade> ActiveTrades { get; set; }

        private ObservableCollection<TradeHistoryViewModel> TradeHistory { get; set; }

        private async void TradinPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                await this.ViewModel.InitializeAsync();
            }
        }

        private void AddSourceItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            var selectedItems = this.SourceItemsListView.SelectedItems.Cast<Item>().ToList();
            this.ViewModel.AddSourceItems(selectedItems);
        }

        private void AddDestinationItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            var selectedItems = this.DestinationItemsListView.SelectedItems.Cast<Item>().ToList();
            this.ViewModel.AddDestinationItems(selectedItems);
        }

        private void RemoveSourceItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is Item item)
            {
                this.ViewModel.RemoveSourceItem(item);
            }
        }

        private void RemoveDestinationItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is Item item)
            {
                this.ViewModel.RemoveDestinationItem(item);
            }
        }

        private void ActiveTradesListView_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (sender is ListView listView && listView.SelectedItem is ItemTrade selectedTrade)
            {
                this.ViewModel.SelectedTrade = selectedTrade;
            }
        }

        private async void CreateTradeOffer_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                await this.ViewModel.TrySendTradeAsync(this.XamlRoot);
            }
        }

        private async void AcceptTrade_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.SelectedTrade == null)
            {
                return;
            }

            if (this.ViewModel != null)
            {
                await this.ViewModel.TryAcceptTradeAsync(this.ViewModel.SelectedTrade, this.XamlRoot);
                this.ViewModel.SelectedTrade = null;
            }
        }

        private async void DeclineTrade_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.SelectedTrade == null)
            {
                return;
            }

            if (this.ViewModel != null)
            {
                await this.ViewModel.TryDeclineTradeAsync(this.ViewModel.SelectedTrade, this.XamlRoot);
                this.ViewModel.SelectedTrade = null;
            }
        }

        private class TradeHistoryViewModel
        {
            public int TradeId { get; set; }

            public string? PartnerName { get; set; }

            public List<Item>? TradeItems { get; set; }

            public string? TradeDescription { get; set; }

            public string? TradeStatus { get; set; }

            public string? TradeDate { get; set; }

            public SolidColorBrush? StatusColor { get; set; }

            public bool IsSourceUser { get; set; }
        }
    }
}
