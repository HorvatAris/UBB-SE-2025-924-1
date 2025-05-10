using SteamHub.ApiContract.Models.Game;

namespace SteamHub.Web.ViewModels
{
    public class CartPageViewModel
    {
        public List<Game> CartGames { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public int LastEarnedPoints { get; set; }
    }
}
