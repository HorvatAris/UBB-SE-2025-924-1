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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CtrlAltElite.Pages
{
    public sealed partial class TradingPage : Page
    {
        // Constants (avoid magic strings)
        private const string DisplayMemberUsername = "UserName";
        private const string LoadUsersErrorMessage = "Error loading users. Please try again later.";
        private const string LoadUsersDebugMessagePrefix = "Error loading users: ";
        private const string LoadUserItemsErrorMessage = "Error loading your items. Please try again later.";
        private const string LoadUserItemsDebugMessagePrefix = "Error loading user items: ";
        private const string LoadRecipientItemsErrorMessage = "Error loading recipient's items. Please try again later.";

        private const string LoadRecipientItemsDebugMessagePrefix = "Error loading recipient items: ";
        private const string GameDisplayMemberPath = "GameTitle";
        private const string LoadGamesErrorPrefix = "Error loading games: ";
        private const string LoadGamesInnerErrorPrefix = "Inner error: ";
        private const string LoadGamesSuccessMessagePrefix = "Successfully loaded ";
        private const string LoadActiveTradesErrorMessage = "Error loading active trades. Please try again later.";
        private const string LoadActiveTradesDebugMessagePrefix = "Error loading active trades: ";
        private const string CurrentUserNullMessage = "Current user is null. Cannot load active trades.";
        private const string LoadTradeHistoryErrorMessage = "Error loading trade history. Please try again later.";
        private const string LoadTradeHistoryDebugMessagePrefix = "Error loading trade history: ";
        private const string CurrentUserNullMessageForHistory = "Current user is null. Cannot load trade history.";
        private const string TradeStatusCompleted = "Completed";
        private const string TradeDateTimeDisplayFormat = "MMM dd, yyyy HH:mm";
        private const string ErrorSelectCurrentUser = "Please select your user.";
        private const string ErrorSelectRecipientUser = "Please select a user to trade with.";
        private const string ErrorSelectItems = "Please select at least one item to trade.";
        private const string ErrorMissingDescription = "Please enter a trade description.";
        private const string ErrorUnableToDetermineGame = "Unable to determine the game for the trade.";
        private const string ErrorCreatingTradePrefix = "An error occurred while creating the trade offer: ";
        private const string SuccessTradeCreated = "Trade offer created successfully!";
        private const string DebugTradeCreationErrorPrefix = "Error creating item trade: ";
        private const string DebugInnerExceptionPrefix = "Inner exception: ";
        private const string AcceptTradeErrorPrefix = "Error accepting trade: ";
        private const string DeclineTradeErrorPrefix = "Error declining trade: ";
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

        private async void RecipientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (this.ViewModel?.SelectedUser is User)
            {
                await this.ViewModel.LoadDestinationUserInventory();
            }
        }

        //private async void LoadUserItems()
        //{
        //    try
        //    {
        //        this.itemsOfferedByCurrentUser.Clear();
        //        this.selectedItemsFromCurrentUserInventory.Clear();

        //        if (this.currentUser != null)
        //        {
        //            var userItems = await this.ViewModel.GetUserInventoryAsync(this.currentUser.UserId);
        //            var selectedGame = this.GameComboBox.SelectedItem as Game;

        //            if (selectedGame != null)
        //            {
        //                userItems = userItems.Where(item => item.GetCorrespondingGame().GameId == selectedGame.GameId).ToList();
        //                foreach(var item in userItems)
        //                {
        //                    System.Diagnostics.Debug.WriteLine($"Loading users items {item.ItemId}");
        //                }
        //            }

        //            foreach (var item in userItems)
        //            {
        //                this.itemsOfferedByCurrentUser.Add(item);
        //                System.Diagnostics.Debug.WriteLine(itemsOfferedByCurrentUser);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        this.ErrorMessage.Text = LoadUserItemsErrorMessage;
        //        System.Diagnostics.Debug.WriteLine($"{LoadUserItemsDebugMessagePrefix}{exception.Message}");
        //    }
        //}

        //private async void LoadRecipientItems()
        //{
        //    try
        //    {
        //        this.itemsOfferedByRecipientUser.Clear();
        //        this.selectedItemsFromRecipientUserInventory.Clear();

        //        if (this.recipientUser != null)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"Loading items for recipient user: {this.recipientUser.UserId}");
        //            var recipientItems = await this.ViewModel.GetUserInventoryAsync(this.recipientUser.UserId);
        //            var selectedGame = this.GameComboBox.SelectedItem as Game;

        //            if (selectedGame != null)
        //            {
        //                recipientItems = recipientItems.Where(item => item.GetCorrespondingGame().GameId == selectedGame.GameId).ToList();
        //            }

        //            foreach (var item in recipientItems)
        //            {
        //                this.itemsOfferedByRecipientUser.Add(item);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        this.ErrorMessage.Text = LoadRecipientItemsErrorMessage;
        //        System.Diagnostics.Debug.WriteLine($"{LoadRecipientItemsDebugMessagePrefix}{exception.Message}");
        //    }
        //}

        //private async void LoadGames()
        //{
        //    try
        //    {
        //        var allGames = await this.ViewModel.GetAllGamesAsync();
        //        this.GameComboBox.ItemsSource = allGames;
        //        this.GameComboBox.DisplayMemberPath = GameDisplayMemberPath;

        //        System.Diagnostics.Debug.WriteLine($"{LoadGamesSuccessMessagePrefix}{allGames.Count} games");
        //    }
        //    catch (Exception exception)
        //    {
        //        var errorMessage = $"{LoadGamesErrorPrefix}{exception.Message}";

        //        if (exception.InnerException != null)
        //        {
        //            errorMessage += $"\n{LoadGamesInnerErrorPrefix}{exception.InnerException.Message}";
        //        }

        //        this.ErrorMessage.Text = errorMessage;
        //        System.Diagnostics.Debug.WriteLine(errorMessage);
        //        System.Diagnostics.Debug.WriteLine($"Stack trace: {exception.StackTrace}");
        //    }
        //}


        private async void LoadActiveTrades()
        {
            if (this.ViewModel.CurrentUser == null)
            {
                System.Diagnostics.Debug.WriteLine(CurrentUserNullMessage);
                return;
            }

            try
            {
                var activeTrades = await this.ViewModel.GetActiveTradesAsync(this.ViewModel.CurrentUser.UserId);
                this.ActiveTrades.Clear();

                foreach (var trade in activeTrades)
                {
                    this.ActiveTrades.Add(trade);
                }
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = LoadActiveTradesErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadActiveTradesDebugMessagePrefix}{exception.Message}");
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
            catch (Exception exception)
            {
                this.ErrorMessage.Text = LoadTradeHistoryErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadTradeHistoryDebugMessagePrefix}{exception.Message}");
            }
        }


        private void AddSourceItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            this.ViewModel.AddSourceItemsAsync();
        }

        private void AddDestinationItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            this.ViewModel.AddDestinationItemsAsync();
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

        //private async void CreateTradeOffer_Click(object sender, RoutedEventArgs eventArgs)
        //{
        //    this.ErrorMessage.Text = string.Empty;
        //    this.SuccessMessage.Text = string.Empty;

        //    if (this.currentUser == null)
        //    {
        //        this.ErrorMessage.Text = ErrorSelectCurrentUser;
        //        return;
        //    }

        //    if (this.recipientUser == null)
        //    {
        //        this.ErrorMessage.Text = ErrorSelectRecipientUser;
        //        return;
        //    }

        //    if (!this.selectedItemsFromCurrentUserInventory.Any() && !this.selectedItemsFromRecipientUserInventory.Any())
        //    {
        //        this.ErrorMessage.Text = ErrorSelectItems;
        //        return;
        //    }

        //    string description = this.DescriptionTextBox.Text;
        //    if (string.IsNullOrWhiteSpace(description))
        //    {
        //        this.ErrorMessage.Text = ErrorMissingDescription;
        //        return;
        //    }

        //    try
        //    {
        //        var game = this.selectedItemsFromCurrentUserInventory.FirstOrDefault()?.Game ??
        //                  this.selectedItemsFromRecipientUserInventory.FirstOrDefault()?.Game;

        //        if (game == null)
        //        {
        //            this.ErrorMessage.Text = ErrorUnableToDetermineGame;
        //            return;
        //        }

        //        var itemTrade = new ItemTrade(this.currentUser, this.recipientUser, game, description);

        //        foreach (var item in this.selectedItemsFromCurrentUserInventory)
        //        {
        //            itemTrade.AddSourceUserItem(item);
        //        }

        //        foreach (var item in this.selectedItemsFromRecipientUserInventory)
        //        {
        //            itemTrade.AddDestinationUserItem(item);
        //        }

        //        await this.ViewModel.CreateTradeAsync(itemTrade);

        //        this.GameComboBox.SelectedIndex = NoSelectionIndex;
        //        this.RecipientComboBox.SelectedIndex = NoSelectionIndex;
        //        this.DescriptionTextBox.Text = string.Empty;
        //        this.selectedItemsFromCurrentUserInventory.Clear();
        //        this.selectedItemsFromRecipientUserInventory.Clear();
        //        this.LoadUserItems();
        //        this.LoadRecipientItems();

        //        this.SuccessMessage.Text = SuccessTradeCreated;
        //        this.LoadActiveTrades();
        //        this.LoadTradeHistoryAsync();
        //    }
        //    catch (Exception exception)
        //    {
        //        this.ErrorMessage.Text = $"{ErrorCreatingTradePrefix}{exception.Message}";
        //        System.Diagnostics.Debug.WriteLine($"{DebugTradeCreationErrorPrefix}{exception.Message}");
        //        if (exception.InnerException != null)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"{DebugInnerExceptionPrefix}{exception.InnerException.Message}");
        //        }
        //    }
        //}

        //private void ActiveTradesListView_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        //{
        //    if (sender is ListView listView && listView.SelectedItem is ItemTrade selectedTrade)
        //    {
        //        this.ViewModel.SelectedTrade = selectedTrade;
        //    }
        //}

        //private void AcceptTrade_Click(object sender, RoutedEventArgs eventArgs)
        //{
        //    if (this.ViewModel.SelectedTrade == null)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        this.ViewModel.AcceptTrade(this.ViewModel.SelectedTrade);
        //        this.ViewModel.SelectedTrade = null;
        //        this.LoadActiveTrades();
        //        this.LoadTradeHistoryAsync();
        //    }
        //    catch (Exception exception)
        //    {
        //        this.ErrorMessage.Text = $"{AcceptTradeErrorPrefix}{exception.Message}";
        //        System.Diagnostics.Debug.WriteLine($"{AcceptTradeErrorPrefix}{exception.Message}");
        //    }
        //}

        //private async void DeclineTrade_Click(object sender, RoutedEventArgs eventArgs)
        //{
        //    if (this.ViewModel.SelectedTrade == null)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        await this.ViewModel.DeclineTradeAsync(this.ViewModel.SelectedTrade);
        //        this.ViewModel.SelectedTrade = null;
        //        this.LoadActiveTrades();
        //        this.LoadTradeHistoryAsync();
        //    }
        //    catch (Exception exception)
        //    {
        //        this.ErrorMessage.Text = $"{DeclineTradeErrorPrefix}{exception.Message}";
        //        System.Diagnostics.Debug.WriteLine($"{DeclineTradeErrorPrefix}{exception.Message}");
        //    }
        //}

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
