using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Steampunks.DataLink;
using Steampunks.Domain.Entities;
using Steampunks.Repository.Trade;

namespace Steampunks.Tests.Trade
{
    [TestFixture]
    public class TradeRepositoryTests
    {
        private Mock<IDatabaseConnector> mockDatabaseConnector;
        private ITradeRepository tradeRepository;

        [SetUp]
        public void Setup()
        {
            mockDatabaseConnector = new Mock<IDatabaseConnector>();
            tradeRepository = new TradeRepository(mockDatabaseConnector.Object);
        }

        [Test]
        public async Task GetActiveTradesAsync_WhenCalled_ReturnsTrades()
        {
            // Arrange
            var userId = 1;
            var expectedTrades = new List<ItemTrade>
            {
                new ItemTrade(
                    new User("sourceUser"),
                    new User("destUser"),
                    new Game("Test Game", 10.0f, "Action", "GameDescription"),
                    "Test trade")
            };

            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            // Act
            var result = await tradeRepository.GetActiveTradesAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<ItemTrade>>());
        }

        [Test]
        public async Task GetTradeHistoryAsync_WhenCalled_ReturnsTrades()
        {
            // Arrange
            var userId = 1;
            var expectedTrades = new List<ItemTrade>
            {
                new ItemTrade(
                    new User("sourceUser"),
                    new User("destUser"),
                    new Game("Test Game", 10.0f, "Action", "GameDescription"),
                    "Test trade")
            };

            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            // Act
            var result = await tradeRepository.GetTradeHistoryAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<ItemTrade>>());
        }
        [Test]
        public async Task UpdateItemTradeAsync_ValidTrade_UpdatesTrade()
        {
            // Arrange
            var trade = new ItemTrade(
                new User("sourceUser"),
                new User("destUser"),
                new Game("Test Game", 10.0f, "Action", "GameDescription"),
                "Test trade");
            trade.SetTradeId(1);
            trade.AcceptBySourceUser();
            trade.AcceptByDestinationUser();

            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            // Act
            await tradeRepository.UpdateItemTradeAsync(trade);

            // Assert
            Assert.That(trade.TradeStatus, Is.EqualTo("Completed"));
            Assert.That(trade.AcceptedByDestinationUser, Is.True);
        }

        [Test]
        public async Task GetUserInventoryAsync_WhenCalled_ReturnsItems()
        {
            // Arrange
            var userId = 1;

            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            // Act
            var result = await tradeRepository.GetUserInventoryAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<Item>>());
        }

        [Test]
        public async Task GetCurrentUserAsync_WhenCalled_ReturnsUser()
        {
            // Arrange
            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            // Act
            var result = await tradeRepository.GetCurrentUserAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<User>());
        }
    }
}