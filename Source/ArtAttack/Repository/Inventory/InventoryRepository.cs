// <copyright file="InventoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Repository.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;
    using Steampunks.Utils;

    /// <summary>
    /// Provides functionality to manage inventory items associated with users and games.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly string connectionString;
        private SqlConnection? connection;

        private IDatabaseConnector? dataBaseConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryRepository"/> class.
        /// </summary>
        /// <param name="databaseConnector"> Database connector. </param>
        public InventoryRepository(IDatabaseConnector databaseConnector)
        {
            this.dataBaseConnector = databaseConnector;
        }

        /// <summary>
        /// Retrieves a list of items from a specific game's inventory.
        /// </summary>
        /// <param name="game">The game whose inventory items are to be retrieved.</param>
        /// <returns>A list of <see cref="Item"/> objects associated with the specified game.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="game"/> is null.</exception>.
        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            var items = new List<Item>();
            const string query = @"
                SELECT 
                    i.ItemId,
                    i.ItemName,
                    i.Price,
                    i.Description,
                    i.IsListed
                FROM Items i
                JOIN UserInventory ui ON i.ItemId = ui.ItemId
                WHERE ui.GameId = @GameId AND ui.UserId = @UserId";

            try
            {
                var currentUser = this.dataBaseConnector.GetCurrentUser();
                if (currentUser == null)
                {
                    throw new InvalidOperationException("Current user not found.");
                }

                using (var command = new SqlCommand(query, this.dataBaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    command.Parameters.AddWithValue("@UserId", currentUser.UserId);
                    await this.dataBaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                game,
                                (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));
                            items.Add(item);
                        }
                    }
                }
            }
            catch (Exception getItemsException)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetUserInventory: {getItemsException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {getItemsException.StackTrace}");
                if (getItemsException.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {getItemsException.InnerException.Message}");
                }

                throw;
            }
            finally
            {
                this.dataBaseConnector.CloseConnection();
            }

            return items;
        }

        /// <summary>
        /// Get the inventory of a given User by it's userID Asynchronously.
        /// </summary>
        /// <param name="userId">The id of the user whose inventory items are to be retrieved.</param>
        /// <returns>A <see cref="Task"/> asynchronously resolving to a list of <see cref="Item"/> objects associated with the specified user.</returns>
        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            var items = new List<Item>();
            const string query = @"
                SELECT 
                    i.ItemId,
                    i.ItemName,
                    i.Price,
                    i.Description,
                    i.IsListed,
                    g.GameId,
                    g.Title as GameTitle,
                    g.Genre,
                    g.Description as GameDescription,
                    g.Price as GamePrice,
                    g.Status as GameStatus
                FROM Items i
                JOIN Games g ON i.CorrespondingGameId = g.GameId
                JOIN UserInventory ui ON i.ItemId = ui.ItemId AND g.GameId = ui.GameId
                WHERE ui.UserId = @UserId
                ORDER BY g.Title, i.Price";

            try
            {
                System.Diagnostics.Debug.WriteLine($"Executing GetUserInventory query for userId: {userId}");
                using (var command = new SqlCommand(query, this.dataBaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    await this.dataBaseConnector.OpenConnectionAsync();
                    System.Diagnostics.Debug.WriteLine("Connection opened successfully");

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        System.Diagnostics.Debug.WriteLine("Query executed successfully");
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            System.Diagnostics.Debug.WriteLine($"Found item: {reader.GetString(reader.GetOrdinal("ItemName"))}");
                            var game = new Game(
                                reader.GetString(reader.GetOrdinal("GameTitle")),
                                (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                reader.GetString(reader.GetOrdinal("Genre")),
                                reader.GetString(reader.GetOrdinal("GameDescription")));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));
                            game.SetStatus(reader.GetString(reader.GetOrdinal("GameStatus")));

                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                game,
                                (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));

                            // Set image path based on game and item name
                            string imagePath = this.dataBaseConnector.GetItemImagePath(item);
                            item.SetImagePath(imagePath);

                            items.Add(item);
                        }

                        System.Diagnostics.Debug.WriteLine($"Total items found: {items.Count}");
                    }
                }
            }
            catch (Exception getUserInventoryException)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetUserInventory: {getUserInventoryException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {getUserInventoryException.StackTrace}");
                if (getUserInventoryException.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {getUserInventoryException.InnerException.Message}");
                }

                throw;
            }
            finally
            {
                this.dataBaseConnector.CloseConnection();
            }

            return items;
        }

        /// <summary>
        /// Retrieves all inventory items associated with a specific user across all games.
        /// </summary>
        /// <param name="user">The user whose inventory is to be retrieved.</param>
        /// <returns>A list of all <see cref="Item"/> objects belonging to the specified user.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="user"/> is null.</exception>.
        public async Task<List<Item>> GetAllItemsFromInventoryAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var items = new List<Item>();
            const string query = @"
                SELECT 
                    i.ItemId,
                    i.ItemName,
                    i.Price,
                    i.Description,
                    i.IsListed,
                    g.GameId,
                    g.Title as GameTitle,
                    g.Genre,
                    g.Description as GameDescription,
                    g.Price as GamePrice,
                    g.Status as GameStatus
                FROM Items i
                JOIN UserInventory ui ON i.ItemId = ui.ItemId
                JOIN Games g ON ui.GameId = g.GameId
                WHERE ui.UserId = @UserId";

            try
            {
                using (var command = new SqlCommand(query, this.dataBaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    await this.dataBaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var game = new Game(
                                reader.GetString(reader.GetOrdinal("GameTitle")),
                                (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                reader.GetString(reader.GetOrdinal("Genre")),
                                reader.GetString(reader.GetOrdinal("GameDescription")));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));
                            game.SetStatus(reader.GetString(reader.GetOrdinal("GameStatus")));

                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                game,
                                (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));
                            items.Add(item);
                        }
                    }
                }
            }
            finally
            {
                this.dataBaseConnector.CloseConnection();
            }

            return items;
        }

        /// <summary>
        /// Adds an item to a specific game's inventory for a user.
        /// </summary>
        /// <param name="game">The game to which the item is to be added.</param>
        /// <param name="item">The item to be added.</param>
        /// <param name="user">The user who is adding the item to their inventory.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="game"/>, <paramref name="item"/>, or <paramref name="user"/> is null.
        /// </exception>
        /// <returns>AddInventoryItemAsync returns <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddItemToInventoryAsync(Game game, Item item, User user)
        {
            ArgumentNullException.ThrowIfNull(game);

            ArgumentNullException.ThrowIfNull(item);

            ArgumentNullException.ThrowIfNull(user);

            const string query = @"
                INSERT INTO UserInventory (UserId, GameId, ItemId)
                VALUES (@UserId, @GameId, @ItemId)";

            try
            {
                using (var command = new SqlCommand(query, this.dataBaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    command.Parameters.AddWithValue("@ItemId", item.ItemId);
                    await this.dataBaseConnector.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                this.dataBaseConnector.CloseConnection();
            }
        }

        /// <summary>
        /// Removes an item from a specific game's inventory for a user.
        /// </summary>
        /// <param name="game">The game from which the item is to be removed.</param>
        /// <param name="item">The item to be removed.</param>
        /// <param name="user">The user whose inventory the item is being removed from.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="game"/>, <paramref name="item"/>, or <paramref name="user"/> is null.
        /// </exception>
        /// <returns>RemoveInventoryItemAsync returns <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveItemFromInventoryAsync(Game game, Item item, User user)
        {
            ArgumentNullException.ThrowIfNull(game);

            ArgumentNullException.ThrowIfNull(item);

            ArgumentNullException.ThrowIfNull(user);

            const string query = @"
                DELETE FROM UserInventory 
                WHERE UserId = @UserId AND GameId = @GameId AND ItemId = @ItemId";

            try
            {
                using (var command = new SqlCommand(query, this.dataBaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    command.Parameters.AddWithValue("@ItemId", item.ItemId);
                    await this.dataBaseConnector.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                this.dataBaseConnector.CloseConnection();
            }
        }

        /// <summary>
        /// Asynchronously sells the specified item by updating its listed status.
        /// </summary>
        /// <param name="item">The item to be sold.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a value indicating whether the operation succeeded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
        public async Task<bool> SellItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                // Start transaction.
                await this.dataBaseConnector.OpenConnectionAsync().ConfigureAwait(false);
                using (var transaction = this.dataBaseConnector.GetConnection().BeginTransaction())
                {
                    try
                    {
                        // Update item's listed status.
                        using (var command = new SqlCommand(
                            @"
                            UPDATE Items 
                            SET IsListed = 1
                            WHERE ItemId = @ItemId", this.dataBaseConnector.GetConnection(),
                            transaction))
                        {
                            command.Parameters.AddWithValue("@ItemId", item.ItemId);
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Commit transaction asynchronously.
                        // (Assuming the underlying transaction supports asynchronous commit.)
                        await transaction.CommitAsync().ConfigureAwait(false);

                        return true;
                    }
                    catch (Exception transactionException)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in transaction: {transactionException.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {transactionException.StackTrace}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception sellingItemException)
            {
                System.Diagnostics.Debug.WriteLine($"Error selling item: {sellingItemException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {sellingItemException.StackTrace}");
                return false;
            }
            finally
            {
                this.dataBaseConnector.CloseConnection();
            }
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using (var command = new SqlCommand("SELECT UserId, Username FROM Users", this.dataBaseConnector.GetConnection()))
            {
                try
                {
                    await this.dataBaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var user = new User(reader.GetString(reader.GetOrdinal("Username")));
                            user.SetUserId(reader.GetInt32(reader.GetOrdinal("UserId")));
                            users.Add(user);
                        }
                    }
                }
                finally
                {
                    this.dataBaseConnector.CloseConnection();
                }
            }

            return users;
        }
    }
}