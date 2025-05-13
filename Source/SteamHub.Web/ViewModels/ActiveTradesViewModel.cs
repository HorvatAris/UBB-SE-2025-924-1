using Microsoft.AspNetCore.Mvc.Rendering;

namespace SteamHub.Web.ViewModels
{
	public class ActiveTradesViewModel
	{
		public int? CurrentUserId { get; set; } // For the current user selection
		public List<SelectListItem> Users { get; set; } // List of users to populate the select dropdown

		public IEnumerable<TradeViewModelDetails> ActiveTrades { get; set; } // List of active trades to display

		public bool CanAcceptOrDeclineTrade { get; set; } // Determines if the trade buttons should be enabled or not
		public string CurrentUserName { get; set; } // for display only

	}
}
