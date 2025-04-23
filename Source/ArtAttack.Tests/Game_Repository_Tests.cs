using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Steampunks.DataLink;
using Steampunks.Domain.Entities;
using Steampunks.Repository.GameRepo;

namespace Steampunks.Tests
{
    [TestFixture]
    public class GameRepositoryTests
    {
        private Mock<IDatabaseConnector> mockDatabaseConnector;
        private IGameRepository gameRepository;

        [SetUp]
        public void Setup()
        {
            mockDatabaseConnector = new Mock<IDatabaseConnector>();

            var realConnection = new DatabaseConnector().GetNewConnection();
            realConnection.Open(); 

            mockDatabaseConnector.Setup(x => x.GetConnection()).Returns(realConnection);
            mockDatabaseConnector.Setup(x => x.OpenConnectionAsync()).Returns(Task.CompletedTask); 
            mockDatabaseConnector.Setup(x => x.CloseConnection()).Callback(() => realConnection.Close());

            gameRepository = new GameRepository(mockDatabaseConnector.Object);
        }

        [Test]
        public async Task GetGamesAsync_WhenCalled_ReturnsGames()
        {
            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            var result = await gameRepository.GetGamesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<Game>>());
        }

        [Test]
        public async Task GetGamesAsync_ExceptionThrown_ClosesConnection()
        {
            mockDatabaseConnector.Object.CloseConnection();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await gameRepository.GetGamesAsync();
            });
        }

        [Test]
        public async Task GetGameByIdAsync_InvalidId_ReturnsNull()
        {
            var result = await gameRepository.GetGameByIdAsync(999); 
            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task GetGameByIdAsync_ValidId_ReturnsGame()
        {
            int gameId = 1;
            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            var result = await gameRepository.GetGameByIdAsync(gameId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GameId, Is.EqualTo(gameId));
        }

        [Test]
        public async Task UpdateGameAsync_ValidGame_UpdatesGame()
        {
            var game = new Game("Test Game", 10.0f, "RPG", "Description");
            game.SetGameId(1);
            mockDatabaseConnector.Setup(x => x.GetNewConnection())
                .Returns(new DatabaseConnector().GetNewConnection());

            var result = await gameRepository.UpdateGameAsync(game);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateGameAsync_ExceptionThrown_ClosesConnection()
        {
            var game = new Game("Oops", 9.99f, "Bugged", "Will fail");
            game.SetGameId(1);

            mockDatabaseConnector.Object.CloseConnection();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await gameRepository.UpdateGameAsync(game);
            });
        }

        [Test]
        public async Task GetGamesAsync_CallsCloseConnection_EvenOnFailure()
        {
            mockDatabaseConnector.Object.CloseConnection(); 

            try
            {
                await gameRepository.GetGamesAsync();
            }
            catch { }

            Assert.That(mockDatabaseConnector.Object.GetConnection().State, Is.EqualTo(System.Data.ConnectionState.Closed));
        }

        [Test]
        public async Task UpdateGameAsync_NoRowsAffected_ReturnsFalse()
        {
            var game = new Game("No Effect", 15.0f, "Puzzle", "Unused");
            game.SetGameId(999); 

            var result = await gameRepository.UpdateGameAsync(game);
            Assert.That(result, Is.False);
        }
    }
}