namespace SteamHub.ApiContract.Models.ItemTrade
{
    public class CreateItemTradeRequest
    {
        public int SourceUserId { get; set; }
        public int DestinationUserId { get; set; }
        public int GameOfTradeId { get; set; }
        public string TradeDescription { get; set; }
        public DateTime? TradeDate { get; set; }

        public TradeStatusEnum TradeStatus { get; set; } = TradeStatusEnum.Pending;
        public bool AcceptedBySourceUser { get; set; } = false;
        public bool AcceptedByDestinationUser { get; set; } = false;
    }
}
