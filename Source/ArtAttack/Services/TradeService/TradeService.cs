// <copyright file="TradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services.TradeService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;
    using Steampunks.Repository.Trade;
    using Steampunks.Services.TradeService;

    /// <summary>
    /// Service for trading operations.
    /// </summary>
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository tradeRepository;

        public TradeService(ITradeRepository tradeRepository)
        {
            this.tradeRepository = tradeRepository;
        }

        /// <inheritdoc/> moved
        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            return await this.tradeRepository.GetActiveTradesAsync(userId);
        }

        /// <inheritdoc/> moved
        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            return await this.tradeRepository.GetTradeHistoryAsync(userId);
        }

        /// <inheritdoc/> moved 
        public async Task CreateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.tradeRepository.AddItemTradeAsync(trade);
            }
            catch (Exception tradeCreationException)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating trade: {tradeCreationException.Message}");
                throw;
            }
        }

        /// <inheritdoc/> done
        public async Task UpdateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.tradeRepository.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeUpdateException)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating trade: {tradeUpdateException.Message}");
                throw;
            }
        }

        /// <inheritdoc/>
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

                await this.tradeRepository.UpdateItemTradeAsync(trade);

                // If both users have accepted, complete the trade
                if (trade.AcceptedByDestinationUser)
                {
                    this.CompleteTrade(trade);
                }
            }
            catch (Exception tradeAcceptionException)
            {
                System.Diagnostics.Debug.WriteLine($"Error accepting trade: {tradeAcceptionException.Message}");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task DeclineTradeAsync(ItemTrade trade)
        {
            try
            {
                trade.DeclineTradeRequest();
                await this.tradeRepository.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeDeclineException)
            {
                System.Diagnostics.Debug.WriteLine($"Error declining trade: {tradeDeclineException.Message}");
                throw;
            }
        }

        /// <inheritdoc/> gata
        public async Task<User?> GetCurrentUserAsync()
        {
            return await this.tradeRepository.GetCurrentUserAsync();
        }

        /// <inheritdoc/> gata
        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            return await this.tradeRepository.GetUserInventoryAsync(userId);
        }

        //moved
        public async void CompleteTrade(ItemTrade trade)
        {
            try
            {
                // Transfer source user items to destination user
                foreach (var item in trade.SourceUserItems)
                {
                    await this.tradeRepository.TransferItemAsync(item.ItemId, trade.SourceUser.UserId, trade.DestinationUser.UserId);
                }

                // Transfer destination user items to source user
                foreach (var item in trade.DestinationUserItems)
                {
                    await this.tradeRepository.TransferItemAsync(item.ItemId, trade.DestinationUser.UserId, trade.SourceUser.UserId);
                }

                trade.MarkTradeAsCompleted();
                await this.tradeRepository.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeCompletingException)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing trade: {tradeCompletingException.Message}");
                throw;
            }
        }
    }
}