// <copyright file="ITradeViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Interface for TradeViewModel.
    /// </summary>
    public interface ITradeViewModel
    {
        /// <summary>
        /// Event for property changes.
        /// </summary>
        event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets activeTrades.
        /// </summary>
        ObservableCollection<ItemTrade> ActiveTrades { get; }

        /// <summary>
        /// Gets a list of all available users.
        /// </summary>
        ObservableCollection<User> AvailableUsers { get; }

        /// <summary>
        /// Gets a value indicating whether or not the trade can be accepted or declined.
        /// </summary>
        bool CanAcceptOrDeclineTrade { get; }

        /// <summary>
        /// Gets a value indicating whether or not a trade offer can be sent.
        /// </summary>
        bool CanSendTradeOffer { get; }

        /// <summary>
        /// Gets or sets currentUser.
        /// </summary>
        User? CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets destinationUserItems.
        /// </summary>
        ObservableCollection<Item> DestinationUserItems { get; set; }

        /// <summary>
        /// Gets or sets games.
        /// </summary>
        ObservableCollection<Game> Games { get; set; }

        /// <summary>
        /// Gets or sets selectedDestinationItems.
        /// </summary>
        ObservableCollection<Item> SelectedDestinationItems { get; set; }

        /// <summary>
        /// Gets or sets selectedGame.
        /// </summary>
        Game? SelectedGame { get; set; }

        /// <summary>
        /// Gets or sets selectedSourceItems.
        /// </summary>
        ObservableCollection<Item> SelectedSourceItems { get; set; }

        /// <summary>
        /// Gets or sets selectedTrade.
        /// </summary>
        ItemTrade? SelectedTrade { get; set; }

        /// <summary>
        /// Gets or sets selectedUser.
        /// </summary>
        User? SelectedUser { get; set; }

        /// <summary>
        /// Gets or sets sourceUserItems.
        /// </summary>
        ObservableCollection<Item> SourceUserItems { get; set; }

        /// <summary>
        /// Gets or sets tradeDescription.
        /// </summary>
        string? TradeDescription { get; set; }

        /// <summary>
        /// Gets tradeHistory.
        /// </summary>
        ObservableCollection<ItemTrade> TradeHistory { get; }

        /// <summary>
        /// Gets or sets users.
        /// </summary>
        ObservableCollection<User> Users { get; set; }

        /// <summary>
        /// Accepts the given trade.
        /// </summary>
        /// <param name="trade">The trade to be accepted.</param>
        void AcceptTrade(ItemTrade trade);

        /// <summary>
        /// Moves an item from the destination user items list to the list of selected items.
        /// </summary>
        /// <param name="item">The item in the destination user items list.</param>
        void AddDestinationItem(Item item);

        /// <summary>
        /// Moves an item from the source user items list to the list of selected items.
        /// </summary>
        /// <param name="item">The item in the source user items list.</param>
        void AddSourceItem(Item item);

        /// <summary>
        /// Creates a trade from the current selections.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateTradeOffer();

        /// <summary>
        /// Calls the function from the service that adds the trade to the database asynchronously.
        /// </summary>
        /// <param name="trade">The trade.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateTradeAsync(ItemTrade trade);

        /// <summary>
        /// Declines the given trade.
        /// </summary>
        /// <param name="trade">The trade to be declined.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeclineTradeAsync(ItemTrade trade);

        /// <summary>
        /// Calls the function from the service to get the active trades for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        /// <summary>
        /// Asynchronously retrieves all games from the database.
        /// </summary>
        /// <returns>A list of all games.</returns>
        Task<List<Game>> GetAllGamesAsync();

        /// <summary>
        /// Calls the functions from the service that asynchronously retrieves all users from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="User"/> objects.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Calls the service function that gets the current user from the database asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<User?> GetCurrentUserAsync();

        /// <summary>
        /// Calls the function from the service to get the trade history for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">id of the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        /// <summary>
        /// Calls the function from the service to get the inventory list for the user asynchronously.
        /// </summary>
        /// <param name="userId">The userID for which to fetch the inventory.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<Item>> GetUserInventoryAsync(int userId);

        /// <summary>
        /// Populates the Games list with data from the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task LoadGamesAsync();

        /// <summary>
        /// Populates the Users list with data from the dabase.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task LoadUsersAsync();

        /// <summary>
        /// Moves an item from the list of selected items back to destination user items list.
        /// </summary>
        /// <param name="item">The item in the selected list.</param>
        void RemoveDestinationItem(Item item);

        /// <summary>
        /// Moves an item from the list of selected items back to source user items list.
        /// </summary>
        /// <param name="item">The item in the selected list.</param>
        void RemoveSourceItem(Item item);
    }
}