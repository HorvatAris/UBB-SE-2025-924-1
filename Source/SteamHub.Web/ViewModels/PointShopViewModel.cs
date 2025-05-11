namespace SteamHub.Web.ViewModels
{
    using System.Collections.Generic;
    using SteamHub.ApiContract.Models.PointShopItem;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models;
    using System.Collections.ObjectModel;

    public class PointShopViewModel
    {
        public List<PointShopItem> ShopItems { get; set; } = new List<PointShopItem>();
        public Collection<PointShopItem> UserItems { get; set; } = new Collection<PointShopItem>();
        public List<PointShopTransaction> TransactionHistory { get; set; } = new List<PointShopTransaction>();

        public IUserDetails User { get; set; }

        public string FilterType { get; set; } = "All";
        public string SearchText { get; set; } = string.Empty;
        public double MinimumPrice { get; set; } = 0;
        public double MaximumPrice { get; set; } = double.MaxValue;

        public PointShopItem SelectedItem { get; set; }

        public float UserPointBalance => User?.PointsBalance ?? 0;

        public bool CanPurchase => SelectedItem != null && User != null &&
                                   SelectedItem.PointPrice <= User.PointsBalance;
    }
}
