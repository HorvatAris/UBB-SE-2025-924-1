using SteamHub.ApiContract.Models.Game;

namespace SteamHub.Web.ViewModels
{
    public class GamePageViewModel
    {
        public Game Game { get; set; }
        public List<Game> SimilarGames { get; set; }
        public List<string> GameTags { get; set; }
        public List<string> MediaLinks { get; set; }
        public string FormattedPrice => $"${Game?.Price:F2}";
        public string OwnedStatus => IsOwned ? "Owned" : "Not Owned";
        public bool IsOwned { get; set; }
    }
}
