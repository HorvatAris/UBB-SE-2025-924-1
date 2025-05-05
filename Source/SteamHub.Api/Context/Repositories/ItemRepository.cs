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
            var items = await query.Include(item => item.Game).ToListAsync();

            // Map each entity to a detailed response.
            return items.Select(item => new ItemDetailedResponse
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                GameId = item.CorrespondingGameId,
                Price = item.Price,
                Description = item.Description,
                IsListed = item.IsListed,
                ImagePath = item.ImagePath
                // You can also include additional game details if needed.
            });
        }

        public async Task<ItemDetailedResponse?> GetItemByIdAsync(int id)
        {
            var currentItem = await _context.Items
                .Include(item => item.Game)
                .FirstOrDefaultAsync(item => item.ItemId == id);

            if (currentItem == null)
            {
                return null;
            }

            return new ItemDetailedResponse
            {
                ItemId = currentItem.ItemId,
                ItemName = currentItem.ItemName,
                GameId = currentItem.CorrespondingGameId,
                Price = currentItem.Price,
                Description = currentItem.Description,
                IsListed = currentItem.IsListed,
                ImagePath = currentItem.ImagePath
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
            var currentItem = await _context.Items.FirstOrDefaultAsync(item => item.ItemId == id);
            if (currentItem == null)
            {
                throw new KeyNotFoundException($"Item with id {id} not found.");
            }

            // Update the entity's properties.
            currentItem.ItemName = request.ItemName;
            currentItem.CorrespondingGameId = request.GameId;
            currentItem.Price = request.Price;
            currentItem.Description = request.Description;
            currentItem.IsListed = request.IsListed;
            currentItem.ImagePath = request.ImagePath;

            _context.Items.Update(currentItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var currenttem = await _context.Items.FirstOrDefaultAsync(item => item.ItemId == id);

            var currentUserInventory = await _context.UserInventories
                .FirstOrDefaultAsync(userInventory => userInventory.ItemId == id);
            _context.UserInventories.RemoveRange(currentUserInventory);

            var currentItemTradeDetails = _context.ItemTradeDetails
                .Where(itemTradeDetails => itemTradeDetails.ItemId == id);
            _context.ItemTradeDetails.RemoveRange(currentItemTradeDetails);

            if (currenttem == null)
            {
                throw new KeyNotFoundException($"Item with id {id} not found.");
            }

            _context.Items.Remove(currenttem);
            await _context.SaveChangesAsync();
        }
    }
}
