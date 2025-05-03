namespace SteamHub.Api.Context.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SteamHub.Api.Entities;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Repositories;

    public class ItemRepository : IItemRepository
    {
        private readonly DataContext _context;

        public ItemRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemDetailedResponse>> GetItemsAsync()
        {
            // Optionally apply filtering from the request parameter.
            var query = _context.Items.AsQueryable();

            // Include the related Game entity.
            var items = await query.Include(i => i.Game).ToListAsync();

            // Map each entity to a detailed response.
            return items.Select(i => new ItemDetailedResponse
            {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                GameId = i.CorrespondingGameId,
                Price = i.Price,
                Description = i.Description,
                IsListed = i.IsListed,
                ImagePath = i.ImagePath
                // You can also include additional game details if needed.
            });
        }

        public async Task<ItemDetailedResponse?> GetItemByIdAsync(int id)
        {
            var item = await _context.Items
                .Include(i => i.Game)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return null;
            }

            return new ItemDetailedResponse
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                GameId = item.CorrespondingGameId,
                Price = item.Price,
                Description = item.Description,
                IsListed = item.IsListed,
                ImagePath = item.ImagePath
            };
        }

        public async Task<ItemDetailedResponse> CreateItemAsync(CreateItemRequest request)
        {
            // Instantiate a new entity from the incoming DTO.
            var newItem = new Item
            {
                ItemName = request.ItemName,
                CorrespondingGameId = request.GameId,
                Price = request.Price,
                Description = request.Description,
                IsListed = request.IsListed,
                ImagePath = request.ImagePath
            };

            await _context.Items.AddAsync(newItem);
            await _context.SaveChangesAsync();

            // Map the newly created entity to the contract response.
            return new ItemDetailedResponse
            {
                ItemId = newItem.ItemId,
                ItemName = newItem.ItemName,
                GameId = newItem.CorrespondingGameId,
                Price = newItem.Price,
                Description = newItem.Description,
                IsListed = newItem.IsListed,
                ImagePath = newItem.ImagePath
            };
        }

        public async Task UpdateItemAsync(int id, UpdateItemRequest request)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Item with id {id} not found.");
            }

            // Update the entity's properties.
            item.ItemName = request.ItemName;
            item.CorrespondingGameId = request.GameId;
            item.Price = request.Price;
            item.Description = request.Description;
            item.IsListed = request.IsListed;
            item.ImagePath = request.ImagePath;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id);

            var userInventory = await _context.UserInventories
                .FirstOrDefaultAsync(ui => ui.ItemId == id);
            _context.UserInventories.RemoveRange(userInventory);

            var itemTradeDetails = _context.ItemTradeDetails
                .Where(itd => itd.ItemId == id);
            _context.ItemTradeDetails.RemoveRange(itemTradeDetails);

            if (item == null)
            {
                throw new KeyNotFoundException($"Item with id {id} not found.");
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
