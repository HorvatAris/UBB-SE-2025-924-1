using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models.ItemTrade
{
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    public class FilterInventoryRequest
    {

        public List<Item> Items { get; set; }
        public Game SelectedGame { get; set; }
        public string SearchText { get; set; }
    }

}
