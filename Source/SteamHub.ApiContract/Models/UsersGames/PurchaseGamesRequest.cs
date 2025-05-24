using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Game;

namespace SteamHub.ApiContract.Models.UsersGames
{
    public class PurchaseGamesRequest
    {
        public int UserId { get; set; }
        public List<SteamHub.ApiContract.Models.Game.Game> Games { get; set; } = new List<SteamHub.ApiContract.Models.Game.Game>();

        public bool IsWalletPayment { get; set; } = false;
    }
}
