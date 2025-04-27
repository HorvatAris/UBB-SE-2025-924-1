using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public class PointShopItemRepository : IPointShopItemRepository
{
    private readonly DataContext _context;

    public PointShopItemRepository(DataContext context)
    {
        this._context = context;
    }
    public async Task<GetPointShopItemsResponse?> GetPointShopItemsAsync()
    {
        var pointShopItems = await _context.PointShopItems
            .Select(pointShopItem => new PointShopItemResponse
            {
                PointShopItemId = pointShopItem.PointShopItemId,
                Name = pointShopItem.Name,
                Description = pointShopItem.Description,
                ImagePath = pointShopItem.ImagePath,
                PointPrice = pointShopItem.PointPrice,
                ItemType = pointShopItem.ItemType
            })
            .ToListAsync();

        return new GetPointShopItemsResponse
        {
            PointShopItems = pointShopItems
        };
    }

    public async Task<PointShopItemResponse?> GetPointShopItemByIdAsync(int id)
    {
        var result = await _context.PointShopItems
            .Where(item => item.PointShopItemId == id)
            .Select(pointShopItem => new PointShopItemResponse
            {
                PointShopItemId = pointShopItem.PointShopItemId,
                Name = pointShopItem.Name,
                Description = pointShopItem.Description,
                ImagePath = pointShopItem.ImagePath,
                PointPrice = pointShopItem.PointPrice,
                ItemType = pointShopItem.ItemType
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task UpdatePointShopItemAsync(int itemId, UpdatePointShopItemRequest request)
    {
        var existingPointShopItem = await _context.PointShopItems.FindAsync(itemId);
        if (existingPointShopItem == null)
        {
            throw new Exception("PointShopItem not found");
        }

        existingPointShopItem.Name = request.Name;
        existingPointShopItem.Description = request.Description;
        existingPointShopItem.ImagePath = request.ImagePath;
        existingPointShopItem.PointPrice = request.PointPrice;
        existingPointShopItem.ItemType = request.ItemType;

        await _context.SaveChangesAsync();
    }

    public async Task<CreatePointShopItemResponse> CreatePointShopItemAsync(CreatePointShopItemRequest request)
    {
        PointShopItem newPointShopItem = new PointShopItem
        {
            Name = request.Name,
            Description = request.Description,
            ImagePath = request.ImagePath,
            PointPrice = request.PointPrice,
            ItemType = request.ItemType
        };

        await _context.PointShopItems.AddAsync(newPointShopItem);

        await _context.SaveChangesAsync();

        return new CreatePointShopItemResponse
        {
            PointShopItemId = newPointShopItem.PointShopItemId
        };
    }

    public async Task DeletePointShopItemAsync(int id)
    {
        var pointShopItem = await _context.PointShopItems.FindAsync(id);
        if (pointShopItem == null)
        {
            throw new Exception("PointShopItem not found");
        }
        _context.PointShopItems.Remove(pointShopItem);
        await _context.SaveChangesAsync();
    }

}