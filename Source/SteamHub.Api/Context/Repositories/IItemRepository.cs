namespace SteamHub.Api.Context.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.Api.Entities;
    using SteamHub.Api.Models.Item;

    public interface IItemRepository
    {
            Task<IEnumerable<ItemDetailedResponse>> GetItemsAsync(GetItemsRequest request);
            Task<ItemDetailedResponse?> GetItemByIdAsync(int id);
            Task<ItemDetailedResponse> CreateItemAsync(CreateItemRequest request);
            Task UpdateItemAsync(int id, UpdateItemRequest request);
            Task DeleteItemAsync(int id);
    }
}
