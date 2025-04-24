// <copyright file="UserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Service class for accessing and managing Game data.
// </summary>
// Refactored by Team ArtAttack, 2025
namespace Steampunks.Repository.UserRepository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides data access methods for interacting with the user-related data in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// The database connector instance used to establish and manage SQL connections.
        /// </summary>
        private readonly IDatabaseConnector databaseConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository()
        {
            this.databaseConnector = new DatabaseConnector();
        }

        public UserRepository(IDatabaseConnector database)
        {
            this.databaseConnector = database;
        }

        /// <summary>
        /// Asynchronously retrieves all users from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="User"/> objects.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            const string query = "SELECT UserId, UserName FROM Users";

            try
            {
                using (var connection = this.databaseConnector.GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new User(reader.GetString(reader.GetOrdinal("UserName")));
                            user.SetUserId(reader.GetInt32(reader.GetOrdinal("UserId")));
                            users.Add(user);
                        }

                        if (users.Count == 0)
                        {
                            this.databaseConnector.CloseConnection();
                            await this.InsertTestUsersAsync();
                            return await Task.Run(() => this.GetAllUsersAsync());
                        }
                    }
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }

            return users;
        }

        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object,
        /// or <c>null</c> if no user with the specified ID is found.
        /// </returns>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            const string query = @"
        SELECT UserId, UserName, WalletBalance, PointBalance, IsDeveloper
        FROM Users
        WHERE UserId = @UserId";

            try
            {
                using (var connection = this.databaseConnector.GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = new User(reader.GetString(reader.GetOrdinal("UserName")));
                            user.SetUserId(reader.GetInt32(reader.GetOrdinal("UserId")));
                            return user;
                        }

                        return null;
                    }
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }
        }

        /// <summary>
        /// Asynchronously updates the specified user's data in the database.
        /// </summary>
        /// <param name="user">The user object containing updated information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result indicates whether the update was successful.
        /// </returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            const string query = @"
        UPDATE Users
        SET UserName = @UserName,
            WalletBalance = @WalletBalance,
            PointBalance = @PointBalance,
            IsDeveloper = @IsDeveloper
        WHERE UserId = @UserId";

            try
            {
                using (var connection = this.databaseConnector.GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@WalletBalance", user.WalletBalance);
                    command.Parameters.AddWithValue("@PointBalance", user.PointBalance);
                    command.Parameters.AddWithValue("@IsDeveloper", user.IsDeveloper);

                    await connection.OpenAsync();

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }
        }

        /// <summary>
        /// Asynchronously inserts test users into the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InsertTestUsersAsync()
        {
            var testUsers = new List<string>
            {
                "TestUser1",
                "TestUser2",
                "TestUser3",
            };

            const string query = @"INSERT INTO Users (UserName, WalletBalance, PointBalance, IsDeveloper) 
                  VALUES (@UserName, 1000, 100, 0)";

            try
            {
                using (var connection = this.databaseConnector.GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    foreach (var username in testUsers)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserName", username);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }
        }
    }
}
