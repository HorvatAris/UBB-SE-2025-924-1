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
using SteamHub.ApiContract.Models.ItemTradeDetails;

namespace SteamHub.Tests.RepositoriesTests
{
    public class ItemTradeDetailsTests
    {
        private readonly DataContext _context;
        private readonly ItemTradeDetailRepository _repository;

        public ItemTradeDetailsTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var inMemorySettings = new Dictionary<string, string>
            {
                { "SomeSetting", "SomeValue" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _context = new DataContext(options, configuration);
            _repository = new ItemTradeDetailRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var itemTradeDetailIsSourceUser = new ItemTradeDetail
            {
                TradeId = 1,
                ItemId = 1,
                IsSourceUserItem = true
            };

            _context.ItemTradeDetails.Add(itemTradeDetailIsSourceUser);

            var itemTradeDetailNotSourceUser = new ItemTradeDetail
            {
                TradeId = 2,
                ItemId = 2,
                IsSourceUserItem = false
            };
            _context.ItemTradeDetails.Add(itemTradeDetailNotSourceUser);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetItemTradeDetailsAsync_Always_ReturnAllItemTradeDetails()
        {
            const int expectedItemTradeDetailCount = 2;
            var result = await _repository.GetItemTradeDetailsAsync();

            Assert.NotNull(result);
            Assert.NotNull(result.ItemTradeDetails);

            int actualItemTradeDetailCount = result.ItemTradeDetails.Count;

            Assert.Equal(expectedItemTradeDetailCount, actualItemTradeDetailCount);
        }

        [Fact]
        public async Task GetItemTradeDetailAsync_ValidTradeIdAndItemId_ReturnsItemTradeDetail()
        {
            var tradeId = 1;
            var itemId = 1;
            var result = await _repository.GetItemTradeDetailAsync(tradeId, itemId);
            Assert.NotNull(result);
            Assert.Equal(tradeId, result.TradeId);
            Assert.Equal(itemId, result.ItemId);
            Assert.True(result.IsSourceUserItem);
        }

        [Fact]
        public async Task GetItemTradeDetailAsync_InvalidTradeIdAndItemId_ReturnsNull()
        {
            var tradeId = 999;
            var itemId = 999;
            var result = await _repository.GetItemTradeDetailAsync(tradeId, itemId);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateItemTradeDetailAsync_ValidRequest_ReturnsCreatedItemTradeDetail()
        {
            var request = new CreateItemTradeDetailRequest
            {
                TradeId = 3,
                ItemId = 3,
                IsSourceUserItem = true
            };
            var result = await _repository.CreateItemTradeDetailAsync(request);
            Assert.NotNull(result);
            Assert.Equal(request.TradeId, result.TradeId);
            Assert.Equal(request.ItemId, result.ItemId);
        }

        [Fact]
        public async Task DeleteItemTradeDetailAsync_ValidTradeIdAndItemID_RemovesTradeDetails()
        {
            var tradeId = 1;
            var itemId = 1;
            await _repository.DeleteItemTradeDetailAsync(tradeId, itemId);
            var deletedTradeDetail = await _repository.GetItemTradeDetailAsync(tradeId, itemId);
            Assert.Null(deletedTradeDetail);
        }

        [Fact]
        public async Task DeleteItemTradeDetailAsync_InvalidTradeIdAndItemID_DoesNotRemoveAnyTradeDetails()
        {
            var tradeId = 999;
            var itemId = 999;
            string expectedException = "ItemTradeDetail not found";
            var actualException = await Record.ExceptionAsync(() => _repository.DeleteItemTradeDetailAsync(tradeId, itemId));
            Assert.Contains(expectedException, actualException.ToString());
        }
    }
}
