using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.ViewModels
{
    public class InventoryViewModel
    {
        public int? SelectedUserId { get; set; }
        public int? SelectedGameId { get; set; }
        public string SearchText { get; set; }
        public List<Item> InventoryItems { get; set; }
        public List<Game> AvailableGames { get; set; }
        public List<User> AvailableUsers { get; set; }
        public string StatusMessage { get; set; }
    }
}