namespace SteamHub.Web.ViewModels
{
	public class TradeViewModelDetails
	{
		public int Id { get; set; } // Unique identifier for the trade
		public string TradeDescription { get; set; } // Description of the trade
		public string TradeStatus { get; set; } // Status of the trade (e.g., Pending, Accepted)
		public ActiveTradesUserViewModel SourceUser { get; set; } // Source user information
		public List<ActiveTradesItemViewModel> SourceUserItems { get; set; } // Items the source user is trading
		public ActiveTradesUserViewModel DestinationUser { get; set; } // Destination user information
		public List<ActiveTradesItemViewModel> DestinationUserItems { get; set; } // Items the destination user is trading

	}
}
