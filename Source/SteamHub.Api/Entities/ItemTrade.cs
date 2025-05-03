namespace SteamHub.Api.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ItemTrade
    {
        [Key]
        public int TradeId { get; set; }

        [ForeignKey("SourceUser")]
        public int SourceUserId { get; set; }
        public User SourceUser { get; set; }

        [ForeignKey("DestinationUser")]
        public int DestinationUserId { get; set; }
        public User DestinationUser { get; set; }

        [ForeignKey("GameOfTrade")]
        public int GameOfTradeId { get; set; }
        public Game GameOfTrade { get; set; }

        public DateTime TradeDate { get; set; } = DateTime.UtcNow;

        public string TradeDescription { get; set; }

        public TradeStatus TradeStatus { get; set; } = TradeStatus.Pending;

        public bool AcceptedBySourceUser { get; set; } = false;

        public bool AcceptedByDestinationUser { get; set; } = false;

        public IList<ItemTradeDetail> ItemTradeDetails { get; set; }

    }
}
