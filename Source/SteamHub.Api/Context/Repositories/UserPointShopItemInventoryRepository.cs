using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;

namespace SteamHub.Api.Context.Repositories;
public class UserPointShopItemInventoryRepository : IUserPointShopItemInventoryRepository
    {
        private readonly DataContext context;
        public UserPointShopItemInventoryRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<GetUserPointShopItemInventoryResponse> GetUserInventoryAsync(int userId)
        {
            var items = await context.UserPointShopInventories
                .Where(inv => inv.UserId == userId)
                .Include(inv => inv.PointShopItem)
                .Select(inv => new UserPointShopItemInventoryResponse
                {
                    PointShopItemId = inv.PointShopItem.PointShopItemId,
                    PurchaseDate = inv.PurchaseDate,
                    IsActive = inv.IsActive
                })
                .ToListAsync();

            return new GetUserPointShopItemInventoryResponse
            {
                UserPointShopItemsInventory = items
            };
        }

        public async Task PurchaseItemAsync(PurchasePointShopItemRequest request)
        {
            var user = await context.Users.FindAsync(request.UserId);
            if (user == null) throw new Exception("User not found");

            var item = await context.PointShopItems.FindAsync(request.PointShopItemId);
            if (item == null) throw new Exception("Item not found");

            // if somebody calls this method, make sure that the UpdateUser is called with the deducted amount of points !!!!!

            // Add to inventory
            var inventoryItem = new UserPointShopItemInventory
            {
                UserId = user.UserId,
                PointShopItemId = item.PointShopItemId,
                IsActive = false
            };

            await context.UserPointShopInventories.AddAsync(inventoryItem);

            await context.SaveChangesAsync();
        }

        public async Task UpdateItemStatusAsync(UpdateUserPointShopItemInventoryRequest request)
        {
            var inventoryItem = await context.UserPointShopInventories
                .FirstOrDefaultAsync(inv => inv.UserId == request.UserId && inv.PointShopItemId == request.PointShopItemId);

            if (inventoryItem == null)
                throw new Exception("Inventory item not found");

            inventoryItem.IsActive = request.IsActive;
            await context.SaveChangesAsync();
        }

        public async Task ResetUserInventoryAsync(int userId)
        {
            var items = context.UserPointShopInventories.Where(inv => inv.UserId == userId);
            context.UserPointShopInventories.RemoveRange(items);

            await context.SaveChangesAsync();
        }
    }

