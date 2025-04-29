namespace SteamHub.Api.Context.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SteamHub.Api.Entities;

    public class ItemRepository : IItemRepository
    {
        private readonly DataContext _context;

        public ItemRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Item?> GetItemAsync(int itemId)
        {
            // Include the related Game entity in the query.
            return await _context.Items
                .Include(i => i.Game)
                .FirstOrDefaultAsync(i => i.ItemId == itemId);
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            // Retrieve all items along with their related Game.
            return await _context.Items
                .Include(i => i.Game)
                .ToListAsync();
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            // Mark the item as updated.
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(Item item)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
    

}
