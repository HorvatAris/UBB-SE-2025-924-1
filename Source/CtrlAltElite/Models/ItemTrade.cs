namespace CtrlAltElite.Models
{
    using System;
    using System.Collections.Generic;

    public class ItemTrade
    {
        private const string StatusPending = "Pending";
        private const string StatusCompleted = "Completed";
        private const string StatusDeclined = "Declined";

        public ItemTrade()
        {
            this.SourceUserItems = new List<Item>();
            this.DestinationUserItems = new List<Item>();
        }

        public int TradeId { get; set; }

        public User SourceUser { get; set; }

        public User DestinationUser { get; set; }

        public Game GameOfTrade { get; set; }

        public DateTime TradeDate { get; set; }

        public string TradeDescription { get; set; }

        public string TradeStatus { get; set; }

        public bool AcceptedBySourceUser { get; set; }

        public bool AcceptedByDestinationUser { get; set; }

        public List<Item> SourceUserItems { get; set; }

        public List<Item> DestinationUserItems { get; set; }

        public void AcceptBySourceUser()
        {
            this.AcceptedBySourceUser = true;
            if (this.AcceptedByDestinationUser)
            {
                this.TradeStatus = StatusCompleted;
            }
        }

        public void AcceptByDestinationUser()
        {
            this.AcceptedByDestinationUser = true;
            if (this.AcceptedBySourceUser)
            {
                this.TradeStatus = StatusCompleted;
            }
        }

        public void DeclineTradeRequest()
        {
            this.TradeStatus = StatusDeclined;
            this.AcceptedBySourceUser = false;
            this.AcceptedByDestinationUser = false;
        }

        public void MarkTradeAsCompleted()
        {
            this.TradeStatus = StatusCompleted;
            this.AcceptedBySourceUser = true;
            this.AcceptedByDestinationUser = true;
        }
    }
}