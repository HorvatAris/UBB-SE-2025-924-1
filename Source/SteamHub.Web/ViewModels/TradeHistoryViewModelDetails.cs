namespace SteamHub.Web.ViewModels
{
	public class TradeHistoryViewModelDetails
	{
		public string TradeDescription { get; set; } // Description of the trade
		public string TradeStatus { get; set; } // Status of the trade (e.g., Completed, Cancelled)
		public TradeHistoryUserViewModel SourceUser { get; set; } // Source user information
		public List<TradeHistoryItemViewModel> SourceUserItems { get; set; } // Items the source user traded
		public TradeHistoryUserViewModel DestinationUser { get; set; } // Destination user information
		public List<TradeHistoryItemViewModel> DestinationUserItems { get; set; } // Items the destination user traded
	}
}
