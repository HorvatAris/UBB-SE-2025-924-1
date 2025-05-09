using SteamHub.ApiContract.Models.Game;
using System.Collections.ObjectModel;

namespace SteamHub.Web.ViewModels
{
    public class WishlistViewModel
    {
        public List<Game> WishListGames { get; set; } = new();
        public string Search { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
    }
}
