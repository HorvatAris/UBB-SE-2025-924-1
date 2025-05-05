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
using Microsoft.UI;
using System.Collections.ObjectModel;
using CtrlAltElite.Models;
using CtrlAltElite.Services;
using SteamStore.Services;
using CtrlAltElite.ViewModels;
using CtrlAltElite.Services.Interfaces;
using SteamStore.Services.Interfaces;
using System.Threading.Tasks;
using System.Diagnostics;
using ABI.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CtrlAltElite.Pages
{
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

        private async void LoadTradeHistoryAsync()
        {
            if (this.ViewModel.CurrentUser == null)
            {
                System.Diagnostics.Debug.WriteLine(CurrentUserNullMessageForHistory);
                return;
            }

            try
            {
                var tradeHistoryEntries = await this.ViewModel.GetTradeHistoryAsync(this.ViewModel.CurrentUser.UserId);
                this.TradeHistory.Clear();

                foreach (var trade in tradeHistoryEntries)
                {
                    bool isCurrentUserSource = trade.SourceUser.UserId == this.ViewModel.CurrentUser.UserId;
                    User tradePartner = isCurrentUserSource ? trade.DestinationUser : trade.SourceUser;

                    var statusColor = trade.TradeStatus == TradeStatusCompleted
                        ? new SolidColorBrush(Colors.Green)
                        : new SolidColorBrush(Colors.Red);

                    this.TradeHistory.Add(new TradeHistoryViewModel
                    {
                        TradeId = trade.TradeId,
                        PartnerName = tradePartner.UserName,
                        TradeItems = trade.SourceUserItems.Concat(trade.DestinationUserItems).ToList(),
                        TradeDescription = trade.TradeDescription,
                        TradeStatus = trade.TradeStatus,
                        TradeDate = trade.TradeDate.ToString(TradeDateTimeDisplayFormat),
                        StatusColor = statusColor,
                        IsSourceUser = isCurrentUserSource,
                    });
                }
            }
            catch (System.Exception exception)
            {
                this.ErrorMessage.Text = LoadTradeHistoryErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadTradeHistoryDebugMessagePrefix}{exception.Message}");
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

        private async void CreateTradeOffer_Click(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.CreateTradeOfferAsync();

            //await this.LoadTradeHistoryAsync();
        }

        private void ActiveTradesListView_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (sender is ListView listView && listView.SelectedItem is ItemTrade selectedTrade)
            {
                this.ViewModel.SelectedTrade = selectedTrade;
            }
        }

        private async void AcceptTrade_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.SelectedTrade == null)
            {
                return;
            }

            await this.ViewModel.AcceptTrade(this.ViewModel.SelectedTrade);
            this.ViewModel.SelectedTrade = null;
        }

        private async void DeclineTrade_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.SelectedTrade == null)
            {
                return;
            }

            await this.ViewModel.DeclineTradeAsync(this.ViewModel.SelectedTrade);
            this.ViewModel.SelectedTrade = null;
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
