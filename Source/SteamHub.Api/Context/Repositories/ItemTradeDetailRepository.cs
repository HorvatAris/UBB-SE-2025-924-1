using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.ItemTradeDetails;
using SteamHub.ApiContract.Repositories;
namespace SteamHub.Api.Context.Repositories;
public class ItemTradeDetailRepository : IItemTradeDetailRepository
{
    private readonly DataContext _context;

    public ItemTradeDetailRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<GetItemTradeDetailsResponse?> GetItemTradeDetailsAsync()
    {
        var details = await _context.ItemTradeDetails
            .Select(d => new ItemTradeDetailResponse
            {
                TradeId = d.TradeId,
                ItemId = d.ItemId,
                IsSourceUserItem = d.IsSourceUserItem
            })
            .ToListAsync();

        return new GetItemTradeDetailsResponse
        {
            ItemTradeDetails = details
        };
    }

    public async Task<ItemTradeDetailResponse?> GetItemTradeDetailAsync(int tradeId, int itemId)
    {
        var result = await _context.ItemTradeDetails
            .Where(d => d.TradeId == tradeId && d.ItemId == itemId)
            .Select(d => new ItemTradeDetailResponse
            {
                TradeId = d.TradeId,
                ItemId = d.ItemId,
                IsSourceUserItem = d.IsSourceUserItem
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task<CreateItemTradeDetailResponse> CreateItemTradeDetailAsync(CreateItemTradeDetailRequest request)
    {
        var newDetail = new ItemTradeDetail
        {
            TradeId = request.TradeId,
            ItemId = request.ItemId,
            IsSourceUserItem = request.IsSourceUserItem
        };

        await _context.ItemTradeDetails.AddAsync(newDetail);
        await _context.SaveChangesAsync();

        return new CreateItemTradeDetailResponse
        {
            TradeId = newDetail.TradeId,
            ItemId = newDetail.ItemId
        };
    }

    public async Task DeleteItemTradeDetailAsync(int tradeId, int itemId)
    {
        var detail = await _context.ItemTradeDetails.FindAsync(tradeId, itemId);
        if (detail == null)
        {
            throw new Exception("ItemTradeDetail not found");
        }

        _context.ItemTradeDetails.Remove(detail);
        await _context.SaveChangesAsync();
    }
}