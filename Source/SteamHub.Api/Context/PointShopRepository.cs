using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.Api.Models;
using System.Data;
using System.Threading.Tasks;

namespace SteamHub.Api.Context;

public class PointShopRepository : IPointShopRepository
{
    private readonly DataContext _context;

    public PointShopRepository(DataContext context)
    {
        this._context = context;
    }

    public async Task<List<PointShopItem>> GetAllItemsAsync()
    {
        return await _context.PointShopItems.Include(i => i.UserInventoryItems).ToListAsync();
    }

    public async Task<List<PointShopItem>> GetUserItemsAsync(int userId)
    {
        return await _context.PointShopItems
            .Include(i => i.UserInventoryItems)
            .Where(i => i.UserInventoryItems.Any(ui => ui.UserId == userId))
            .ToListAsync();
    }

    public async Task PurchaseItemAsync(User user, PointShopItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Cannot purchase a null item");
        }

        if (user == null)
        {
            throw new InvalidOperationException("User is not initialized");
        }

        if (user.PointsBalance < item.PointPrice)
        {
            throw new Exception("Insufficient points to purchase this item");
        }

        bool userHasItem = await _context.UserInventoryItems
            .AnyAsync(i => i.UserId == user.UserId && i.ItemIdentifier == item.ItemIdentifier);

        if (userHasItem)
        {
            throw new Exception("User already owns this item");
        }

        await _context.UserInventoryItems
            .AddAsync(new UserInventoryItem
            {
                UserId = user.UserId,
                ItemIdentifier = item.ItemIdentifier,
                PointShopItem = item,
                User = user,
                PurchaseDate = DateTime.Now,
                isActive = false
            });
        await _context.SaveChangesAsync();
        user.PointsBalance -= (float)item.PointPrice;
    }

    public async Task ActivateItemAsync(User user, PointShopItem item)
    {
        if (item == null)
        {
            throw new Exception("Cannot activate a null item");
        }

        if (user == null)
        {
            throw new Exception("User is not initialized");
        }

        var itemToActivate = await _context.PointShopItems
            .Where(i => i.ItemIdentifier == item.ItemIdentifier)
            .FirstOrDefaultAsync();

        if (itemToActivate == null)
        {
            throw new Exception("Item not found");
        }

        var itemsToDeactivate = await _context.UserInventoryItems
            .Where(ui => ui.UserId == user.UserId && ui.PointShopItem.ItemType == itemToActivate.ItemType && ui.PointShopItem.ItemIdentifier != item.ItemIdentifier)
            .ToListAsync();

        foreach (var userInventoryItem in itemsToDeactivate)
        {
            userInventoryItem.PointShopItem.IsActive = false;
        }

        itemToActivate.IsActive = true;

        await _context.SaveChangesAsync();

    }

    public async Task DeactivateItemAsync(User user, PointShopItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Cannot deactivate a null item");
        }

        if (user == null)
        {
            throw new InvalidOperationException("User is not initialized");
        }

        var itemToDeactivate = await _context.PointShopItems
             .Where(i => i.ItemIdentifier == item.ItemIdentifier)
             .FirstOrDefaultAsync();

        if (itemToDeactivate == null)
        {
            throw new Exception("Item not found");
        }

        itemToDeactivate.IsActive = false;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserPointBalanceAsync(User user)
    {
        var userToUpdate = await _context.Users
             .Where(u => u.UserId == user.UserId)
             .FirstOrDefaultAsync();

        if (userToUpdate == null)
        {
            throw new Exception("User not found");
        }

        userToUpdate.PointsBalance = user.PointsBalance;
        await _context.SaveChangesAsync();
    }
}
