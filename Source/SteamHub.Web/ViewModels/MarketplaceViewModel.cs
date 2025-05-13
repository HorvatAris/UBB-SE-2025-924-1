namespace SteamHub.Web.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;

    public class MarketplaceViewModel
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public List<User> AvailableUsers { get; set; } = new List<User>();

        public Item SelectedItem { get; set; }
        public User CurrentUser { get; set; }

        public string SearchText { get; set; } = string.Empty;
        public string SelectedGame { get; set; } = string.Empty;
        public string SelectedType { get; set; } = string.Empty;
        public string SelectedRarity { get; set; } = string.Empty;

        public List<string> AvailableGames { get; set; } = new List<string>();
        public List<string> AvailableTypes { get; set; } = new List<string>();
        public List<string> AvailableRarities { get; set; } = new List<string>();

        public bool CanBuyItem =>
            SelectedItem != null && SelectedItem.IsListed && CurrentUser != null;

        public void InitializeFilters()
        {
            AvailableGames = Items
                .Select(i => i.Game.GameTitle)
                .Distinct()
                .ToList();

            AvailableTypes = Items
                .Select(i => i.ItemName.Split('|')[0].Trim())
                .Distinct()
                .ToList();

            AvailableRarities = new List<string> { "Common", "Uncommon", "Rare", "Epic", "Legendary" };
        }
    }
}
