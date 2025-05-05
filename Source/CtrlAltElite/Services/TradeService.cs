// <copyright file="TradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services.Interfaces;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.ItemTrade;
    using SteamHub.ApiContract.Models.ItemTradeDetails;
    using SteamHub.ApiContract.Models.UserInventory;

    public class TradeService : ITradeService
    {
        private IITemTradeServiceProxy itemTradeServiceProxy;
        private IItemTradeDetailServiceProxy itemTradeDetailServiceProxy;
        private IUserServiceProxy userServiceProxy;
        private IGameServiceProxy gameServiceProxy;
        private IItemServiceProxy itemServiceProxy;
        private IUserInventoryServiceProxy userInventoryServiceProxy;
        private User currentUser;

        public TradeService(IITemTradeServiceProxy itemTradeServiceProxy, User currentUser, IItemTradeDetailServiceProxy itemTradeDetailServiceProxy, IUserServiceProxy userServiceProxy, IGameServiceProxy gameServiceProxy, IItemServiceProxy itemServiceProxy, IUserInventoryServiceProxy userInventoryServiceProxy)
        {
            this.itemTradeServiceProxy = itemTradeServiceProxy;
            this.currentUser = currentUser;
            this.itemTradeDetailServiceProxy = itemTradeDetailServiceProxy;
            this.userServiceProxy = userServiceProxy;
            this.gameServiceProxy = gameServiceProxy;
            this.itemServiceProxy = itemServiceProxy;
            this.userInventoryServiceProxy = userInventoryServiceProxy;
        }

        public async Task MarkTradeAsCompletedAsync(int tradeId)
        {
            var updateRequest = new UpdateItemTradeRequest
            {
                TradeStatus = TradeStatusEnum.Completed,
                AcceptedBySourceUser = true,
                AcceptedByDestinationUser = true,
            };
            await this.itemTradeServiceProxy.UpdateItemTradeAsync(tradeId, updateRequest);
        }

        public void DeclineTradeRequest()
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
            System.Diagnostics.Debug.WriteLine($"Current user: {this.currentUser.UserName}, ID: {this.currentUser.UserId}");
            return this.currentUser;
        }

        // Fix for CS1579: Convert the `itemsToTransfer` variable to its list property `ItemTradeDetails` before iterating.
        public async Task UpdateItemTradeAsync(ItemTrade trade)
        {
            var tradeStatus = TradeStatusEnum.Pending; // Default value
            if (Enum.TryParse(trade.TradeStatus, out TradeStatusEnum parsedStatus))
            {
                tradeStatus = parsedStatus;
            }

            // 1. Prepare the update trade request
            var updateTradeRequest = new UpdateItemTradeRequest
            {
                TradeDescription = trade.TradeDescription, // You may or may not want to update this
                TradeStatus = trade.AcceptedByDestinationUser ? TradeStatusEnum.Completed : tradeStatus,
                AcceptedBySourceUser = trade.AcceptedBySourceUser,
                AcceptedByDestinationUser = trade.AcceptedByDestinationUser,
            };
            System.Diagnostics.Debug.WriteLine($"Updating trade with ID: {trade.TradeId} to status: {updateTradeRequest.TradeStatus}");

            await this.itemTradeServiceProxy.UpdateItemTradeAsync(trade.TradeId, updateTradeRequest);
        }

        public async Task TransferItemAsync(int itemId, int fromUserId, int toUserId, int gameId)
        {
            var removeRequest = new ItemFromInventoryRequest
            {
                UserId = fromUserId,
                ItemId = itemId,
                GameId = gameId,
            };

            var addRequest = new ItemFromInventoryRequest
            {
                UserId = toUserId,
                ItemId = itemId,
                GameId = gameId,
            };

            try
            {
                // Remove the item from the source user
                await this.userInventoryServiceProxy.RemoveItemFromUserInventoryAsync(removeRequest);

                // Add the item to the destination user
                await this.userInventoryServiceProxy.AddItemToUserInventoryAsync(addRequest);

                System.Diagnostics.Debug.WriteLine($"Successfully transferred item {itemId} from user {fromUserId} to user {toUserId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error transferring item {itemId}: {ex.Message}");
                throw new Exception($"Failed to transfer item {itemId} from user {fromUserId} to user {toUserId}", ex);
            }
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
            trade.TradeId = createTradeResponse.TradeId;

            //// 2. Add source user items
            foreach (var item in trade.SourceUserItems)
            {
                System.Diagnostics.Debug.WriteLine($"Adding item {item.ItemId} to trade {trade.TradeId}");
                var detailRequest = new CreateItemTradeDetailRequest
                {
                    TradeId = trade.TradeId,
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
                    TradeId = trade.TradeId,
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
                .Where(trade => ((trade.SourceUserId == userId || trade.DestinationUserId == userId)
                         && trade.TradeStatus == TradeStatusEnum.Completed) || trade.TradeStatus == TradeStatusEnum.Declined) // Fixed comparison to use the enum directly
                .ToList();

            var allUsersApi = (await this.userServiceProxy.GetUsersAsync()).Users;
            var allUsers = allUsersApi
                .Select(currentUser =>
                {
                    var user = new User
                    {
                        UserId = currentUser.UserId,
                        UserName = currentUser.UserName,
                        Email = currentUser.Email,
                        UserRole = (User.Role)currentUser.Role,
                        WalletBalance = currentUser.WalletBalance,
                        PointsBalance = currentUser.PointsBalance,
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
                if (game == null)
                {
                    continue; // Skip if game not found
                }

                var itemTrade = new ItemTrade
                {
                    TradeId = tradeDto.TradeId,
                    SourceUser = sourceUser,
                    DestinationUser = destinationUser,
                    GameOfTrade = game,
                    TradeDescription = tradeDto.TradeDescription,
                };

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
                    .Where(tradeDetail => tradeDetail.TradeId == trade.TradeId);

                foreach (var detail in tradeDetailsForThisTrade)
                {
                    var itemResponse = await this.itemTradeServiceProxy.GetItemTradeByIdAsync(detail.TradeId);
                    var itemResponseFromItemProxy = await this.itemServiceProxy.GetItemByIdAsync(detail.ItemId);
                    var gameResponse = await this.gameServiceProxy.GetGameByIdAsync(itemResponse.GameOfTradeId);
                    var itemGame = GameMapper.MapToGame(gameResponse);

                    // itemGame.SetGameId(gameResponse.GameId);
                    var item = new Item(itemResponseFromItemProxy.ItemName, itemGame, (float)itemResponseFromItemProxy.Price, itemResponseFromItemProxy.Description);
                    item.SetItemId(itemResponseFromItemProxy.ItemId);
                    item.SetIsListed(itemResponseFromItemProxy.IsListed);

                    if (detail.IsSourceUserItem)
                    {
                        trade.SourceUserItems.Add(item);
                    }
                    else
                    {
                        trade.DestinationUserItems.Add(item);
                    }
                }
            }

            foreach (var tradeFromHistory in result)
            {
                System.Diagnostics.Debug.WriteLine($"Trade ID: {tradeFromHistory.TradeId}, Source User: {tradeFromHistory.SourceUser.UserName}, Destination User: {tradeFromHistory.DestinationUser.UserName}, Game: {tradeFromHistory.GameOfTrade}");
                System.Diagnostics.Debug.WriteLine(tradeFromHistory.SourceUserItems);
                System.Diagnostics.Debug.WriteLine(tradeFromHistory.DestinationUserItems);
            }

            return result;
        }

        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            var allTrades = await this.itemTradeServiceProxy.GetAllItemTradesAsync();

            // 1. Get all trades and filter
            var filteredTrades = allTrades.ItemTrades
                .Where(trade => (trade.SourceUserId == userId || trade.DestinationUserId == userId)
                         && trade.TradeStatus == TradeStatusEnum.Pending) // Fixed comparison to use the enum directly
                .ToList();

            var allUsersApi = (await this.userServiceProxy.GetUsersAsync()).Users;
            var allUsers = allUsersApi
                .Select(tradeUser =>
                {
                    var user = new User
                    {
                        UserId = tradeUser.UserId,
                        UserName = tradeUser.UserName,
                        Email = tradeUser.Email,
                        UserRole = (User.Role)tradeUser.Role,
                        WalletBalance = tradeUser.WalletBalance,
                        PointsBalance = tradeUser.PointsBalance,
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
                var sourceUser = allUsers.First(currentUser => currentUser.UserId == tradeDto.SourceUserId);
                var destinationUser = allUsers.First(currentUser => currentUser.UserId == tradeDto.DestinationUserId);

                var game = allGames.FirstOrDefault(currentGame => currentGame.GameId == tradeDto.GameOfTradeId);
                if (game == null)
                {
                    continue; // Skip if game not found
                }

                var itemTrade = new ItemTrade
                {
                    TradeId = tradeDto.TradeId,
                    SourceUser = sourceUser,
                    DestinationUser = destinationUser,
                    GameOfTrade = game,
                    TradeDescription = tradeDto.TradeDescription,
                    AcceptedBySourceUser = tradeDto.AcceptedBySourceUser,
                    AcceptedByDestinationUser = tradeDto.AcceptedByDestinationUser,
                };

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
                    .Where(tradeDetail => tradeDetail.TradeId == trade.TradeId);

                foreach (var detail in tradeDetailsForThisTrade)
                {
                    var itemResponse = await this.itemTradeServiceProxy.GetItemTradeByIdAsync(detail.TradeId);
                    var itemResponseFromItemProxy = await this.itemServiceProxy.GetItemByIdAsync(detail.ItemId);
                    var gameResponse = await this.gameServiceProxy.GetGameByIdAsync(itemResponse.GameOfTradeId);
                    var itemGame = GameMapper.MapToGame(gameResponse);

                    var item = new Item(itemResponseFromItemProxy.ItemName, itemGame, (float)itemResponseFromItemProxy.Price, itemResponseFromItemProxy.Description);
                    item.SetItemId(itemResponseFromItemProxy.ItemId);
                    item.SetIsListed(itemResponseFromItemProxy.IsListed);

                    if (detail.IsSourceUserItem)
                    {
                        trade.SourceUserItems.Add(item);
                    }
                    else
                    {
                        trade.DestinationUserItems.Add(item);
                    }
                }
            }

            foreach (var activeTrade in result)
            {
                System.Diagnostics.Debug.WriteLine($"Trade ID: {activeTrade.TradeId}, Source User: {activeTrade.SourceUser.UserName}, Destination User: {activeTrade.DestinationUser.UserName}, Game: {activeTrade.GameOfTrade}");
                System.Diagnostics.Debug.WriteLine(activeTrade.SourceUserItems);
                System.Diagnostics.Debug.WriteLine(activeTrade.DestinationUserItems);
            }

            return result;
        }

        public async Task CreateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.AddItemTradeAsync(trade);
            }
            catch (Exception tradeCreationException)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating trade: {tradeCreationException.Message}");
                throw;
            }
        }

        public async Task UpdateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeUpdateException)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating trade: {tradeUpdateException.Message}");
                throw;
            }
        }

        public async Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser)
        {
            try
            {
                if (isSourceUser)
                {
                    trade.AcceptBySourceUser();
                }
                else
                {
                    trade.AcceptByDestinationUser();
                }

                await this.UpdateItemTradeAsync(trade);

                // If both users have accepted, complete the trade
                if (trade.AcceptedByDestinationUser)
                {
                    await this.CompleteTradeAsync(trade);
                }
            }
            catch (Exception tradeAcceptionException)
            {
                System.Diagnostics.Debug.WriteLine($"Error accepting trade: {tradeAcceptionException.Message}");
                throw;
            }
        }

        public async Task CompleteTradeAsync(ItemTrade trade)
        {
            try
            {
                // Transfer source user items to destination user
                foreach (var item in trade.SourceUserItems)
                {
                    await this.TransferItemAsync(item.ItemId, trade.SourceUser.UserId, trade.DestinationUser.UserId, trade.GameOfTrade.GameId);
                }

                // Transfer destination user items to source user
                foreach (var item in trade.DestinationUserItems)
                {
                    await this.TransferItemAsync(item.ItemId, trade.DestinationUser.UserId, trade.SourceUser.UserId, trade.GameOfTrade.GameId);
                }

                trade.MarkTradeAsCompleted();
                await this.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeCompletingException)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing trade: {tradeCompletingException.Message}");
                throw;
            }
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            var inventoryResponse = await this.userInventoryServiceProxy.GetUserInventoryAsync(userId);
            var allGamesResponse = await this.gameServiceProxy.GetGamesAsync(new GetGamesRequest());
            var result = new List<Item>();
            var allGames = allGamesResponse.Select(GameMapper.MapToGame).ToList();
            foreach (var inventoryItem in inventoryResponse.Items)
            {
                var matchingGame = allGames.FirstOrDefault(game =>

                string.Equals(game.GameTitle, inventoryItem.GameName, StringComparison.OrdinalIgnoreCase));
                var item = new Item(inventoryItem.ItemName, matchingGame, (float)inventoryItem.Price, inventoryItem.Description);
                item.SetItemId(inventoryItem.ItemId);
                item.SetIsListed(inventoryItem.IsListed);
                result.Add(item);
            }

            return result;
        }
    }
}
