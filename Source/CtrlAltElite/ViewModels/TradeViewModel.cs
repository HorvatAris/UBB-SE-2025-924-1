// <copyright file="TradeViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using ABI.System;
    using CtrlAltElite.Models;
    using CtrlAltElite.Services.Interfaces;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamStore.Services.Interfaces;

    /// <summary>
    /// Viewmodel for Trade page.
    /// </summary>
    public partial class TradeViewModel : INotifyPropertyChanged
    {
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

        private const string LoadUsersErrorMessage = "Error loading users. Please try again later.";
        private const string LoadUsersDebugMessagePrefix = "Error loading users: ";
        private const string LoadUserItemsErrorMessage = "Error loading your items. Please try again later.";
        private const string LoadUserItemsDebugMessagePrefix = "Error loading user items: ";
        private const string LoadRecipientItemsErrorMessage = "Error loading recipient's items. Please try again later.";
        private const string LoadRecipientItemsDebugMessagePrefix = "Error loading recipient items: ";
        private const string LoadGamesErrorPrefix = "Error loading games: ";
        private const string LoadGamesInnerErrorPrefix = "Inner error: ";
        private const string LoadGamesSuccessMessagePrefix = "Successfully loaded ";
        private const string LoadActiveTradesErrorMessage = "Error loading active trades. Please try again later.";
        private const string LoadActiveTradesDebugMessagePrefix = "Error loading active trades: ";
        private const string LoadTradeHistoryErrorMessage = "Error loading trade history. Please try again later.";
        private const string LoadTradeHistoryDebugMessagePrefix = "Error loading trade history: ";

        private const string ErrorSelectCurrentUser = "Please select your user.";
        private const string ErrorSelectRecipientUser = "Please select a user to trade with.";
        private const string ErrorSelectItems = "Please select at least one item to trade.";
        private const string ErrorMissingDescription = "Please enter a trade description.";
        private const string ErrorUnableToDetermineGame = "Please select the game for the trade.";
        private const string ErrorCreatingTradePrefix = "An error occurred while creating the trade offer: ";
        private const string SuccessTradeCreated = "Trade offer created successfully!";
        private const string DebugTradeCreationErrorPrefix = "Error creating item trade: ";
        private const string DebugInnerExceptionPrefix = "Inner exception: ";
        private const string AcceptTradeErrorPrefix = "Error accepting trade: ";
        private const string DeclineTradeErrorPrefix = "Error declining trade: ";

        private readonly ITradeService tradeService;
        private readonly IUserService userService;
        private readonly IGameService gameService;

        private User? currentUser;
        private User? selectedUser;
        private Game? selectedGame;
        private string? tradeDescription;
        private string errorMessage;
        private string successMessage;
        private ItemTrade? selectedTrade;

        private ObservableCollection<Game> games;
        private ObservableCollection<User> users;
        private ObservableCollection<User> availableUsers;
        private ObservableCollection<Item> itemsOfferedByCurrentUser;
        private ObservableCollection<Item> itemsOfferedByRecipientUser;
        private ObservableCollection<Item> selectedItemsFromCurrentUserInventory;
        private ObservableCollection<Item> selectedItemsFromRecipientUserInventory;

        private ObservableCollection<Item> sourceUserItems;
        private ObservableCollection<Item> destinationUserItems;
        private ObservableCollection<Item> selectedSourceItems;
        private ObservableCollection<Item> selectedDestinationItems;
        private ObservableCollection<ItemTrade> activeTrades;
        private ObservableCollection<ItemTrade> tradeHistory;

        public TradeViewModel(ITradeService tradeService, IUserService userService, IGameService gameService)
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
            this.availableUsers = new ObservableCollection<User>();
            this.games = new ObservableCollection<Game>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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

        public ObservableCollection<ItemTrade> ActiveTrades
        {
            get => this.activeTrades;
            private set
            {
                this.activeTrades = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<ItemTrade> TradeHistory
        {
            get => this.tradeHistory;
            private set
            {
                this.tradeHistory = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<User> AvailableUsers
        {
            get => this.availableUsers;
            set
            {
                if (this.availableUsers != value)
                {
                    this.availableUsers = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public User? CurrentUser
        {
            get => this.currentUser;
            set
            {
                if (this.currentUser != value)
                {
                    this.currentUser = value;
                    this.OnPropertyChanged();
                    if (this.currentUser != null)
                    {
                        _ = this.LoadUserInventoryAsync();
                        _ = this.LoadActiveTradesAsync();
                        _ = this.LoadTradeHistoryAsync();
                    }
                }
            }
        }

        public User? SelectedUser
        {
            get => this.selectedUser;
            set
            {
                if (this.selectedUser != value)
                {
                    this.selectedUser = value;
                    this.OnPropertyChanged();
                    if (this.selectedUser != null)
                    {
                        _ = this.LoadDestinationUserInventoryAsync();
                    }
                }
            }
        }

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
                        _ = this.LoadUserInventoryAsync();
                        _ = this.LoadDestinationUserInventoryAsync();
                    }
                }
            }
        }

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

        public string ErrorMessage
        {
            get => this.errorMessage;
            set
            {
                if (this.errorMessage != value)
                {
                    this.errorMessage = value;
                    this.OnPropertyChanged(); // This must raise PropertyChanged
                }
            }
        }

        public string SuccessMessage
        {
            get => this.successMessage;
            set
            {
                if (this.successMessage != value)
                {
                    this.successMessage = value;
                    this.OnPropertyChanged(); // This must raise PropertyChanged
                }
            }
        }

        public bool CanAcceptOrDeclineTrade => this.SelectedTrade != null;

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

        public async Task InitializeAsync()
        {
            await this.LoadUsersAsync();
            await this.LoadGamesAsync();
        }

        public async Task LoadUsersAsync()
        {
            try
            {
                var loggedInUser = this.tradeService.GetCurrentUser();
                this.Users.Clear();
                this.Users.Add(loggedInUser);
                this.CurrentUser = loggedInUser;

                var allUsers = await this.userService.GetAllUsersAsync();
                this.AvailableUsers.Clear();
                foreach (var user in allUsers)
                {
                    if (user.UserId != this.CurrentUser.UserId)
                    {
                        this.AvailableUsers.Add(user);
                    }
                }

                await this.LoadActiveTradesAsync();
                await this.LoadTradeHistoryAsync();
            }
            catch (System.Exception exception)
            {
                this.ErrorMessage = LoadUsersErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadUsersDebugMessagePrefix}{exception.Message}");

                this.AvailableUsers.Clear();
            }
        }

        public async Task LoadGamesAsync()
        {
            try
            {
                var allGames = await this.gameService.GetAllGamesAsync();

                this.Games.Clear();
                foreach (var game in allGames)
                {
                    this.Games.Add(game);
                }
            }
            catch (System.Exception loadingGamesException)
            {
                var errorMessage = $"{LoadGamesErrorPrefix}{loadingGamesException.Message}";

                if (loadingGamesException.InnerException != null)
                {
                    errorMessage += $"\n{LoadGamesInnerErrorPrefix}{loadingGamesException.InnerException.Message}";
                }

                this.ErrorMessage = errorMessage;
                System.Diagnostics.Debug.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine($"Stack trace: {loadingGamesException.StackTrace}");

                this.Games.Clear();
            }
        }

        private async Task LoadUserInventoryAsync()
        {
            this.SelectedSourceItems.Clear();

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
            catch (System.Exception loadingUserInventoryException)
            {
                this.ErrorMessage = LoadUserItemsErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadUserItemsDebugMessagePrefix}{loadingUserInventoryException.Message}");
            }
        }

        private async Task LoadDestinationUserInventoryAsync()
        {
            this.SelectedDestinationItems.Clear();

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
            catch (System.Exception loadingDestinationUserInventoryException)
            {
                this.ErrorMessage = LoadRecipientItemsErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadRecipientItemsDebugMessagePrefix}{loadingDestinationUserInventoryException.Message}");
            }
        }

        public void AddSourceItems(IEnumerable<Item> selectedItems)
        {
            foreach (var item in selectedItems)
            {
                if (item != null && !this.SelectedSourceItems.Contains(item))
                {
                    this.SelectedSourceItems.Add(item);
                    this.SourceUserItems.Remove(item);
                    this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
                }
            }
        }

        public void RemoveSourceItem(Item item)
        {
            if (item != null)
            {
                this.SelectedSourceItems.Remove(item);
                this.SourceUserItems.Add(item);
                this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
            }
        }

        public void AddDestinationItems(IEnumerable<Item> selectedItems)
        {
            foreach (var item in selectedItems)
            {
                if (item != null && !this.SelectedDestinationItems.Contains(item))
                {
                    this.SelectedDestinationItems.Add(item);
                    this.DestinationUserItems.Remove(item);
                    this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
                }
            }
        }

        public void RemoveDestinationItem(Item item)
        {
            if (item != null)
            {
                this.SelectedDestinationItems.Remove(item);
                this.DestinationUserItems.Add(item);
                this.OnPropertyChanged(nameof(this.CanSendTradeOffer));
            }
        }

        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            return await this.tradeService.GetActiveTradesAsync(userId);
        }

        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            return await this.tradeService.GetTradeHistoryAsync(userId);
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            return await this.tradeService.GetUserInventoryAsync(userId);
        }

        public async Task CreateTradeAsync(ItemTrade trade)
        {
            await this.tradeService.CreateTradeAsync(trade);
        }

        public async Task<List<Game>> GetAllGamesAsync()
        {
            var gamesCollection = await this.gameService.GetAllGamesAsync();
            return gamesCollection.ToList(); // Convert Collection<Game> to List<Game>
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.userService.GetAllUsersAsync();
        }

        public User GetCurrentUserAsync()
        {
            return this.tradeService.GetCurrentUser();
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
                await this.CreateTradeOfferAsync();
            }
        }

        public async Task CreateTradeOfferAsync()
        {
            this.ErrorMessage = string.Empty;
            this.SuccessMessage = string.Empty;

            try
            {
                if (this.CurrentUser == null)
                {
                    throw new InvalidOperationException(ErrorSelectCurrentUser);
                }

                if (this.SelectedUser == null)
                {
                    throw new InvalidOperationException(ErrorSelectRecipientUser);
                }

                if (!this.SelectedSourceItems.Any() && !this.SelectedDestinationItems.Any())
                {
                    throw new InvalidOperationException(ErrorSelectItems);
                }

                if (string.IsNullOrWhiteSpace(this.TradeDescription))
                {
                    throw new InvalidOperationException(ErrorMissingDescription);
                }

                if (this.SelectedGame == null)
                {
                    throw new InvalidOperationException(ErrorUnableToDetermineGame);
                }

                var trade = new ItemTrade
                {
                    SourceUser = this.CurrentUser,
                    DestinationUser = this.SelectedUser,
                    GameOfTrade = this.SelectedGame,
                    TradeDescription = this.TradeDescription,
                    TradeDate = DateTime.UtcNow,
                };

                foreach (var item in this.SelectedSourceItems)
                {
                    trade.SourceUserItems.Add(item);
                }

                foreach (var item in this.SelectedDestinationItems)
                {
                    trade.DestinationUserItems.Add(item);
                }

                await this.tradeService.CreateTradeAsync(trade);
                await this.LoadActiveTradesAsync();
                this.SuccessMessage = SuccessTradeCreated;

                // Clear selections
                this.SelectedSourceItems.Clear();
                this.SelectedDestinationItems.Clear();
                this.TradeDescription = string.Empty;
                await this.LoadUserInventoryAsync();
                await this.LoadDestinationUserInventoryAsync();
            }
            catch (System.Exception creatingTradeException)
            {
                this.ErrorMessage = $"{ErrorCreatingTradePrefix}{creatingTradeException.Message}";
                System.Diagnostics.Debug.WriteLine($"{DebugTradeCreationErrorPrefix}{creatingTradeException.Message}");
                if (creatingTradeException.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"{DebugInnerExceptionPrefix}{creatingTradeException.InnerException.Message}");
                }
            }
        }

        public async Task AcceptTrade(ItemTrade trade)
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
                await this.LoadActiveTradesAsync();
                await this.LoadTradeHistoryAsync();
                await this.LoadUserInventoryAsync();
                await this.LoadDestinationUserInventoryAsync();

                // Notify UI of changes
                this.OnPropertyChanged(nameof(this.ActiveTrades));
                this.OnPropertyChanged(nameof(this.TradeHistory));
            }
            catch (System.Exception acceptingTradeException)
            {
                this.ErrorMessage = $"{AcceptTradeErrorPrefix}{acceptingTradeException.Message}";
                System.Diagnostics.Debug.WriteLine($"{AcceptTradeErrorPrefix}{acceptingTradeException.Message}");
            }
        }

        public async Task DeclineTradeAsync(ItemTrade trade)
        {
            if (trade == null)
            {
                return;
            }

            try
            {
                trade.DeclineTradeRequest();
                await this.tradeService.UpdateTradeAsync(trade);

                // Clear the selected trade
                this.SelectedTrade = null;

                // Refresh all relevant data
                await this.LoadActiveTradesAsync();
                await this.LoadTradeHistoryAsync();

                // Notify UI of changes
                this.OnPropertyChanged(nameof(this.ActiveTrades));
                this.OnPropertyChanged(nameof(this.TradeHistory));
            }
            catch (System.Exception decliningTradeException)
            {
                this.ErrorMessage = $"{DeclineTradeErrorPrefix}{decliningTradeException.Message}";
                System.Diagnostics.Debug.WriteLine($"{DeclineTradeErrorPrefix}{decliningTradeException.Message}");
            }
        }

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
            catch (System.Exception loadingActiveTradesException)
            {
                this.ErrorMessage = LoadActiveTradesErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadActiveTradesDebugMessagePrefix}{loadingActiveTradesException.Message}");
            }
        }

        private async Task LoadTradeHistoryAsync()
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
            catch (System.Exception loadingTradeHistoryException)
            {
                this.ErrorMessage = LoadTradeHistoryErrorMessage;
                System.Diagnostics.Debug.WriteLine($"{LoadTradeHistoryDebugMessagePrefix}{loadingTradeHistoryException.Message}");
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
