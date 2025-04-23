// <copyright file="GameTrade.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Domain.Entities
{
    using System;

    /// <summary>
    /// Represents a trade of a game between two users.
    /// </summary>
    public class GameTrade
    {
        /// <summary>
        /// Represents the default trade status for newly created trades.
        /// </summary>
        private const string DefaultTradeStatus = "Pending";

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTrade"/> class with specified data.
        /// </summary>
        /// <param name="sourceUser">The user initiating the trade.</param>
        /// <param name="destinationUser">The user receiving the trade.</param>
        /// <param name="game">The game being traded.</param>
        /// <param name="description">The trade description.</param>
        public GameTrade(User sourceUser, User destinationUser, Game game, string description)
        {
            this.SourceUser = sourceUser;
            this.DestinationUser = destinationUser;
            this.TradeGame = game;
            this.TradeDescription = description;
            this.TradeDate = DateTime.UtcNow;
            this.AcceptedBySourceUser = false;
            this.AcceptedByDestinationUser = false;
            this.TradeStatus = DefaultTradeStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTrade"/> class for EF Core.
        /// </summary>
        private GameTrade()
        {
        }

        /// <summary>
        /// Gets the unique identifier of the trade.
        /// </summary>
        public int TradeId { get; private set; }

        /// <summary>
        /// Gets the user who initiated the trade.
        /// </summary>
        public User SourceUser { get; private set; } = default!;

        /// <summary>
        /// Gets the user who receives the trade request.
        /// </summary>
        public User DestinationUser { get; private set; } = default!;

        /// <summary>
        /// Gets the date and time when the trade was created.
        /// </summary>
        public DateTime TradeDate { get; private set; }

        /// <summary>
        /// Gets the description of the trade.
        /// </summary>
        public string TradeDescription { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the trade was accepted by the source user.
        /// </summary>
        public bool AcceptedBySourceUser { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the trade was accepted by the destination user.
        /// </summary>
        public bool AcceptedByDestinationUser { get; private set; }

        /// <summary>
        /// Gets the game associated with the trade.
        /// </summary>
        public Game TradeGame { get; private set; } = default!;

        /// <summary>
        /// Gets the status of the trade.
        /// </summary>
        public string TradeStatus { get; private set; } = string.Empty;

        /// <summary>
        /// Sets the unique identifier for the trade.
        /// </summary>
        /// <param name="id">The identifier to assign.</param>
        public void SetTradeId(int id)
        {
            this.TradeId = id;
        }

        /// <summary>
        /// Gets the user who initiated the trade.
        /// </summary>
        /// <returns>The source user.</returns>
        public User GetSourceUser()
        {
            return this.SourceUser;
        }

        /// <summary>
        /// Sets the user who initiated the trade.
        /// </summary>
        /// <param name="user">The source user.</param>
        public void SetSourceUser(User user)
        {
            this.SourceUser = user;
        }

        /// <summary>
        /// Gets the user who received the trade request.
        /// </summary>
        /// <returns>The destination user.</returns>
        public User GetDestinationUser()
        {
            return this.DestinationUser;
        }

        /// <summary>
        /// Sets the user who receives the trade request.
        /// </summary>
        /// <param name="user">The destination user.</param>
        public void SetDestinationUser(User user)
        {
            this.DestinationUser = user;
        }

        /// <summary>
        /// Sets the description of the trade.
        /// </summary>
        /// <param name="description">The description to set.</param>
        public void SetTradeDescription(string description)
        {
            this.TradeDescription = description;
        }

        /// <summary>
        /// Sets the status of the trade.
        /// </summary>
        /// <param name="status">The status to assign.</param>
        public void SetTradeStatus(string status)
        {
            this.TradeStatus = status;
        }

        /// <summary>
        /// Gets the game being traded.
        /// </summary>
        /// <returns>The game associated with the trade.</returns>
        public Game GetTradeGame()
        {
            return this.TradeGame;
        }
    }
}