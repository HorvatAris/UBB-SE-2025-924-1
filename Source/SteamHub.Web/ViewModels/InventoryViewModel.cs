using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.ViewModels
{
    public class InventoryViewModel
    {
        public List<Item> InventoryItems { get; set; } = new();
        public List<Game> AvailableGames { get; set; } = new();
        public List<User> AvailableUsers { get; set; } = new();

        public int? SelectedGameId { get; set; }
        public int? SelectedUserId { get; set; }
        public string? SearchText { get; set; }
        public int? SelectedItemId { get; set; }
        public string? StatusMessage { get; set; }
    }
}