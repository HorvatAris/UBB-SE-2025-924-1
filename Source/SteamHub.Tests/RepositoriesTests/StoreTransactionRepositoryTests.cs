using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.Api.Models.StoreTransaction;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SteamHub.Tests.RepositoriesTests
{
    public class StoreTransactionRepositoryTests
    {
        private readonly DataContext _context;
        private readonly StoreTransactionRepository _repository;

        public StoreTransactionRepositoryTests()
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
            _repository = new StoreTransactionRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            _context.StoreTransactions.AddRange(
                new StoreTransaction { StoreTransactionId = 1, UserId = 10, GameId = 100, Date = new DateTime(2024, 1, 1), Amount = 59.99f, WithMoney = true },
                new StoreTransaction { StoreTransactionId = 2, UserId = 20, GameId = 200, Date = new DateTime(2024, 2, 2), Amount = 39.99f, WithMoney = false }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetStoreTransactionsAsync_WhenGenerallyCalled_ReturnsAllTransactions()
        {
            var result = await _repository.GetStoreTransactionsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.StoreTransactions.Count);
        }

        [Fact]
        public async Task GetStoreTransactionByIdAsync_WhencalledWithExistingId_ReturnsTransaction()
        {
            var result = await _repository.GetStoreTransactionByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(10, result.UserId);
        }

        [Fact]
        public async Task GetStoreTransactionByIdAsync_WhenCalledWithInvalidId_ReturnsNull()
        {
            int invalidIdValue = 999;
            var result = await _repository.GetStoreTransactionByIdAsync(invalidIdValue);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateStoreTransactionAsync_WhenCalledWithValidRequest_CreatesTransaction()
        {
            var request = new CreateStoreTransactionRequest
            {
                UserId = 30,
                GameId = 300,
                Date = DateTime.UtcNow,
                Amount = 49.99f,
                WithMoney = true
            };

            var response = await _repository.CreateStoreTransactionAsync(request);
            var created = await _context.StoreTransactions.FindAsync(response.StoreTransactionId);

            Assert.NotNull(created);
            Assert.Equal(30, created.UserId);
        }

        [Fact]
        public async Task UpdateStoreTransactionAsync_WhenCalledWithValidRequest_UpdatesTransaction()
        {
            var expectedAmount = 29.99f;
            var request = new UpdateStoreTransactionRequest
            {
                Date = new DateTime(2024, 3, 3),
                Amount = 29.99f,
                WithMoney = false
            };

            await _repository.UpdateStoreTransactionAsync(1, request);

            var updated = await _context.StoreTransactions.FindAsync(1);
            Assert.Equal(expectedAmount, updated.Amount);
            Assert.False(updated.WithMoney);
        }

        [Fact]
        public async Task UpdateStoreTransactionAsync_WhenCalledWithInvalidId_ThrowsException()
        {
            var transactionIdValue = 999;
            var request = new UpdateStoreTransactionRequest
            {
                Date = DateTime.UtcNow,
                Amount = 10.00f,
                WithMoney = true
            };

            await Assert.ThrowsAsync<Exception>(() => _repository.UpdateStoreTransactionAsync(transactionIdValue, request));
        }

        [Fact]
        public async Task DeleteStoreTransactionAsync_WhenCalledWithValidId_DeletesTransaction()
        {
            await _repository.DeleteStoreTransactionAsync(2);

            var deleted = await _context.StoreTransactions.FindAsync(2);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteStoreTransactionAsync_WhenCalledWithInvalidId_ThrowsException()
        {
            var invalidIdValue = 999;  
            await Assert.ThrowsAsync<Exception>(() => _repository.DeleteStoreTransactionAsync(invalidIdValue));
        }
    }
}
