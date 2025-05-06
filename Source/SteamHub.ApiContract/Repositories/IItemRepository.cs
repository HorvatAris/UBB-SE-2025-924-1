using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Item;

namespace SteamHub.ApiContract.Repositories
{
    public interface IItemRepository
    {
            Task<IEnumerable<ItemDetailedResponse>> GetItemsAsync();

            Task<ItemDetailedResponse?> GetItemByIdAsync(int id);

            Task<ItemDetailedResponse> CreateItemAsync(CreateItemRequest request);

            Task UpdateItemAsync(int id, UpdateItemRequest request);

            Task DeleteItemAsync(int id);
    }
}
