using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.Services;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamStore.Services;

    /// <summary>
    /// The ViewModel for game and item Trade.
    /// </summary>
    public partial class TradeViewModel : INotifyPropertyChanged
    {
        private readonly TradeService tradeService;
        private readonly UserService userService;
        private readonly GameService gameService;
        private ObservableCollection<Item> sourceUserItems;
        private ObservableCollection<Item> destinationUserItems;
        private ObservableCollection<Item> selectedSourceItems;
        private ObservableCollection<Item> selectedDestinationItems;
        private ObservableCollection<ItemTrade> activeTrades;
        private ObservableCollection<ItemTrade> tradeHistory;
        private User? currentUser;
        private User? selectedUser;
        private Game? selectedGame;
        private string? tradeDescription;
        private ItemTrade? selectedTrade;
        private ObservableCollection<Game> games;
        private ObservableCollection<User> users;
        public const string CannotSendTradeTitle = "Cannot Send Trade";
        public const string CannotSendTradeMessage = "Please select a user to trade with, add items to trade, and provide a trade description.";
        public const string ConfirmTradeTitle = "Confirm Trade";
        public const string ConfirmTradeMessage = "Are you sure you want to send this trade offer?";
        public const string AcceptTradeTitle = "Accept Trade";
        public const string AcceptTradeMessage = "Are you sure you want to accept this trade?";
        public const string DeclineTradeTitle = "Decline Trade";
        public const string DeclineTradeMessage = "Are you sure you want to decline this trade?";
        public const string SendButtonText = "Send";
        public const string AcceptButtonText = "Accept";
        public const string DeclineButtonText = "Decline";
        public const string CancelButtonText = "Cancel";
        public const string OkButtonText = "OK";
        public TradeViewModel(TradeService tradeService, UserService userService, GameService gameService)
        {
            this.tradeService = tradeService;
            this.userService = userService;
            this.gameService = gameService;
            this.sourceUserItems = new ObservableCollection<Item>();
            this.destinationUserItems = new ObservableCollection<Item>();
            this.selectedSourceItems = new ObservableCollection<Item>();
            this.selectedDestinationItems = new ObservableCollection<Item>();
            this.activeTrades = new ObservableCollection<ItemTrade>();
            this.tradeHistory = new ObservableCollection<ItemTrade>();
            this.users = new ObservableCollection<User>();
            this.games = new ObservableCollection<Game>();
            this.LoadInitialData();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public ObservableCollection<User> Users
        {
            get => this.users;
            set
            {
                if (this.users != value)
                {
                    this.users = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Game> Games
        {
            get => this.games;
            set
            {
                if (this.games != value)
                {
                    this.games = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Item> SourceUserItems
        {
            get => this.sourceUserItems;
            set
            {
                if (this.sourceUserItems != value)
                {
                    this.sourceUserItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Item> DestinationUserItems
        {
            get => this.destinationUserItems;
            set
            {
                if (this.destinationUserItems != value)
                {
                    this.destinationUserItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Item> SelectedSourceItems
        {
            get => this.selectedSourceItems;
            set
            {
                if (this.selectedSourceItems != value)
                {
                    this.selectedSourceItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Item> SelectedDestinationItems
        {
            get => this.selectedDestinationItems;
            set
            {
                if (this.selectedDestinationItems != value)
                {
                    this.selectedDestinationItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<ItemTrade> ActiveTrades
        {
            get => this.activeTrades;
            private set
            {
                this.activeTrades = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<ItemTrade> TradeHistory
        {
            get => this.tradeHistory;
            private set
            {
                this.tradeHistory = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<User> AvailableUsers
        {
            get
            {
                if (this.CurrentUser == null)
                {
                    return new ObservableCollection<User>(this.Users);
                }

                return new ObservableCollection<User>(this.Users.Where(userInner => userInner.UserId != this.CurrentUser.UserId));
            }
        }

        /// <inheritdoc/>
        public User? CurrentUser
        {
            get => this.currentUser;
            set
            {
                if (this.currentUser != value)
                {
                    this.currentUser = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(this.AvailableUsers));
                    this.LoadUserInventory();
                    this.LoadActiveTrades();
                    this.LoadTradeHistory();
                }
            }
        }

        /// <inheritdoc/>
        public User? SelectedUser
        {
            get => this.selectedUser;
            set
            {
                if (this.selectedUser != value)
                {
                    this.selectedUser = value;
                    this.OnPropertyChanged();
                    this.LoadDestinationUserInventory();
                }
            }
        }

        /// <inheritdoc/>
        public Game? SelectedGame
        {
            get => this.selectedGame;
            set
            {
                if (this.selectedGame != value)
                {
                    this.selectedGame = value;
                    this.OnPropertyChanged();
                    if (this.CurrentUser != null)
                    {
                        this.LoadUserInventory();
                    }

                    if (this.SelectedUser != null)
                    {
                        this.LoadDestinationUserInventory();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public string? TradeDescription
        {
            get => this.tradeDescription;
            set
            {
                if (this.tradeDescription != value)
                {
                    this.tradeDescription = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public ItemTrade? SelectedTrade
        {
            get => this.selectedTrade;
            set
            {
                this.selectedTrade = value;
                this.OnPropertyChanged(nameof(this.SelectedTrade));
                this.OnPropertyChanged(nameof(this.CanAcceptOrDeclineTrade));
            }
        }

        /// <inheritdoc/>
        public bool CanAcceptOrDeclineTrade => this.SelectedTrade != null;

        /// <inheritdoc/>
        public bool CanSendTradeOffer
        {
            get
            {
                return this.CurrentUser != null &&
                       this.SelectedUser != null &&
                       this.CurrentUser.UserId != this.SelectedUser.UserId &&
                       (this.SelectedSourceItems.Count > 0 || this.SelectedDestinationItems.Count > 0) &&
                       !string.IsNullOrWhiteSpace(this.TradeDescription);
            }
        }

        /// <inheritdoc/>
        public void AddSourceItem(Item item)
        {
            if (item != null && !this.SelectedSourceItems.Contains(item))
            {
                this.SelectedSourceItems.Add(item);
                this.SourceUserItems.Remove(item);
                this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
            }
        }

        /// <inheritdoc/>
        public void RemoveSourceItem(Item item)
        {
            if (item != null)
            {
                this.SelectedSourceItems.Remove(item);
                this.SourceUserItems.Add(item);
                this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
            }
        }

        /// <inheritdoc/>
        public void AddDestinationItem(Item item)
        {
            if (item != null && !this.SelectedDestinationItems.Contains(item))
            {
                this.SelectedDestinationItems.Add(item);
                this.DestinationUserItems.Remove(item);
                this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
            }
        }

        /// <inheritdoc/>
        public void RemoveDestinationItem(Item item)
        {
            if (item != null)
            {
                this.SelectedDestinationItems.Remove(item);
                this.DestinationUserItems.Add(item);
                this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
            }
        }

        /// <inheritdoc/>
        public async Task CreateTradeOffer()
        {
            if (!this.CanSendTradeOffer)
            {
                return;
            }

            try
            {
                if (this.CurrentUser == null || this.SelectedUser == null || this.SelectedGame == null || this.TradeDescription == null)
                {
                    throw new NullReferenceException();
                }

                var trade = new ItemTrade(this.CurrentUser, this.SelectedUser, this.SelectedGame, this.TradeDescription);

                foreach (var item in this.SelectedSourceItems)
                {
                    trade.AddSourceUserItem(item);
                }

                foreach (var item in this.SelectedDestinationItems)
                {
                    trade.AddDestinationUserItem(item);
                }

                await this.tradeService.CreateTradeAsync(trade);
                await this.LoadActiveTradesAsync();

                // Clear selections
                this.SelectedSourceItems.Clear();
                this.SelectedDestinationItems.Clear();
                this.TradeDescription = string.Empty;
                this.LoadUserInventory();
                this.LoadDestinationUserInventory();
            }
            catch (Exception creatingTradeException)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating trade offer: {creatingTradeException.Message}");
            }
        }

        /// <inheritdoc/>
        public async void AcceptTrade(ItemTrade trade)
        {
            try
            {
                if (this.CurrentUser == null)
                {
                    throw new NullReferenceException();
                }

                bool isSourceUser = trade.SourceUser.UserId == this.CurrentUser.UserId;
                await this.tradeService.AcceptTradeAsync(trade, isSourceUser);

                // Clear the selected trade
                this.SelectedTrade = null;

                // Refresh all relevant data
                this.LoadActiveTrades();
                this.LoadTradeHistory();
                this.LoadUserInventory();
                this.LoadDestinationUserInventory();

                // Notify UI of changes
                this.OnPropertyChanged(nameof(this.ActiveTrades));
                this.OnPropertyChanged(nameof(this.TradeHistory));
            }
            catch (Exception acceptingTradeException)
            {
                System.Diagnostics.Debug.WriteLine($"Error accepting trade: {acceptingTradeException.Message}");
                if (acceptingTradeException.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {acceptingTradeException.InnerException.Message}");
                }

                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeclineTradeAsync(ItemTrade trade)
        {
            if (trade == null)
            {
                return false;
            }

            try
            {
                trade.DeclineTradeRequest();
                await this.tradeService.UpdateTradeAsync(trade);

                // Clear the selected trade
                this.SelectedTrade = null;

                // Refresh all relevant data
                this.LoadActiveTrades();
                this.LoadTradeHistory();

                // Notify UI of changes
                this.OnPropertyChanged(nameof(this.ActiveTrades));
                this.OnPropertyChanged(nameof(this.TradeHistory));

                return true;
            }
            catch (Exception decliningTradeException)
            {
                System.Diagnostics.Debug.WriteLine($"Error declining trade: {decliningTradeException.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task LoadUsersAsync()
        {
            try
            {
                var allUsers = await this.userService.GetAllUsersAsync();

                this.Users.Clear();
                foreach (var user in allUsers)
                {
                    this.Users.Add(user);
                }
            }
            catch (Exception loadingUsersException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {loadingUsersException.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task LoadGamesAsync()
        {
            try
            {
                var allGames = await this.gameService.GetAllGames();

                this.Games.Clear();
                foreach (var game in allGames)
                {
                    this.Games.Add(game);
                }
            }
            catch (Exception loadingGamesException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading games: {loadingGamesException.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            return await this.tradeService.GetActiveTradesAsync(userId);
        }

        /// <inheritdoc/>
        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            return await this.tradeService.GetTradeHistoryAsync(userId);
        }

        /// <inheritdoc/>
        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            return await this.tradeService.GetUserInventoryAsync(userId);
        }

        /// <summary>
        /// Calls the service function that asynchronously creates a new trade offer.
        /// </summary>
        /// <param name="trade">The trade offer to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateTradeAsync(ItemTrade trade)
        {
            await this.tradeService.CreateTradeAsync(trade);
        }

        public async Task<List<Game>> GetAllGamesAsync()
        {
            var gamesCollection = await this.gameService.GetAllGames();
            return gamesCollection.ToList(); // Convert Collection<Game> to List<Game>
        }

        /// <summary>
        /// Retrieves all users from the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.userService.GetAllUsersAsync();
        }

        public User GetCurrentUserAsync()
        {
            return this.tradeService.GetCurrentUser();
        }

        public void AddSelectedSourceItems(IList<object> selectedItems)
        {
            foreach (var obj in selectedItems.OfType<Item>())
            {
                this.AddSourceItem(obj);
            }
        }

        public void AddSelectedDestinationItems(IList<object> selectedItems)
        {
            foreach (var obj in selectedItems.OfType<Item>())
            {
                this.AddDestinationItem(obj);
            }

            this.SelectedDestinationItems.Clear();
        }

        public async Task TrySendTradeAsync(XamlRoot root)
        {
            if (!this.CanSendTradeOffer)
            {
                await this.ShowDialogAsync(
                    root,
                    CannotSendTradeTitle,
                    CannotSendTradeMessage,
                    OkButtonText);
                return;
            }

            var result = await this.ShowDialogAsync(
                root,
                ConfirmTradeTitle,
                ConfirmTradeMessage,
                SendButtonText,
                CancelButtonText);

            if (result == ContentDialogResult.Primary)
            {
                await this.CreateTradeOffer();
            }
        }

        /// <summary>
        /// Attempts to accept a given trade after displaying a confirmation dialog to the user.
        /// </summary>
        /// <param name="trade">The trade to accept.</param>
        /// <param name="root">The XamlRoot used to display the confirmation dialog in the UI.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TryAcceptTradeAsync(ItemTrade trade, XamlRoot root)
        {
            var result = await this.ShowDialogAsync(
                root,
                AcceptTradeTitle,
                AcceptTradeMessage,
                AcceptButtonText,
                CancelButtonText);

            if (result == ContentDialogResult.Primary)
            {
                this.AcceptTrade(trade);
            }
        }

        /// <summary>
        /// Attempts to decline a given trade after displaying a confirmation dialog to the user.
        /// </summary>
        /// <param name="trade">The trade to decline.</param>
        /// <param name="root">The XamlRoot used to display the confirmation dialog in the UI.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TryDeclineTradeAsync(ItemTrade trade, XamlRoot root)
        {
            var result = await this.ShowDialogAsync(
                root,
                DeclineTradeTitle,
                DeclineTradeMessage,
                DeclineButtonText,
                CancelButtonText);

            if (result == ContentDialogResult.Primary)
            {
                await this.DeclineTradeAsync(trade);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void LoadInitialData()
        {
            this.LoadCurrentUserAsync();
            await this.LoadUsersAsync();
            await this.LoadGamesAsync();
            await this.LoadActiveTradesAsync();
        }

        private void LoadCurrentUserAsync()
        {
            try
            {
                this.CurrentUser = this.tradeService.GetCurrentUser();
            }
            catch (Exception loadingCurrentUserException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current user: {loadingCurrentUserException.Message}");
            }
        }

        private async void LoadUserInventory()
        {
            if (this.CurrentUser == null)
            {
                return;
            }

            try
            {
                var userInventoryItems = await this.tradeService.GetUserInventoryAsync(this.CurrentUser.UserId);


                this.SourceUserItems.Clear();
                foreach (var item in userInventoryItems.Where(itemInner => !itemInner.IsListed))
                {
                    if (this.SelectedGame == null || item.Game.GameId == this.SelectedGame.GameId)
                    {
                        this.SourceUserItems.Add(item);
                    }
                }
            }
            catch (Exception loadingUserInventoryException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user inventory: {loadingUserInventoryException.Message}");
            }
        }

        private async void LoadDestinationUserInventory()
        {
            if (this.SelectedUser == null)
            {
                return;
            }

            try
            {
                var userInventoryItems = await this.tradeService.GetUserInventoryAsync(this.SelectedUser.UserId);

                this.DestinationUserItems.Clear();
                foreach (var item in userInventoryItems.Where(itemInner => !itemInner.IsListed))
                {
                    if (this.SelectedGame == null || item.Game.GameId == this.SelectedGame.GameId)
                    {
                        this.DestinationUserItems.Add(item);
                    }
                }
            }
            catch (Exception loadingDestinationUserInventoryException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading destination user inventory: {loadingDestinationUserInventoryException.Message}");
            }
        }

        private async Task LoadActiveTradesAsync()
        {
            if (this.CurrentUser == null)
            {
                return;
            }

            try
            {
                var activeTrades = await this.tradeService.GetActiveTradesAsync(this.CurrentUser.UserId);
                this.ActiveTrades.Clear();
                foreach (var trade in activeTrades)
                {
                    this.ActiveTrades.Add(trade);
                }
            }
            catch (Exception loadingActiveTradesException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading active trades: {loadingActiveTradesException.Message}");
            }
        }

        private async void LoadActiveTrades()
        {
            if (this.CurrentUser == null)
            {
                return;
            }

            try
            {
                var activeTrades = await this.tradeService.GetActiveTradesAsync(this.CurrentUser.UserId);
                this.ActiveTrades.Clear();
                foreach (var trade in activeTrades)
                {
                    this.ActiveTrades.Add(trade);
                }
            }
            catch (Exception loadingActiveTradesException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading active trades: {loadingActiveTradesException.Message}");
            }
        }

        private async void LoadTradeHistory()
        {
            if (this.CurrentUser == null)
            {
                return;
            }

            try
            {
                var historyTrades = await this.tradeService.GetTradeHistoryAsync(this.CurrentUser.UserId);
                this.TradeHistory.Clear();
                foreach (var trade in historyTrades)
                {
                    // Only add trades where the current user is involved
                    if (trade.SourceUser.UserId == this.CurrentUser.UserId ||
                        trade.DestinationUser.UserId == this.CurrentUser.UserId)
                    {
                        this.TradeHistory.Add(trade);
                    }
                }

                this.OnPropertyChanged(nameof(this.TradeHistory));
            }
            catch (Exception loadingTradeHistoryException)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading trade history: {loadingTradeHistoryException.Message}");
            }
        }

        private async Task<ContentDialogResult> ShowDialogAsync(XamlRoot root, string title, string content, string? primaryButton = null, string closeButton = "OK")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                XamlRoot = root,
                CloseButtonText = closeButton,
            };

            if (!string.IsNullOrEmpty(primaryButton))
            {
                dialog.PrimaryButtonText = primaryButton;
            }

            return await dialog.ShowAsync();
        }
    }
}
