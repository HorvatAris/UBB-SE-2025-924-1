// <copyright file="InventoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using SteamStore.Data;
    using SteamStore.Models;
    using SteamStore.Repositories.Interfaces;

    public class InventoryRepository : IInventoryRepository
    {
        private readonly IDataLink dataLink;
        private User user;

        public InventoryRepository(IDataLink dataLink, User user)
        {
            this.dataLink = dataLink;
            this.user = user;
        }

        // done
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
                var currentUser = this.user;
                if (currentUser == null)
                {
                    throw new InvalidOperationException("Current user not found.");
                }

                using (var command = new SqlCommand(query, this.dataLink.GetConnection()))
                {
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    command.Parameters.AddWithValue("@UserId", currentUser.UserId);
                    this.dataLink.OpenConnection();
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
                this.dataLink.CloseConnection();
            }

            return items;
        }

        /// <summary>
        /// Get the inventory of a given User by it's userID Asynchronously.
        /// </summary>
        /// <param name="userId">The id of the user whose inventory items are to be retrieved.</param>
        /// <returns>A <see cref="Task"/> asynchronously resolving to a list of <see cref="Item"/> objects associated with the specified user.</returns>
        // done
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
                    i.ImagePath,   
                    g.game_id,
                    g.name as GameTitle,
                   
                    g.description as GameDescription,
                    g.price as GamePrice,
                    g.status as GameStatus
                FROM Items i
                JOIN Games g ON i.CorrespondingGameId = g.game_id
                JOIN UserInventory ui ON i.ItemId = ui.ItemId AND g.game_id = ui.GameId
                WHERE ui.UserId = @UserId
                ORDER BY g.name, i.Price";

            try
            {
                System.Diagnostics.Debug.WriteLine($"Executing GetUserInventory query for userId: {userId}");
                using (var command = new SqlCommand(query, this.dataLink.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    this.dataLink.OpenConnection();
                    System.Diagnostics.Debug.WriteLine("Connection opened successfully");

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        System.Diagnostics.Debug.WriteLine("Query executed successfully");
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            System.Diagnostics.Debug.WriteLine($"Found item: {reader.GetString(reader.GetOrdinal("ItemName"))}");

                            // var game = new Game(
                            //    reader.GetString(reader.GetOrdinal("GameTitle")),
                            //    (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                            //    reader.GetString(reader.GetOrdinal("Genre")),
                            //    reader.GetString(reader.GetOrdinal("GameDescription")));
                            var game = new Game
                            {
                                GameId = reader.GetInt32(reader.GetOrdinal("game_id")),
                                GameTitle = reader.GetString(reader.GetOrdinal("GameTitle")),

                                // Genre = reader.GetString(reader.GetOrdinal("Genre")),
                                GameDescription = reader.GetString(reader.GetOrdinal("GameDescription")),
                                Price = (decimal)reader.GetDecimal(reader.GetOrdinal("GamePrice")),
                                Status = reader.GetString(reader.GetOrdinal("GameStatus")),
                            };
                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                game,
                                (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));

                            // Set image path based on game and item name
                            string imagePath = reader.GetString(reader.GetOrdinal("ImagePath"));
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
                this.dataLink.CloseConnection();
            }

            return items;
        }

        /// <summary>
        /// Retrieves all inventory items associated with a specific user across all games.
        /// </summary>
        /// <param name="user">The user whose inventory is to be retrieved.</param>
        /// <returns>A list of all <see cref="Item"/> objects belonging to the specified user.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="user"/> is null.</exception>.
        /// nu o folosim
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
                    g.game_id,
                    g.name as GameTitle,
                    
                    g.description as GameDescription,
                    g.price as GamePrice,
                    g.status as GameStatus
                FROM Items i
                JOIN UserInventory ui ON i.ItemId = ui.ItemId
                JOIN Games g ON ui.GameId = g.game_id
                WHERE ui.UserId = @UserId";

            try
            {
                using (var command = new SqlCommand(query, this.dataLink.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    this.dataLink.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            // var game = new Game(
                            //    reader.GetString(reader.GetOrdinal("GameTitle")),
                            //    (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                            //    reader.GetString(reader.GetOrdinal("Genre")),
                            //    reader.GetString(reader.GetOrdinal("GameDescription")));
                            // game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));
                            // game.SetStatus(reader.GetString(reader.GetOrdinal("GameStatus")));
                            var game = new Game
                            {
                                GameId = reader.GetInt32(reader.GetOrdinal("game_id")),
                                GameTitle = reader.GetString(reader.GetOrdinal("GameTitle")),

                                // Genre = reader.GetString(reader.GetOrdinal("Genre")),
                                GameDescription = reader.GetString(reader.GetOrdinal("GameDescription")),
                                Price = (decimal)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                Status = reader.GetString(reader.GetOrdinal("GameStatus")),
                            };

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
                this.dataLink.CloseConnection();
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
                using (var command = new SqlCommand(query, this.dataLink.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    command.Parameters.AddWithValue("@ItemId", item.ItemId);
                    this.dataLink.OpenConnection();
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                this.dataLink.CloseConnection();
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
                using (var command = new SqlCommand(query, this.dataLink.GetConnection()))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    command.Parameters.AddWithValue("@ItemId", item.ItemId);
                    this.dataLink.OpenConnection();
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                this.dataLink.CloseConnection();
            }
        }

        public async Task<bool> SellItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                // await this.dataBaseConnector.OpenConnectionAsync().ConfigureAwait(false);
                this.dataLink.OpenConnection();

                // Start transaction.
                using (var transaction = this.dataLink.GetConnection().BeginTransaction())
                {
                    try
                    {
                        // Update item's listed status.
                        using (var command = new SqlCommand(
                            @"
                            UPDATE Items 
                            SET IsListed = 1
                            WHERE ItemId = @ItemId", this.dataLink.GetConnection(),
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
                this.dataLink.CloseConnection();
            }
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using (var command = new SqlCommand("SELECT user_id, UserName FROM Users", this.dataLink.GetConnection()))
            {
                try
                {
                    this.dataLink.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var user = new User
                            {
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            };

                            users.Add(user);
                        }
                    }
                }
                finally
                {
                    this.dataLink.CloseConnection();
                }
            }

            return users;
        }
    }
}