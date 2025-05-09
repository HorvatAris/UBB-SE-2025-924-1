// <copyright file="ItemTrade.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.ItemTrade
{
    using System;
    using System.Collections.Generic;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;

    public class ItemTrade
    {
        private const string StatusPending = "Pending";
        private const string StatusCompleted = "Completed";
        private const string StatusDeclined = "Declined";

        public ItemTrade()
        {
            SourceUserItems = new List<Item>();
            DestinationUserItems = new List<Item>();
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
            AcceptedBySourceUser = true;
            if (AcceptedByDestinationUser)
            {
                TradeStatus = StatusCompleted;
            }
        }

        public void AcceptByDestinationUser()
        {
            AcceptedByDestinationUser = true;
            if (AcceptedBySourceUser)
            {
                TradeStatus = StatusCompleted;
            }
        }

        public void DeclineTradeRequest()
        {
            TradeStatus = StatusDeclined;
            AcceptedBySourceUser = false;
            AcceptedByDestinationUser = false;
        }

        public void MarkTradeAsCompleted()
        {
            TradeStatus = StatusCompleted;
            AcceptedBySourceUser = true;
            AcceptedByDestinationUser = true;
        }
    }
}