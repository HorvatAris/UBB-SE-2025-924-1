using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.ItemTrade;
using ItemTrade = SteamHub.Api.Entities.ItemTrade;
using Xunit;

namespace SteamHub.Tests.Repositories
{
    public class ItemTradeRepositoryTests : IDisposable
    {
        private readonly DataContext _mockContext;
        private readonly ItemTradeRepository _repository;

        public ItemTradeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockContext = new DataContext(options, null);
            _repository = new ItemTradeRepository(_mockContext);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var trades = new List<ItemTrade>
            {
                new ItemTrade
                {
                    TradeId = 1,
                    SourceUserId = 1,
                    DestinationUserId = 2,
                    GameOfTradeId = 10,
                    TradeDescription = "Trade 1",
                    TradeDate = DateTime.UtcNow,
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 2,
                    SourceUserId = 2,
                    DestinationUserId = 3,
                    GameOfTradeId = 11,
                    TradeDescription = "Trade 2",
                    TradeDate = DateTime.UtcNow,
                    TradeStatus = TradeStatus.Completed,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = true
                }
            };

            _mockContext.ItemTrades.AddRange(trades);
            _mockContext.SaveChanges();
        }

        public void Dispose()
        {
            _mockContext.Dispose();
        }

        [Fact]
        public async Task GetItemTradesAsync_ReturnsAllTrades()
        {
            var result = await _repository.GetItemTradesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.ItemTrades.Count);
        }

        [Fact]
        public async Task GetItemTradeByIdAsync_WithValidId_ReturnsTrade()
        {
            var result = await _repository.GetItemTradeByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.TradeId);
        }

        [Fact]
        public async Task GetItemTradeByIdAsync_WithInvalidId_ReturnsNull()
        {
            var result = await _repository.GetItemTradeByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateItemTradeAsync_UpdatesFieldsSuccessfully()
        {
            var updateRequest = new UpdateItemTradeRequest
            {
                TradeDescription = "Updated description",
                TradeStatus = TradeStatusEnum.Declined,
                AcceptedBySourceUser = false,
                AcceptedByDestinationUser = true
            };

            await _repository.UpdateItemTradeAsync(1, updateRequest);
            var updatedTrade = await _mockContext.ItemTrades.FindAsync(1);

            Assert.Equal("Updated description", updatedTrade.TradeDescription);
            Assert.Equal(TradeStatus.Declined, updatedTrade.TradeStatus);
            Assert.False(updatedTrade.AcceptedBySourceUser);
            Assert.True(updatedTrade.AcceptedByDestinationUser);
        }

        [Fact]
        public async Task CreateItemTradeAsync_CreatesTradeSuccessfully()
        {
            _mockContext.ItemTrades.RemoveRange(_mockContext.ItemTrades);
            await _mockContext.SaveChangesAsync();

            var request = new CreateItemTradeRequest
            {
                SourceUserId = 4,
                DestinationUserId = 5,
                GameOfTradeId = 12,
                TradeDescription = "New trade",
                TradeStatus = TradeStatusEnum.Pending,
                AcceptedBySourceUser = false,
                AcceptedByDestinationUser = false
            };

            var result = await _repository.CreateItemTradeAsync(request);

            Assert.NotNull(result);
            var createdTrade = await _mockContext.ItemTrades.FindAsync(result.TradeId);
            Assert.Equal("New trade", createdTrade.TradeDescription);
        }

        [Fact]
        public async Task DeleteItemTradeAsync_RemovesTradeSuccessfully()
        {
            await _repository.DeleteItemTradeAsync(1);
            var deletedTrade = await _mockContext.ItemTrades.FindAsync(1);

            Assert.Null(deletedTrade);
        }

        [Fact]
        public async Task DeleteItemTradeAsync_WithInvalidId_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _repository.DeleteItemTradeAsync(999));
        }
    }
}
