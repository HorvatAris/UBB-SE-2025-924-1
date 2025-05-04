using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.UserInventory;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories
{
    public class UserInventoryRepository : IUserInventoryRepository
    {
        private readonly DataContext _context;

        public UserInventoryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddItemToUserInventoryAsync(ItemFromInventoryRequest request)
        {
            var userId = await _context.Users.FindAsync(request.UserId);
            if (userId == null)
            {
                throw new ArgumentException("User not found");
            }

            var itemId =await  _context.Items.FindAsync(request.ItemId);
            if (itemId == null)
            {
                throw new ArgumentException("Item not found");
            }

            var gameId = await _context.Games.FindAsync(request.GameId);
            if (gameId == null)
            {
                throw new ArgumentException("Game not found");
            }

            var userInventory = new UserInventory
            {
                UserId = request.UserId,
                ItemId = request.ItemId,
                GameId = request.GameId,
                AcquiredDate = DateTime.Now,
                IsActive = true
            };

            await _context.UserInventories.AddAsync(userInventory);
            await _context.SaveChangesAsync();
        }

        public async Task<InventoryItemResponse?> GetItemFromUserInventoryAsync(int userId, int itemId)
        {
            var userInventory = await _context.UserInventories
                .Include(ui => ui.Item)
                .Include(ui => ui.Game)
                .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.ItemId == itemId);

            if (userInventory == null) return null;

            return new InventoryItemResponse
            {
                ItemId = userInventory.ItemId,
                ItemName = userInventory.Item.ItemName,
                Price = userInventory.Item.Price,
                Description = userInventory.Item.Description,
                IsListed = userInventory.Item.IsListed,
                GameName = userInventory.Game.Name,
                GameId = userInventory.Game.GameId,
                ImagePath = userInventory.Item.ImagePath
            };
        }

        public async Task<UserInventoryResponse> GetUserInventoryAsync(int userId)
        {
            var userInventories = await _context.UserInventories
                .Where(ui => ui.UserId == userId)
                .Include(ui => ui.Item)
                .Include(ui => ui.Game)
                .ToListAsync();

            var items = userInventories.Select(ui => new InventoryItemResponse
            {
                ItemId = ui.ItemId,
                ItemName = ui.Item.ItemName,
                Price = ui.Item.Price,
                Description = ui.Item.Description,
                IsListed = ui.Item.IsListed,
                GameName = ui.Game.Name,
                GameId = ui.Game.GameId,
                ImagePath = ui.Item.ImagePath
            }).ToList();

            return new UserInventoryResponse
            {
                UserId = userId,
                Items = items
            };
        }

        public async Task RemoveItemFromUserInventoryAsync(ItemFromInventoryRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var userInventory = await _context.UserInventories
                .FirstOrDefaultAsync(ui => ui.UserId == request.UserId && ui.ItemId == request.ItemId && ui.GameId == request.GameId);

            if (userInventory == null) throw new ArgumentException("Item not found in user's inventory");

            _context.UserInventories.Remove(userInventory);
            await _context.SaveChangesAsync();
        }
    }
}