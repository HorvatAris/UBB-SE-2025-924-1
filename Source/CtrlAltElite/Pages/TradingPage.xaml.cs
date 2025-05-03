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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CtrlAltElite.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        private User? currentUser;
        private User? recipientUser;
        private ObservableCollection<Item> itemsOfferedByCurrentUser;
        private ObservableCollection<Item> itemsOfferedByRecipientUser;
        private ObservableCollection<Item> selectedItemsFromCurrentUserInventory;
        private ObservableCollection<Item> selectedItemsFromRecipientUserInventory;

        private TradeService tradeService;
        private UserService userService;
        private GameService gameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradingPage"/> class and sets up dependencies.
        /// </summary>
        public TradingPage(TradeService tradeService, UserService userService, GameService gameService)
        {
            this.InitializeComponent();
            this.tradeService = tradeService;
            this.userService = userService;
            this.gameService = gameService;
            //ITradeRepository tradeRepository = new TradeRepository();
            // IUserRepository userRepository = new UserRepository();
            // IGameRepository gameRepository = new GameRepository();


            this.ViewModel = new TradeViewModel(tradeService, userService, gameService);

            this.ActiveTrades = new ObservableCollection<ItemTrade>();
            this.TradeHistory = new ObservableCollection<TradeHistoryViewModel>();
            this.itemsOfferedByCurrentUser = new ObservableCollection<Item>();
            this.itemsOfferedByRecipientUser = new ObservableCollection<Item>();
            this.selectedItemsFromCurrentUserInventory = new ObservableCollection<Item>();
            this.selectedItemsFromRecipientUserInventory = new ObservableCollection<Item>();

            this.SourceItemsListView.ItemsSource = this.itemsOfferedByCurrentUser;
            this.DestinationItemsListView.ItemsSource = this.itemsOfferedByRecipientUser;
            this.SelectedSourceItemsListView.ItemsSource = this.selectedItemsFromCurrentUserInventory;
            this.SelectedDestinationItemsListView.ItemsSource = this.selectedItemsFromRecipientUserInventory;

            this.LoadUsers();
            this.LoadActiveTrades();
            this.LoadTradeHistoryAsync();
            this.LoadGames();
        }

        private TradeViewModel ViewModel { get; set; }

        private ObservableCollection<ItemTrade> ActiveTrades { get; set; }

        private ObservableCollection<TradeHistoryViewModel> TradeHistory { get; set; }

        /// <summary>
        /// Loads all users from the database and binds them to the UserComboBox.
        /// Selects the current logged-in user and refreshes both the active trades and trade history.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the user list or current user could not be loaded from the database.
        /// </exception>
        private async void LoadUsers()
        {
            try
            {
                var allUsers = await this.ViewModel.GetAllUsersAsync();
                this.UserComboBox.ItemsSource = allUsers;
                this.UserComboBox.DisplayMemberPath = DisplayMemberUsername;

                var loggedInUser = this.ViewModel.GetCurrentUserAsync();
                this.currentUser = loggedInUser;

                if (loggedInUser != null)
                {
                    this.UserComboBox.SelectedItem = allUsers.FirstOrDefault(user => user.UserId == loggedInUser.UserId);
                }
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = LoadUsersErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadUsersDebugMessagePrefix}{exception.Message}");
            }
        }

        /// <summary>
        /// Handles the selection change event in the UserComboBox.
        /// Updates the recipient user list (excluding the current user) and loads the selected user's items.
        /// </summary>
        /// <exception cref="Exception">
        /// May throw an exception if the user list cannot be retrieved from the database.
        /// </exception>
        private async void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            this.currentUser = this.UserComboBox.SelectedItem as User;
            if (this.currentUser != null)
            {
                var availableTradePartners = (await this.ViewModel.GetAllUsersAsync()).Where(user => user.UserId != this.currentUser.UserId).ToList();
                this.RecipientComboBox.ItemsSource = availableTradePartners;
                this.RecipientComboBox.DisplayMemberPath = DisplayMemberUsername;

                this.LoadUserItems();
            }
        }

        /// <summary>
        /// Handles the selection change event in the RecipientComboBox.
        /// When a recipient is selected, their inventory is loaded and displayed.
        /// </summary>
        /// <exception cref="Exception">
        /// May throw an exception if recipient data cannot be retrieved from the database.
        /// </exception>
        private void RecipientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            this.recipientUser = this.RecipientComboBox.SelectedItem as User;
            if (this.recipientUser != null)
            {
                this.LoadRecipientItems();
            }
        }

        /// <summary>
        /// Loads the items belonging to the current user.
        /// If a game is selected, only items related to that game will be included.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if user items could not be retrieved or filtered.
        /// </exception>
        private async void LoadUserItems()
        {
            try
            {
                this.itemsOfferedByCurrentUser.Clear();
                this.selectedItemsFromCurrentUserInventory.Clear();

                if (this.currentUser != null)
                {
                    var userItems = await this.ViewModel.GetUserInventoryAsync(this.currentUser.UserId);
                    var selectedGame = this.GameComboBox.SelectedItem as Game;

                    if (selectedGame != null)
                    {
                        userItems = userItems.Where(item => item.GetCorrespondingGame().GameId == selectedGame.GameId).ToList();
                        foreach(var item in userItems)
                        {
                            System.Diagnostics.Debug.WriteLine($"Loading users items {item.ItemId}");
                        }
                    }

                    foreach (var item in userItems)
                    {
                        this.itemsOfferedByCurrentUser.Add(item);
                        System.Diagnostics.Debug.WriteLine(itemsOfferedByCurrentUser);
                    }
                }
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = LoadUserItemsErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadUserItemsDebugMessagePrefix}{exception.Message}");
            }
        }

        /// <summary>
        /// Loads the items belonging to the selected recipient user.
        /// If a game is selected, only items related to that game will be included.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if recipient items could not be retrieved or filtered.
        /// </exception>
        private async void LoadRecipientItems()
        {
            try
            {
                this.itemsOfferedByRecipientUser.Clear();
                this.selectedItemsFromRecipientUserInventory.Clear();

                if (this.recipientUser != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Loading items for recipient user: {this.recipientUser.UserId}");
                    var recipientItems = await this.ViewModel.GetUserInventoryAsync(this.recipientUser.UserId);
                    var selectedGame = this.GameComboBox.SelectedItem as Game;

                    if (selectedGame != null)
                    {
                        recipientItems = recipientItems.Where(item => item.GetCorrespondingGame().GameId == selectedGame.GameId).ToList();
                    }

                    foreach (var item in recipientItems)
                    {
                        this.itemsOfferedByRecipientUser.Add(item);
                    }
                }
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = LoadRecipientItemsErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadRecipientItemsDebugMessagePrefix}{exception.Message}");
            }
        }

        /// <summary>
        /// Loads all available games from the database and binds them to the GameComboBox.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the list of games could not be retrieved from the database.
        /// </exception>
        private async void LoadGames()
        {
            try
            {
                var allGames = await this.ViewModel.GetAllGamesAsync();
                this.GameComboBox.ItemsSource = allGames;
                this.GameComboBox.DisplayMemberPath = GameDisplayMemberPath;

                System.Diagnostics.Debug.WriteLine($"{LoadGamesSuccessMessagePrefix}{allGames.Count} games");
            }
            catch (Exception exception)
            {
                var errorMessage = $"{LoadGamesErrorPrefix}{exception.Message}";

                if (exception.InnerException != null)
                {
                    errorMessage += $"\n{LoadGamesInnerErrorPrefix}{exception.InnerException.Message}";
                }

                this.ErrorMessage.Text = errorMessage;
                System.Diagnostics.Debug.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine($"Stack trace: {exception.StackTrace}");
            }
        }

        /// <summary>
        /// Loads all active item trades for the current user and updates the ActiveTrades collection.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if active trades could not be retrieved from the database.
        /// </exception>
        private async void LoadActiveTrades()
        {
            if (this.currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine(CurrentUserNullMessage);
                return;
            }

            try
            {
                var activeTrades = await this.ViewModel.GetActiveTradesAsync(this.currentUser.UserId);
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

        /// <summary>
        /// Loads the trade history for the current user and populates the TradeHistory collection
        /// with detailed information including partner name, items, status, and colors.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if trade history data could not be retrieved or processed.
        /// </exception>
        private async void LoadTradeHistoryAsync()
        {
            if (this.currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine(CurrentUserNullMessageForHistory);
                return;
            }

            try
            {
                var tradeHistoryEntries = await this.ViewModel.GetTradeHistoryAsync(this.currentUser.UserId);
                this.TradeHistory.Clear();

                foreach (var trade in tradeHistoryEntries)
                {
                    bool isCurrentUserSource = trade.SourceUser.UserId == this.currentUser.UserId;
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

        /// <summary>
        /// Handles the selection change event for the GameComboBox.
        /// Reloads the items for both the current user and the recipient user based on the selected game.
        /// </summary>
        /// <exception cref="Exception">
        /// May be thrown if loading items for the users fails due to a database or processing error.
        /// </exception>
        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            this.LoadUserItems();
            this.LoadRecipientItems();
        }

        /// <summary>
        /// Handles the click event for adding items from the source list to the selected items inventory of the current user.
        /// Moves selected items from the available list to the current user's selected inventory.
        /// </summary>
        /// <exception cref="Exception">
        /// May be thrown if the selected items cannot be cast or modified due to runtime errors.
        /// </exception>
        private void AddSourceItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            var selectedItems = this.SourceItemsListView.SelectedItems.Cast<Item>().ToList();
            foreach (var item in selectedItems)
            {
                if (!this.selectedItemsFromCurrentUserInventory.Contains(item))
                {
                    this.selectedItemsFromCurrentUserInventory.Add(item);
                    this.itemsOfferedByCurrentUser.Remove(item);
                }
            }
        }

        /// <summary>
        /// Handles the click event for adding items from the destination list to the recipient user's selected items inventory.
        /// Moves selected items from the available recipient list to the recipient's selected inventory for trade.
        /// </summary>
        /// <exception cref="Exception">
        /// May be thrown if the selected items cannot be cast or modified due to runtime errors.
        /// </exception>
        private void AddDestinationItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            var selectedItems = this.DestinationItemsListView.SelectedItems.Cast<Item>().ToList();
            foreach (var item in selectedItems)
            {
                if (!this.selectedItemsFromRecipientUserInventory.Contains(item))
                {
                    this.selectedItemsFromRecipientUserInventory.Add(item);
                    this.itemsOfferedByRecipientUser.Remove(item);
                }
            }
        }

        /// <summary>
        /// Handles the click event to remove an item from the current user's selected inventory list.
        /// The item is moved back to the list of available items offered by the current user.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if the sender is not a Button or its Tag is not an Item.
        /// </exception>
        private void RemoveSourceItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is Item item)
            {
                this.selectedItemsFromCurrentUserInventory.Remove(item);
                this.itemsOfferedByCurrentUser.Add(item);
            }
        }

        /// <summary>
        /// Handles the click event to remove an item from the recipient user's selected inventory list.
        /// The item is moved back to the list of available items offered by the recipient user.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if the sender is not a Button or its Tag is not an Item.
        /// </exception>
        private void RemoveDestinationItem_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is Item item)
            {
                this.selectedItemsFromRecipientUserInventory.Remove(item);
                this.itemsOfferedByRecipientUser.Add(item);
            }
        }

        /// <summary>
        /// Handles the click event to create a trade offer between the current user and a selected recipient.
        /// Validates the inputs, creates the trade, stores it in the database, and resets the form.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the trade offer cannot be created or saved to the database.
        /// </exception>
        private async void CreateTradeOffer_Click(object sender, RoutedEventArgs eventArgs)
        {
            this.ErrorMessage.Text = string.Empty;
            this.SuccessMessage.Text = string.Empty;

            if (this.currentUser == null)
            {
                this.ErrorMessage.Text = ErrorSelectCurrentUser;
                return;
            }

            if (this.recipientUser == null)
            {
                this.ErrorMessage.Text = ErrorSelectRecipientUser;
                return;
            }

            if (!this.selectedItemsFromCurrentUserInventory.Any() && !this.selectedItemsFromRecipientUserInventory.Any())
            {
                this.ErrorMessage.Text = ErrorSelectItems;
                return;
            }

            string description = this.DescriptionTextBox.Text;
            if (string.IsNullOrWhiteSpace(description))
            {
                this.ErrorMessage.Text = ErrorMissingDescription;
                return;
            }

            try
            {
                var game = this.selectedItemsFromCurrentUserInventory.FirstOrDefault()?.Game ??
                          this.selectedItemsFromRecipientUserInventory.FirstOrDefault()?.Game;

                if (game == null)
                {
                    this.ErrorMessage.Text = ErrorUnableToDetermineGame;
                    return;
                }

                var itemTrade = new ItemTrade(this.currentUser, this.recipientUser, game, description);

                foreach (var item in this.selectedItemsFromCurrentUserInventory)
                {
                    itemTrade.AddSourceUserItem(item);
                }

                foreach (var item in this.selectedItemsFromRecipientUserInventory)
                {
                    itemTrade.AddDestinationUserItem(item);
                }

                await this.ViewModel.CreateTradeAsync(itemTrade);

                this.GameComboBox.SelectedIndex = NoSelectionIndex;
                this.RecipientComboBox.SelectedIndex = NoSelectionIndex;
                this.DescriptionTextBox.Text = string.Empty;
                this.selectedItemsFromCurrentUserInventory.Clear();
                this.selectedItemsFromRecipientUserInventory.Clear();
                this.LoadUserItems();
                this.LoadRecipientItems();

                this.SuccessMessage.Text = SuccessTradeCreated;
                this.LoadActiveTrades();
                this.LoadTradeHistoryAsync();
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = $"{ErrorCreatingTradePrefix}{exception.Message}";
                System.Diagnostics.Debug.WriteLine($"{DebugTradeCreationErrorPrefix}{exception.Message}");
                if (exception.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"{DebugInnerExceptionPrefix}{exception.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Handles the selection change event in the ActiveTradesListView.
        /// When a trade is selected, it updates the ViewModel with the selected trade.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if the sender is not a ListView or its SelectedItem is not an ItemTrade.
        /// </exception>
        private void ActiveTradesListView_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (sender is ListView listView && listView.SelectedItem is ItemTrade selectedTrade)
            {
                this.ViewModel.SelectedTrade = selectedTrade;
            }
        }

        /// <summary>
        /// Handles the click event for accepting the selected trade.
        /// If a trade is selected, it attempts to accept it and refreshes the trade lists.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if an error occurs while accepting the trade.
        /// </exception>
        private void AcceptTrade_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.SelectedTrade == null)
            {
                return;
            }

            try
            {
                this.ViewModel.AcceptTrade(this.ViewModel.SelectedTrade);
                this.ViewModel.SelectedTrade = null;
                this.LoadActiveTrades();
                this.LoadTradeHistoryAsync();
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = $"{AcceptTradeErrorPrefix}{exception.Message}";
                System.Diagnostics.Debug.WriteLine($"{AcceptTradeErrorPrefix}{exception.Message}");
            }
        }

        /// <summary>
        /// Handles the click event for declining the selected trade.
        /// If a trade is selected, it asynchronously declines the trade and refreshes the trade lists.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if an error occurs while declining the trade.
        /// </exception>
        private async void DeclineTrade_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.SelectedTrade == null)
            {
                return;
            }

            try
            {
                await this.ViewModel.DeclineTradeAsync(this.ViewModel.SelectedTrade);
                this.ViewModel.SelectedTrade = null;
                this.LoadActiveTrades();
                this.LoadTradeHistoryAsync();
            }
            catch (Exception exception)
            {
                this.ErrorMessage.Text = $"{DeclineTradeErrorPrefix}{exception.Message}";
                System.Diagnostics.Debug.WriteLine($"{DeclineTradeErrorPrefix}{exception.Message}");
            }
        }

        /// <summary>
        /// Represents a view model for displaying a trade history entry in the UI.
        /// Includes details about the trade, items involved, status, and partner.
        /// </summary>
        private class TradeHistoryViewModel
        {
            /// <summary>
            /// Gets or sets the unique identifier of the trade.
            /// </summary>
            public int TradeId { get; set; }

            /// <summary>
            /// Gets or sets the username of the trade partner.
            /// Can be null if not available.
            /// </summary>
            public string? PartnerName { get; set; }

            /// <summary>
            /// Gets or sets the list of items involved in the trade.
            /// Can be null if not yet loaded or trade has no items.
            /// </summary>
            public List<Item>? TradeItems { get; set; }

            /// <summary>
            /// Gets or sets the textual description of the trade.
            /// </summary>
            public string? TradeDescription { get; set; }

            /// <summary>
            /// Gets or sets the status of the trade.
            /// </summary>
            public string? TradeStatus { get; set; }

            /// <summary>
            /// Gets or sets the date of the trade formatted as a string.
            /// </summary>
            public string? TradeDate { get; set; }

            /// <summary>
            /// Gets or sets the color used to visually indicate the trade status in the UI.
            /// </summary>
            public SolidColorBrush? StatusColor { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the current user is the source of the trade.
            /// </summary>
            public bool IsSourceUser { get; set; }
        }
    }
}
