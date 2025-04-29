namespace SteamHub.Api.Context
{
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ItemTradeRepository : IItemTradeRepository
    {
        private readonly DataContext context;

        public ItemTradeRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<CreateItemTradeResponse> CreateItemTradeAsync(CreateItemTradeRequest request)
        {
            var newTrade = new ItemTrade
            {
                SourceUserId = request.SourceUserId,
                DestinationUserId = request.DestinationUserId,
                GameOfTradeId = request.GameOfTradeId,
                TradeDescription = request.TradeDescription,
                TradeDate = request.TradeDate ?? DateTime.UtcNow,
                TradeStatus = request.TradeStatus,
                AcceptedBySourceUser = request.AcceptedBySourceUser,
                AcceptedByDestinationUser = request.AcceptedByDestinationUser
            };

            await context.Set<ItemTrade>().AddAsync(newTrade);
            await context.SaveChangesAsync();

            return new CreateItemTradeResponse
            {
                TradeId = newTrade.TradeId
            };
        }

        public async Task<GetItemTradesResponse?> GetItemTradesAsync()
        {
            var trades = await context.Set<ItemTrade>()
                .Include(trade => trade.SourceUser)
                .Include(trade => trade.DestinationUser)
                .Include(trade => trade.GameOfTrade)
                .Select(trade => new ItemTradeResponse
                {
                    TradeId = trade.TradeId,
                    SourceUserId = trade.SourceUserId,
                    DestinationUserId = trade.DestinationUserId,
                    GameOfTradeId = trade.GameOfTradeId,
                    TradeDescription = trade.TradeDescription,
                    TradeDate = trade.TradeDate,
                    TradeStatus = trade.TradeStatus,
                    AcceptedBySourceUser = trade.AcceptedBySourceUser,
                    AcceptedByDestinationUser = trade.AcceptedByDestinationUser
                })
                .ToListAsync();

            return new GetItemTradesResponse
            {
                ItemTrades = trades
            };
        }

        public async Task<ItemTradeResponse?> GetItemTradeByIdAsync(int id)
        {
            var trade = await context.Set<ItemTrade>()
                .Where(t => t.TradeId == id)
                .Select(trade => new ItemTradeResponse
                {
                    TradeId = trade.TradeId,
                    SourceUserId = trade.SourceUserId,
                    DestinationUserId = trade.DestinationUserId,
                    GameOfTradeId = trade.GameOfTradeId,
                    TradeDescription = trade.TradeDescription,
                    TradeDate = trade.TradeDate,
                    TradeStatus = trade.TradeStatus,
                    AcceptedBySourceUser = trade.AcceptedBySourceUser,
                    AcceptedByDestinationUser = trade.AcceptedByDestinationUser
                })
                .SingleOrDefaultAsync();

            return trade;
        }

        public async Task UpdateItemTradeAsync(int tradeId, UpdateItemTradeRequest request)
        {
            var existingTrade = await context.Set<ItemTrade>().FindAsync(tradeId);
            if (existingTrade == null)
            {
                throw new Exception("Trade not found");
            }

            existingTrade.TradeDescription = request.TradeDescription ?? existingTrade.TradeDescription;
            existingTrade.TradeStatus = request.TradeStatus ?? existingTrade.TradeStatus;
            existingTrade.AcceptedBySourceUser = request.AcceptedBySourceUser ?? existingTrade.AcceptedBySourceUser;
            existingTrade.AcceptedByDestinationUser = request.AcceptedByDestinationUser ?? existingTrade.AcceptedByDestinationUser;

            await context.SaveChangesAsync();
        }

        public async Task DeleteItemTradeAsync(int tradeId)
        {
            var trade = await context.Set<ItemTrade>().FindAsync(tradeId);
            if (trade == null)
            {
                throw new Exception("Trade not found");
            }

            context.Set<ItemTrade>().Remove(trade);
            await context.SaveChangesAsync();
        }
    }
}
