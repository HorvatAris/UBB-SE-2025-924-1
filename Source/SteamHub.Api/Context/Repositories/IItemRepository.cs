namespace SteamHub.Api.Context.Repositories
{
    using SteamHub.Api.Entities;

    public interface IItemRepository
    {
        Task<Item> AddItemAsync(Item item);

        Task<Item?> GetItemAsync(int itemId);

        Task<IEnumerable<Item>> GetAllItemsAsync();

        Task<Item> UpdateItemAsync(Item item);

        Task DeleteItemAsync(Item item);

        Task<bool> SaveChangesAsync();
    }
}
