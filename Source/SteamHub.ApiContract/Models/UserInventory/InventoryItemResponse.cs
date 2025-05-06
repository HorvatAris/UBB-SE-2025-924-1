using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models.UserInventory
{
    public class InventoryItemResponse
    {
        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public float Price { get; set; }

        public string Description { get; set; }

        public bool IsListed { get; set; }

        public string GameName { get; set; }

        public int GameId { get; set; }

        public string ImagePath { get; set; }
    }
}
