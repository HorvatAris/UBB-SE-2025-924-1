using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;
using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;

namespace CtrlAltElite.ServiceProxies
{

    public interface IUserPointShopItemInventoryServiceProxy : IUserPointShopItemInventoryRepository
    {
        [Get("/api/UserPointShopItemInventory/{userId}")]
        Task<GetUserPointShopItemInventoryResponse> GetUserInventoryAsync(int userId);

        [Post("/api/UserPointShopItemInventory/purchase")]
        Task PurchaseItemAsync([Body] PurchasePointShopItemRequest request);

        [Put("/api/UserPointShopItemInventory/update")]
        Task UpdateItemStatusAsync([Body] UpdateUserPointShopItemInventoryRequest request);
    }
}
