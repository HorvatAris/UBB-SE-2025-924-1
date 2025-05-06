using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models.UserInventory
{
    public class InventoryItemUpdateRequest
    {
        public int SourceUserId { get; set; }

        public int TargetUserId { get; set; }

        public int ItemId { get; set; }
    }
}
