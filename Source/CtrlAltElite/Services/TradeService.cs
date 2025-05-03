using CtrlAltElite.Models;
using CtrlAltElite.ServiceProxies;
using CtrlAltElite.Services.Interfaces;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Models.ItemTradeDetails;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.Services
{
    class TradeService : ITradeService
    {
        private IITemTradeServiceProxy itemTradeServiceProxy;
        private IItemTradeDetailServiceProxy itemTradeDetailServiceProxy;
        private IUserServiceProxy userServiceProxy;
        private IGameServiceProxy gameServiceProxy;
        private IItemServiceProxy itemServiceProxy;
        private User currentUser;

        public TradeService(IITemTradeServiceProxy itemTradeServiceProxy, User currentUser, IItemTradeDetailServiceProxy itemTradeDetailServiceProxy,IUserServiceProxy userServiceProxy,IGameServiceProxy gameServiceProxy,IItemServiceProxy itemServiceProxy)
        {
            this.itemTradeServiceProxy = itemTradeServiceProxy;
            this.currentUser = currentUser;
            this.itemTradeDetailServiceProxy = itemTradeDetailServiceProxy;
            this.userServiceProxy = userServiceProxy;
            this.gameServiceProxy = gameServiceProxy;
            this.itemServiceProxy = itemServiceProxy;
        }

        public async Task MarkTradeAsCompleted(int tradeId)
        {
            var updateRequest = new UpdateItemTradeRequest
            {
                TradeStatus = TradeStatusEnum.Completed,
                AcceptedBySourceUser = true,
                AcceptedByDestinationUser = true,
            };
            await this.itemTradeServiceProxy.UpdateItemTradeAsync(tradeId, updateRequest);
        }

        public async Task DeclineTradeRequest()
        {
            var updateRequest = new UpdateItemTradeRequest
            {
                TradeStatus = TradeStatusEnum.Declined,
                AcceptedBySourceUser = false,
                AcceptedByDestinationUser = false,
            };
        }

        public User GetCurrentUser()
        {
            return this.currentUser;
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

        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            var allTrades = await this.itemTradeServiceProxy.GetAllItemTradesAsync();
            // 1. Get all trades and filter
            var filteredTrades = allTrades.ItemTrades
                .Where(t => (t.SourceUserId == userId || t.DestinationUserId == userId)
                         && t.TradeStatus == TradeStatusEnum.Completed || t.TradeStatus==TradeStatusEnum.Declined) // Fixed comparison to use the enum directly
                .ToList();

            var allUsersApi = (await this.userServiceProxy.GetUsersAsync()).Users;
            var allUsers = allUsersApi
                .Select(u =>
                {
                    var user = new User
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        Email = u.Email,
                        UserRole = (User.Role)u.Role,
                        WalletBalance = u.WalletBalance,
                        PointsBalance = u.PointsBalance,
                    };
                    return user;
                })
                .ToList();
            // 3. Get all games
            var gamesResponse = await this.gameServiceProxy.GetGamesAsync(new GetGamesRequest());
            var allGames = new Collection<Game>(gamesResponse.Select(GameMapper.MapToGame).ToList());
            // 4. Map to domain model
            var result = new List<ItemTrade>();
            foreach (var tradeDto in filteredTrades)
            {
                var sourceUser = allUsers.First(u => u.UserId == tradeDto.SourceUserId);
                var destinationUser = allUsers.First(u => u.UserId == tradeDto.DestinationUserId);

                var game = allGames.FirstOrDefault(g => g.GameId == tradeDto.GameOfTradeId);
                if (game == null) continue; // Skip if game not found

                var itemTrade = new ItemTrade(sourceUser, destinationUser, game, tradeDto.TradeDescription);
                itemTrade.SetTradeId(tradeDto.TradeId);

                // Set trade status
                switch (tradeDto.TradeStatus)
                {
                    case TradeStatusEnum.Completed:
                        itemTrade.MarkTradeAsCompleted();
                        break;
                    case TradeStatusEnum.Declined:
                        itemTrade.DeclineTradeRequest();
                        break;
                }

                result.Add(itemTrade);
            }

            // 5. Enrich each trade with its item details
            var allTradeDetails = (await this.itemTradeDetailServiceProxy.GetAllItemTradeDetailsAsync()).ItemTradeDetails;

            foreach (var trade in result)
            {
                var tradeDetailsForThisTrade = allTradeDetails
                    .Where(d => d.TradeId == trade.TradeId);

                foreach (var detail in tradeDetailsForThisTrade)
                {
                    var itemResponse = await this.itemTradeServiceProxy.GetItemTradeByIdAsync(detail.ItemId);
                    var itemResponseFromItemProxy = await this.itemServiceProxy.GetItemByIdAsync(detail.ItemId);
                    var gameResponse = await this.gameServiceProxy.GetGameByIdAsync(itemResponse.GameOfTradeId);
                    var itemGame = GameMapper.MapToGame(gameResponse);
                    //itemGame.SetGameId(gameResponse.GameId);

                    var item = new Item(itemResponseFromItemProxy.ItemName, itemGame, (float)itemResponseFromItemProxy.Price, itemResponseFromItemProxy.Description);
                    item.SetItemId(itemResponseFromItemProxy.ItemId);
                    item.SetIsListed(itemResponseFromItemProxy.IsListed);

                    if (detail.IsSourceUserItem)
                        trade.AddSourceUserItem(item);
                    else
                        trade.AddDestinationUserItem(item);
                }
            }
            foreach (var r in result)
            {
                System.Diagnostics.Debug.WriteLine($"Trade ID: {r.TradeId}, Source User: {r.SourceUser.UserName}, Destination User: {r.DestinationUser.UserName}, Game: {r.GameOfTrade}");
                System.Diagnostics.Debug.WriteLine(r.SourceUserItems);
                System.Diagnostics.Debug.WriteLine(r.DestinationUserItems);
            }
            return result;
        }

        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            var allTrades = await this.itemTradeServiceProxy.GetAllItemTradesAsync();
            // 1. Get all trades and filter
            var filteredTrades = allTrades.ItemTrades
                .Where(t => (t.SourceUserId == userId || t.DestinationUserId == userId)
                         && t.TradeStatus == TradeStatusEnum.Pending) // Fixed comparison to use the enum directly
                .ToList();

            var allUsersApi = (await this.userServiceProxy.GetUsersAsync()).Users;
            var allUsers = allUsersApi
                .Select(u =>
                {
                    var user = new User
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        Email = u.Email,
                        UserRole = (User.Role)u.Role,
                        WalletBalance = u.WalletBalance,
                        PointsBalance = u.PointsBalance,
                    };
                    return user;
                })
                .ToList();
            // 3. Get all games
            var gamesResponse = await this.gameServiceProxy.GetGamesAsync(new GetGamesRequest());
            var allGames = new Collection<Game>(gamesResponse.Select(GameMapper.MapToGame).ToList());
            // 4. Map to domain model
            var result = new List<ItemTrade>();
            foreach (var tradeDto in filteredTrades)
            {
                var sourceUser = allUsers.First(u => u.UserId == tradeDto.SourceUserId);
                var destinationUser = allUsers.First(u => u.UserId == tradeDto.DestinationUserId);

                var game = allGames.FirstOrDefault(g => g.GameId == tradeDto.GameOfTradeId);
                if (game == null) continue; // Skip if game not found

                var itemTrade = new ItemTrade(sourceUser, destinationUser, game, tradeDto.TradeDescription);
                itemTrade.SetTradeId(tradeDto.TradeId);

                // Set trade status
                switch (tradeDto.TradeStatus)
                {
                    case TradeStatusEnum.Completed:
                        itemTrade.MarkTradeAsCompleted();
                        break;
                    case TradeStatusEnum.Declined:
                        itemTrade.DeclineTradeRequest();
                        break;
                }

                result.Add(itemTrade);
            }

            // 5. Enrich each trade with its item details
            var allTradeDetails = (await this.itemTradeDetailServiceProxy.GetAllItemTradeDetailsAsync()).ItemTradeDetails;

            foreach (var trade in result)
            {
                var tradeDetailsForThisTrade = allTradeDetails
                    .Where(d => d.TradeId == trade.TradeId);

                foreach (var detail in tradeDetailsForThisTrade)
                {
                    var itemResponse = await this.itemTradeServiceProxy.GetItemTradeByIdAsync(detail.ItemId);
                    var itemResponseFromItemProxy = await this.itemServiceProxy.GetItemByIdAsync(detail.ItemId);
                    var gameResponse = await this.gameServiceProxy.GetGameByIdAsync(itemResponse.GameOfTradeId);
                    var itemGame = GameMapper.MapToGame(gameResponse);
                    //itemGame.SetGameId(gameResponse.GameId);

                    var item = new Item(itemResponseFromItemProxy.ItemName, itemGame, (float)itemResponseFromItemProxy.Price, itemResponseFromItemProxy.Description);
                    item.SetItemId(itemResponseFromItemProxy.ItemId);
                    item.SetIsListed(itemResponseFromItemProxy.IsListed);

                    if (detail.IsSourceUserItem)
                        trade.AddSourceUserItem(item);
                    else
                        trade.AddDestinationUserItem(item);
                }
            }
            foreach(var r in result)
            {
                System.Diagnostics.Debug.WriteLine($"Trade ID: {r.TradeId}, Source User: {r.SourceUser.UserName}, Destination User: {r.DestinationUser.UserName}, Game: {r.GameOfTrade}");
                System.Diagnostics.Debug.WriteLine(r.SourceUserItems);
                System.Diagnostics.Debug.WriteLine(r.DestinationUserItems);
            }
            return result;
            //foreach (var trade in filteredTrades)
            //{
            //    var sourceUserDto = allUsers.FirstOrDefault(u => u.UserId == trade.SourceUserId);
            //    var destUserDto = allUsers.FirstOrDefault(u => u.UserId == trade.DestinationUserId);
            //    var gameDto = allGames.FirstOrDefault(g => g.GameId == trade.GameOfTradeId);

            //    if (sourceUserDto == null || destUserDto == null || gameDto == null)
            //    {
            //        continue; // Skip malformed trade
            //    }



            //    var destUser = new User(destUserDto.UserName);
            //    destUser.SetUserId(destUserDto.UserId);

            //    var tradeGame = gameDto;

            //    var domainTrade = new ItemTrade(sourceUser, destUser, tradeGame, trade.TradeDescription);
            //    domainTrade.SetTradeId(trade.TradeId);

            //    if (trade.TradeStatus == TradeStatusEnum.Completed)
            //    {
            //        domainTrade.MarkTradeAsCompleted();
            //    }
            //    else if (trade.TradeStatus == TradeStatusEnum.Declined)
            //    {
            //        domainTrade.DeclineTradeRequest();
            //    }

            //    // Process trade items
            //    foreach (var itemDetail in trade.ItemTradeDetails)
            //    {
            //        var itemDto = itemDetail.Item;

            //        var itemGame = allGames.FirstOrDefault(g => g.GameId == itemDto.CorrespondingGameId);
            //        if (itemGame == null) continue;

            //        var item = new Item(itemDto.ItemName, itemGame, itemDto.Price, itemDto.Description);
            //        item.SetItemId(itemDto.ItemId);
            //        item.SetIsListed(itemDto.IsListed);

            //        if (itemDetail.IsSourceUserItem)
            //        {
            //            domainTrade.AddSourceUserItem(item);
            //        }
            //        else
            //        {
            //            domainTrade.AddDestinationUserItem(item);
            //        }
            //    }

            //    result.Add(domainTrade);
            //}

        }

        public async Task TransferItemAsync(int itemId, int fromUserId, int toUserId)
        {


        }
    }
}
