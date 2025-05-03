using Refit;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ServiceProxies
{
    interface IItemServiceProxy : IItemRepository
    {
        [Get("/api/items")]
        Task<IEnumerable<ItemDetailedResponse>> GetItemsAsync();

        [Get("/api/items/{id}")]
        Task<ItemDetailedResponse> GetItemByIdAsync(int id);

        [Post("/api/items")]
        Task<ItemDetailedResponse> CreateItemAsync([Body] CreateItemRequest request);

        [Put("/api/items/{id}")]
        Task UpdateItemAsync(int id, [Body] UpdateItemRequest request);

        [Delete("/api/items/{id}")]
        Task DeleteItemAsync(int id);
    }
}
