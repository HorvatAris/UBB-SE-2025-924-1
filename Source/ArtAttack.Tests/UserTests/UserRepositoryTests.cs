using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Steampunks.DataLink;
using Steampunks.Domain.Entities;
using Steampunks.Repository.UserRepository;
using Microsoft.Data.SqlClient;
using Steampunks.Repository.GameRepo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Windows.ApplicationModel.Contacts;
using Windows.Security.Cryptography.Certificates;
using Steampunks.Utils;

namespace Steampunks.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<IDatabaseConnector> mockDatabaseConnector;
        private IUserRepository userRepository;
        private const string TestConnectionString = Configuration.CONNECTIONSTRINGNIKOLE;

        [SetUp]
        public void Setup()
        {
            mockDatabaseConnector = new Mock<IDatabaseConnector>();

            var realConnection = new DatabaseConnector().GetNewConnection();

            mockDatabaseConnector.Setup(x => x.GetConnection()).Returns(realConnection);
            //mockDatabaseConnector.Setup(x => x.OpenConnectionAsync()).Returns(Task.CompletedTask);
            mockDatabaseConnector.Setup(x => x.CloseConnection()).Callback(() => realConnection.Close());

            userRepository = new UserRepository(mockDatabaseConnector.Object);

        }
        private async Task ClearUsersTableAsync()
        {
            // Get a fresh connection directly (not through the mock)
            var connector = new DatabaseConnector();
            var connection = connector.GetConnection();

            try
            {
                await connection.OpenAsync();

                using var cmd = new SqlCommand("DELETE FROM Users WHERE Username = '!@#Special_User_测试_🚀'", connection);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                connection.Close();
            }
        }


        [Test]
        public async Task GetUserByIdAsync_InvalidId_ReturnsNull()
        {
            var result = await userRepository.GetUserByIdAsync(99999); 
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateUserAsync_NoUserWithId_ReturnsFalse()
        {
            var nonExistentUser = new User("Ghost");
            nonExistentUser.SetUserId(99999); // Ensure this doesn't exist
            var result = await userRepository.UpdateUserAsync(nonExistentUser);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetUserByIdAsync_SpecialCharactersHandled()
        {
            await ClearUsersTableAsync();
            var specialUser = new User("!@#Special_User_测试_🚀");

            var connection = mockDatabaseConnector.Object.GetConnection();
            await connection.OpenAsync();

            try
            {
                var insertCommand = new SqlCommand(
                    "INSERT INTO Users (Username, WalletBalance, PointBalance, IsDeveloper) " +
                    "OUTPUT INSERTED.UserId " +
                    "VALUES (@Username, 0, 0, 0)",
                    connection);
                insertCommand.Parameters.AddWithValue("@Username", specialUser.Username);

                // await insertCommand.ExecuteNonQueryAsync();
                var insertedId = (int)await insertCommand.ExecuteScalarAsync();
                specialUser.SetUserId(insertedId);
            }
            finally
            {
                connection.Close(); 
            }

            var fetched = await userRepository.GetUserByIdAsync(specialUser.UserId);

            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched.Username, Does.Contain("Special"));
        }

    }
}