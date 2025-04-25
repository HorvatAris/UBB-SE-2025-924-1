using Moq;
using Steampunks.Services.TradeService;
using Steampunks.Repository.Trade;
using Steampunks.Domain.Entities;

namespace Steampunks.Tests.Trade
{
    [TestFixture]
    public class TradeServiceTests
    {
        private Mock<ITradeRepository> mockTradeRepository;
        private TradeService tradeService;

        [SetUp]
        public void Setup()
        {
            mockTradeRepository = new Mock<ITradeRepository>();
            tradeService = new TradeService(mockTradeRepository.Object);
        }

        [Test]
        public async Task GetActiveTradesAsync_ShouldReturnActiveTradesFromRepository()
        {
            // Arrange
            int userId = 1;
            var expectedTrades = new List<ItemTrade>();
            mockTradeRepository.Setup(repo => repo.GetActiveTradesAsync(userId)).ReturnsAsync(expectedTrades);

            // Act
            var result = await tradeService.GetActiveTradesAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedTrades));
            mockTradeRepository.Verify(repo => repo.GetActiveTradesAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetTradeHistoryAsync_ShouldReturnTradeHistoryFromRepository()
        {
            // Arrange
            int userId = 1;
            var expectedHistory = new List<ItemTrade>();
            mockTradeRepository.Setup(repo => repo.GetTradeHistoryAsync(userId)).ReturnsAsync(expectedHistory);

            // Act
            var result = await tradeService.GetTradeHistoryAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedHistory));
            mockTradeRepository.Verify(repo => repo.GetTradeHistoryAsync(userId), Times.Once);
        }

        [Test]
        public async Task CreateTradeAsync_ShouldCallRepositoryMethod()
        {
            // Arrange
            var trade = new ItemTrade(new User("source"), new User("dest"), new Game("game", 10.0f, "genre", "desc"), "trade desc");
            mockTradeRepository.Setup(repo => repo.AddItemTradeAsync(trade)).Returns(Task.CompletedTask);

            // Act
            await tradeService.CreateTradeAsync(trade);

            // Assert
            mockTradeRepository.Verify(repo => repo.AddItemTradeAsync(trade), Times.Once);
        }

        [Test]
        public async Task UpdateTradeAsync_ShouldCallRepositoryMethod()
        {
            // Arrange
            var trade = new ItemTrade(new User("source"), new User("dest"), new Game("game", 10.0f, "genre", "desc"), "trade desc");
            mockTradeRepository.Setup(repo => repo.UpdateItemTradeAsync(trade)).Returns(Task.CompletedTask);

            // Act
            await tradeService.UpdateTradeAsync(trade);

            // Assert
            mockTradeRepository.Verify(repo => repo.UpdateItemTradeAsync(trade), Times.Once);
        }

        [Test]
        public async Task AcceptTradeAsync_WhenSourceUserAccepts_ShouldUpdateTradeCorrectly()
        {
            // Arrange
            var trade = new ItemTrade(new User("source"), new User("dest"), new Game("game", 10.0f, "genre", "desc"), "trade desc");
            mockTradeRepository.Setup(repo => repo.UpdateItemTradeAsync(It.IsAny<ItemTrade>())).Returns(Task.CompletedTask);

            // Act
            await tradeService.AcceptTradeAsync(trade, true);

            // Assert
            Assert.That(trade.AcceptedBySourceUser, Is.True);
            Assert.That(trade.AcceptedByDestinationUser, Is.False);
            mockTradeRepository.Verify(repo => repo.UpdateItemTradeAsync(trade), Times.Once);
        }

        [Test]
        public async Task AcceptTradeAsync_WhenDestinationUserAccepts_ShouldUpdateTradeCorrectly()
        {
            // Arrange
            var trade = new ItemTrade(new User("source"), new User("dest"), new Game("game", 10.0f, "genre", "desc"), "trade desc");
            mockTradeRepository.Setup(repo => repo.UpdateItemTradeAsync(It.IsAny<ItemTrade>())).Returns(Task.CompletedTask);

            // Act
            await tradeService.AcceptTradeAsync(trade, false);

            // Assert
            Assert.That(trade.AcceptedBySourceUser, Is.True);
            Assert.That(trade.AcceptedByDestinationUser, Is.True);
            mockTradeRepository.Verify(repo => repo.UpdateItemTradeAsync(trade), Times.Exactly(2)); //one for updating Accepted by destination and one for CompleteTrade

        }

        [Test]
        public async Task AcceptTradeAsync_WhenBothUsersAccept_ShouldCompleteTrade()
        {
            // Arrange
            var sourceUser = new User("source");
            sourceUser.SetUserId(1);
            var destUser = new User("dest");
            destUser.SetUserId(2);
            var game = new Game("game", 10.0f, "genre", "desc");
            var trade = new ItemTrade(sourceUser, destUser, game, "trade desc");

            var sourceItem = new Item("sourceItem", game, 10.0f, "desc");
            sourceItem.SetItemId(1);
            var destItem = new Item("destItem", game, 10.0f, "desc");
            destItem.SetItemId(2);

            trade.AddSourceUserItem(sourceItem);
            trade.AddDestinationUserItem(destItem);

            trade.AcceptBySourceUser();

            mockTradeRepository.Setup(repo => repo.UpdateItemTradeAsync(It.IsAny<ItemTrade>())).Returns(Task.CompletedTask);
            mockTradeRepository.Setup(repo => repo.TransferItemAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            await tradeService.AcceptTradeAsync(trade, false);

            // Assert
            Assert.That(trade.AcceptedBySourceUser, Is.True);
            Assert.That(trade.AcceptedByDestinationUser, Is.True);
            Assert.That(trade.TradeStatus, Is.EqualTo("Completed"));

            mockTradeRepository.Verify(repo => repo.TransferItemAsync(sourceItem.ItemId, sourceUser.UserId, destUser.UserId), Times.Once);
            mockTradeRepository.Verify(repo => repo.TransferItemAsync(destItem.ItemId, destUser.UserId, sourceUser.UserId), Times.Once);
            mockTradeRepository.Verify(repo => repo.UpdateItemTradeAsync(trade), Times.AtLeastOnce);
        }

        [Test]
        public async Task DeclineTradeAsync_ShouldUpdateTradeStatusAndCallRepository()
        {
            // Arrange
            var trade = new ItemTrade(new User("source"), new User("dest"), new Game("game", 10.0f, "genre", "desc"), "trade desc");
            mockTradeRepository.Setup(repo => repo.UpdateItemTradeAsync(trade)).Returns(Task.CompletedTask);

            // Act
            await tradeService.DeclineTradeAsync(trade);

            // Assert
            Assert.That(trade.TradeStatus, Is.EqualTo("Declined"));
            mockTradeRepository.Verify(repo => repo.UpdateItemTradeAsync(trade), Times.Once);
        }

        [Test]
        public async Task GetCurrentUserAsync_ShouldReturnUserFromRepository()
        {
            // Arrange
            var expectedUser = new User("testUser");
            mockTradeRepository.Setup(repo => repo.GetCurrentUserAsync()).ReturnsAsync(expectedUser);

            // Act
            var result = await tradeService.GetCurrentUserAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedUser));
            mockTradeRepository.Verify(repo => repo.GetCurrentUserAsync(), Times.Once);
        }

        [Test]
        public async Task GetUserInventoryAsync_ShouldReturnInventoryFromRepository()
        {
            // Arrange
            int userId = 1;
            var expectedInventory = new List<Item>();
            mockTradeRepository.Setup(repo => repo.GetUserInventoryAsync(userId)).ReturnsAsync(expectedInventory);

            // Act
            var result = await tradeService.GetUserInventoryAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedInventory));
            mockTradeRepository.Verify(repo => repo.GetUserInventoryAsync(userId), Times.Once);
        }

        [Test]
        public void CompleteTrade_ValidTrade_UpdatesStatusAndCallsRepository()
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

            mockTradeRepository.Setup(x => x.UpdateItemTradeAsync(It.IsAny<ItemTrade>()))
                .Returns(Task.CompletedTask);

            // Act
            tradeService.CompleteTrade(trade);

            // Assert
            Assert.That(trade.TradeStatus, Is.EqualTo("Completed"));
            mockTradeRepository.Verify(x => x.UpdateItemTradeAsync(trade), Times.Once);
        }
    }
}