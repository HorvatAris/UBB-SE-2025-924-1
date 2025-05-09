using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using System.Collections.ObjectModel;

namespace SteamHub.Web.ViewModels
{
    public class HomePageViewModel
    {
        public string SearchFilterText { get; set; }
        public Collection<Game> TrendingGames { get; set; }
        public Collection<Game> RecommendedGames { get; set; }
        public Collection<Game> DiscountedGames { get; set; }
        public Collection<Tag> Tags { get; set; }
        public Collection<Game> FilteredGames { get; set; }
    }
}
