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
        public async Task GetItemTradeDetailsAsync_WhenCalled_ReturnsNonNullResult()
        {
            var result = await _repository.GetItemTradeDetailsAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetItemTradeDetailsAsync_WhenCalled_ReturnsExpectedNumberOfItemTradeDetails()
        {
            const int expectedItemTradeDetailCount = 2;

            var result = await _repository.GetItemTradeDetailsAsync();

            Assert.NotNull(result.ItemTradeDetails);
            Assert.Equal(expectedItemTradeDetailCount, result.ItemTradeDetails.Count);
        }

        [Fact]
        public async Task GetItemTradeDetailAsync_WithValidTradeIdAndItemId_ReturnsNonNullDetail()
        {
            var result = await _repository.GetItemTradeDetailAsync(1, 1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetItemTradeDetailAsync_WithValidTradeIdAndItemId_ReturnsItemMarkedAsSourceUserItem()
        {
            var result = await _repository.GetItemTradeDetailAsync(1, 1);

            Assert.True(result.IsSourceUserItem);
        }


        [Fact]
        public async Task GetItemTradeDetailAsync_WithValidTradeIdAndItemId_ReturnsCorrectItemId()
        {
            var itemId = 1;
            var result = await _repository.GetItemTradeDetailAsync(1, itemId);

            Assert.Equal(itemId, result.ItemId);
        }


        [Fact]
        public async Task GetItemTradeDetailAsync_WithValidTradeIdAndItemId_ReturnsCorrectTradeId()
        {
            var tradeId = 1;
            var result = await _repository.GetItemTradeDetailAsync(tradeId, 1);

            Assert.Equal(tradeId, result.TradeId);
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
        public async Task CreateItemTradeDetailAsync_WithValidRequest_ReturnsNonNullItemTradeDetail()
        {
            var request = new CreateItemTradeDetailRequest
            {
                TradeId = 3,
                ItemId = 3,
                IsSourceUserItem = true
            };

            var result = await _repository.CreateItemTradeDetailAsync(request);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateItemTradeDetailAsync_WithValidRequest_SetsCorrectTradeId()
        {
            var request = new CreateItemTradeDetailRequest
            {
                TradeId = 3,
                ItemId = 3,
                IsSourceUserItem = true
            };

            var result = await _repository.CreateItemTradeDetailAsync(request);

            Assert.Equal(request.TradeId, result.TradeId);
        }

        [Fact]
        public async Task CreateItemTradeDetailAsync_WithValidRequest_SetsCorrectItemId()
        {
            var request = new CreateItemTradeDetailRequest
            {
                TradeId = 3,
                ItemId = 3,
                IsSourceUserItem = true
            };

            var result = await _repository.CreateItemTradeDetailAsync(request);

            Assert.Equal(request.ItemId, result.ItemId);
        }

        [Fact]
        public async Task DeleteItemTradeDetailAsync_WithValidTradeIdAndItemID_RemovesTradeDetails()
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
