using CtrlAltElite.Models;
using CtrlAltElite.ServiceProxies;
using CtrlAltElite.Services.Interfaces;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Models.ItemTradeDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.Services
{
    class TradeService : ITradeService
    {
        private IITemTradeServiceProxy itemTradeServiceProxy;
        private IItemTradeDetailServiceProxy itemTradeDetailServiceProxy;
        private User currentUser;

        public TradeService(IITemTradeServiceProxy itemTradeServiceProxy, User currentUser, IItemTradeDetailServiceProxy itemTradeDetailServiceProxy)
        {
            this.itemTradeServiceProxy = itemTradeServiceProxy;
            this.currentUser = currentUser;
            this.itemTradeDetailServiceProxy = itemTradeDetailServiceProxy;
        }

        public User GetCurrentUser()
        {
            return this.currentUser;
        }

        public Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task AddItemTradeAsync(ItemTrade trade)
        {
            // 1. Create the trade
            var createTradeRequest = new CreateItemTradeRequest
            {
                SourceUserId = trade.SourceUser.UserId,
                DestinationUserId = trade.DestinationUser.UserId,
                GameOfTradeId = trade.GameOfTrade.GameId,
                TradeDate = trade.TradeDate,
                TradeDescription = trade.TradeDescription,
                TradeStatus = TradeStatusEnum.Pending,
                AcceptedBySourceUser = true,  // was hardcoded in SQL
                AcceptedByDestinationUser = false,
            };
            var createTradeResponse = await this.itemTradeServiceProxy.CreateItemTradeAsync(createTradeRequest);
            System.Diagnostics.Debug.WriteLine($"Trade created with ID: {createTradeResponse.TradeId}");
            int tradeId = createTradeResponse.TradeId;
            trade.SetTradeId(tradeId);

            //// 2. Add source user items
            foreach (var item in trade.SourceUserItems)
            {
                System.Diagnostics.Debug.WriteLine($"Adding item {item.ItemId} to trade {tradeId}");
                var detailRequest = new CreateItemTradeDetailRequest
                {
                    TradeId = tradeId,
                    ItemId = item.ItemId,
                    IsSourceUserItem = true,
                };
                await this.itemTradeDetailServiceProxy.CreateItemTradeDetailAsync(detailRequest);
            }

            // 3. Add destination user items
            foreach (var item in trade.DestinationUserItems)
            {
                var detailRequest = new CreateItemTradeDetailRequest
                {
                    TradeId = tradeId,
                    ItemId = item.ItemId,
                    IsSourceUserItem = false,
                };
                await this.itemTradeDetailServiceProxy.CreateItemTradeDetailAsync(detailRequest);
            }
        }

        public async Task TransferItemAsync(int itemId, int fromUserId, int toUserId)
        {


        }
    }
}
