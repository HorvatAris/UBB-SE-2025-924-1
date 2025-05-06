namespace SteamHub.ApiContract.Models.ItemTrade
{
    public class ItemTradeResponse
    {
        public int TradeId { get; set; }

        public int SourceUserId { get; set; }

        public int DestinationUserId { get; set; }

        public int GameOfTradeId { get; set; }

        public DateTime TradeDate { get; set; }

        public string TradeDescription { get; set; }

        public TradeStatusEnum TradeStatus { get; set; }

        public bool AcceptedBySourceUser { get; set; }

        public bool AcceptedByDestinationUser { get; set; }
    }
}
