using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steampunks.Domain.Entities;
using Steampunks.Repository.GameRepo;
using Steampunks.Services;

namespace Steampunks_Tests
{
    [TestFixture]
    public class GameServiceTests
    {
        private Mock<IGameRepository> mockRepository;
        private GameService service;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new Mock<IGameRepository>();
            service = new GameService(mockRepository.Object);
        }

        [Test]
        public async Task GetAllGamesAsync_ReturnsGamesFromRepository()
        {
            Game game1 = new Game("Test Game1", 10.0f, "RPG", "Desc");
            Game game2 = new Game("Test Game2", 11.0f, "RfG", "Asc");
            var games = new List<Game>
        {
           game1, game2
        };
            mockRepository.Setup(r => r.GetGamesAsync()).ReturnsAsync(games);

            var result = await service.GetAllGamesAsync();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].GameTitle, Is.EqualTo("Test Game1"));
            mockRepository.Verify(r => r.GetGamesAsync(), Times.Once);
        }

        [Test] 
        public async Task GetGameByIdAsync_ValidId_ReturnsGame()
        {
            Game game = new Game("Test Game1", 10.0f, "RPG", "Desc");
            mockRepository.Setup(r => r.GetGameByIdAsync(42)).ReturnsAsync(game);

            var result = await service.GetGameByIdAsync(42);

            Assert.IsNotNull(result);
            Assert.That(result.GameTitle, Is.EqualTo("Test Game1"));
            mockRepository.Verify(r => r.GetGameByIdAsync(42), Times.Once);
        }

        [Test]
        public async Task GetGameByIdAsync_InvalidId_ReturnsNull()
        {
            mockRepository.Setup(r => r.GetGameByIdAsync(999)).ReturnsAsync((Game)null);

            var result = await service.GetGameByIdAsync(999);

            Assert.IsNull(result);
            mockRepository.Verify(r => r.GetGameByIdAsync(999), Times.Once);
        }

        [Test]
        public async Task UpdateGameAsync_ValidGame_ReturnsTrue()
        {
            Game game = new Game("Test Game1", 10.0f, "RPG", "Desc");
            mockRepository.Setup(r => r.UpdateGameAsync(game)).ReturnsAsync(true);

            var result = await service.UpdateGameAsync(game);

            Assert.IsTrue(result);
            mockRepository.Verify(r => r.UpdateGameAsync(game), Times.Once);
        }

        [Test]
        public async Task UpdateGameAsync_UpdateFails_ReturnsFalse()
        {
            Game game = new Game("Test Game1", 10.0f, "RPG", "Desc");
            mockRepository.Setup(r => r.UpdateGameAsync(game)).ReturnsAsync(false);

            var result = await service.UpdateGameAsync(game);

            Assert.IsFalse(result);
            mockRepository.Verify(r => r.UpdateGameAsync(game), Times.Once);
        }
    }
}
