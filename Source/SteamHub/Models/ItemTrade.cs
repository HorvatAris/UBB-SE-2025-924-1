namespace SteamHub.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a trade between two users involving items from a specific game.
    /// </summary>
    public class ItemTrade
    {
        private const string StatusPending = "Pending";
        private const string StatusCompleted = "Completed";
        private const string StatusDeclined = "Declined";

        private int tradeId;
        private User sourceUser;
        private User destinationUser;
        private Game gameOfTrade;
        private DateTime tradeDate;
        private string tradeDescription;
        private string tradeStatus;
        private bool acceptedBySourceUser;
        private bool acceptedByDestinationUser;
        private List<Item> sourceUserItems;
        private List<Item> destinationUserItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTrade"/> class.
        /// </summary>
        /// <param name="sourceUser">The user initiating the trade.</param>
        /// <param name="destinationUser">The user receiving the trade.</param>
        /// <param name="gameOfTrade">The game the items belong to.</param>
        /// <param name="description">The Description of the trade.</param>
        public ItemTrade(User sourceUser, User destinationUser, Game gameOfTrade, string description)
        {
            sourceUser = sourceUser ?? throw new ArgumentNullException(nameof(sourceUser));
            destinationUser = destinationUser ?? throw new ArgumentNullException(nameof(destinationUser));
            gameOfTrade = gameOfTrade ?? throw new ArgumentNullException(nameof(gameOfTrade));
            this.tradeDescription = description ?? throw new ArgumentNullException(nameof(description));
            this.tradeDate = DateTime.UtcNow;
            this.tradeStatus = StatusPending;
            this.acceptedBySourceUser = false;
            this.acceptedByDestinationUser = false;
            this.sourceUserItems = new List<Item>();
            this.destinationUserItems = new List<Item>();
            this.sourceUser = sourceUser;
            this.destinationUser = destinationUser;
            this.gameOfTrade = gameOfTrade;
        }

        /// <summary>
        /// Gets the unique identifier of the trade.
        /// </summary>
        public int TradeId => this.tradeId;

        /// <summary>
        /// Gets the user who initiated the trade.
        /// </summary>
        public User SourceUser => this.sourceUser;

        /// <summary>
        /// Gets the user who received the trade.
        /// </summary>
        public User DestinationUser => this.destinationUser;

        /// <summary>
        /// Gets the game associated with the trade.
        /// </summary>
        public Game GameOfTrade => this.gameOfTrade;

        /// <summary>
        /// Gets the date and time when the trade was created.
        /// </summary>
        public DateTime TradeDate => this.tradeDate;

        /// <summary>
        /// Gets the Description of the trade.
        /// </summary>
        public string TradeDescription => this.tradeDescription;

        /// <summary>
        /// Gets the current status of the trade.
        /// </summary>
        public string TradeStatus => this.tradeStatus;

        /// <summary>
        /// Gets a value indicating whether the source user accepted the trade.
        /// </summary>
        public bool AcceptedBySourceUser => this.acceptedBySourceUser;

        /// <summary>
        /// Gets a value indicating whether the destination user accepted the trade.
        /// </summary>
        public bool AcceptedByDestinationUser => this.acceptedByDestinationUser;

        /// <summary>
        /// Gets the list of items offered by the source user.
        /// </summary>
        public IReadOnlyList<Item> SourceUserItems => this.sourceUserItems;

        /// <summary>
        /// Gets the list of items offered by the destination user.
        /// </summary>
        public IReadOnlyList<Item> DestinationUserItems => this.destinationUserItems;

        /// <summary>
        /// Sets the unique identifier of the trade.
        /// </summary>
        /// <param name="tradeId">The ID to assign.</param>
        public void SetTradeId(int tradeId)
        {
            this.tradeId = tradeId;
        }

        /// <summary>
        /// Adds an item to the source user's list of offered items.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddSourceUserItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.sourceUserItems.Add(item);
        }

        /// <summary>
        /// Adds an item to the destination user's list of offered items.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddDestinationUserItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.destinationUserItems.Add(item);
        }

        /// <summary>
        /// Marks the trade as accepted by the source user.
        /// </summary>
        public void AcceptBySourceUser()
        {
            this.acceptedBySourceUser = true;
            if (this.acceptedByDestinationUser)
            {
                this.tradeStatus = StatusCompleted;
            }
        }

        /// <summary>
        /// Marks the trade as accepted by the destination user.
        /// </summary>
        public void AcceptByDestinationUser()
        {
            this.acceptedByDestinationUser = true;
            if (this.acceptedBySourceUser)
            {
                this.tradeStatus = StatusCompleted;
            }
        }

        /// <summary>
        /// Declines the trade and resets acceptance flags.
        /// </summary>
        public void DeclineTradeRequest()
        {
            this.tradeStatus = StatusDeclined;
            this.acceptedBySourceUser = false;
            this.acceptedByDestinationUser = false;
        }

        /// <summary>
        /// Completes the trade by setting it as accepted by both users.
        /// </summary>
        public void MarkTradeAsCompleted()
        {
            this.tradeStatus = StatusCompleted;
            this.acceptedBySourceUser = true;
            this.acceptedByDestinationUser = true;
        }
    }
}