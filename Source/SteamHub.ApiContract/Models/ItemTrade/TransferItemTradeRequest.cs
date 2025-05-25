using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models.ItemTrade
{
    public class TransferItemTradeRequest
    {
        public int SourceUserId { get; set; }

        public int DestinationUserId { get; set; }

        public int GameId { get; set; }

        public int ItemId { get; set; }
    }
}
