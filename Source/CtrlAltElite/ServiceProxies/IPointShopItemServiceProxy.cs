using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;
using SteamHub.ApiContract.Models.PointShopItem;
using SteamHub.ApiContract.Repositories;

namespace CtrlAltElite.ServiceProxies
{
    public interface IPointShopItemServiceProxy : IPointShopItemRepository
    {
        [Get("/api/PointShopItems")]
        Task<GetPointShopItemsResponse> GetPointShopItemsAsync();
    }
}
