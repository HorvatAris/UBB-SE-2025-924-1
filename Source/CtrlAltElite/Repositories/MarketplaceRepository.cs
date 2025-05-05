// <copyright file="MarketplaceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.Repositories.Interfaces;
    using SteamStore.Data;

    public class MarketplaceRepository : IMarketplaceRepository
    {
        private readonly IDataLink dataLink;
        private User user;

        public MarketplaceRepository(IDataLink data, User user)
        {
            this.dataLink = data;
            this.user = user;
        }

        public async Task<bool> BuyItemAsync(Item item, User currentUser)
        {
            try
            {
                // Start transaction
                this.dataLink.OpenConnection();
                using (var transaction = this.dataLink.GetConnection().BeginTransaction())
                {
                    try
                    {
                        int currentOwnerID = await this.GetCurrentOwnerIDAsync(item, transaction);

                        await this.RemoveItemFromUserInventoryAsync(item, transaction);
                        Debug.WriteLine("Is item listes" + item.IsListed);

                        await this.AddItemToUserInventoryAsync(item, currentUser, transaction);

                        await this.UpdateItemListedStatusAsync(item, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception buyItemTransactionException)
                    {
                        Debug.WriteLine($"Error in BuyItem transaction: {buyItemTransactionException.Message}");
                        Debug.WriteLine($"Stack trace: {buyItemTransactionException.StackTrace}");
                        transaction.Rollback();
                        throw new InvalidOperationException("Failed to complete purchase. Please try again.");
                    }
                }
            }
            finally
            {
                this.dataLink.CloseConnection();
            }
        }

        public async Task<int> GetCurrentOwnerIDAsync(Item item, SqlTransaction transaction)
        {
            int currentOwnerId;

            using (var command = new SqlCommand(@"SELECT UserId FROM UserInventory WHERE ItemId = @ItemId", this.dataLink.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                try
                {
                    System.Diagnostics.Debug.WriteLine(item.ItemId);

                    object? currentOwnerIDResult = await command.ExecuteScalarAsync();
                    currentOwnerId = (int)currentOwnerIDResult;
                    return currentOwnerId;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception);
                    throw new Exception(exception.Message);
                }
            }
        }

        public async Task RemoveItemFromUserInventoryAsync(Item item, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"DELETE FROM UserInventory WHERE ItemId = @ItemId", this.dataLink.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddItemToUserInventoryAsync(Item item, User user, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"INSERT INTO UserInventory (UserId, GameId, ItemId) VALUES (@UserId, @GameId, @ItemId)", this.dataLink.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@UserId", user.UserId);
                command.Parameters.AddWithValue("@GameId", item.Game.GameId);
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateItemListedStatusAsync(Item item, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"UPDATE Items SET IsListed = 0 WHERE ItemId = @ItemId", this.dataLink.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Item>> GetAllListedItemsAsync()
        {
            var items = new List<Item>();
            using (var command = new SqlCommand(
                @"SELECT i.ItemId, i.ItemName, i.Description, i.Price, i.IsListed,i.ImagePath,
                g.game_id, g.name as GameTitle, g.price as GamePrice, g.description as GameDescription
                FROM Items i
                JOIN Games g ON i.CorrespondingGameId = g.game_id
                WHERE i.IsListed = 1",
                this.dataLink.GetConnection()))
            {
                try
                {
                    this.dataLink.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var game = new Game
                            {
                                GameId = reader.GetInt32(reader.GetOrdinal("game_id")),
                                GameDescription = reader.GetString(reader.GetOrdinal("GameDescription")),
                                GameTitle = reader.GetString(reader.GetOrdinal("GameTitle")),
                                Price = (decimal)reader.GetDecimal(reader.GetOrdinal("GamePrice")),
                            };
                            var item = new Item(
                               reader.GetString(reader.GetOrdinal("ItemName")),
                               game,
                               (float)reader.GetDouble(reader.GetOrdinal("Price")),
                               reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));

                            // Set image path based on game and item ID
                            string imagePath = reader.GetString(reader.GetOrdinal("ImagePath"));
                            item.SetImagePath(imagePath);
                            System.Diagnostics.Debug.WriteLine($"Added listing item {item.ItemId} with image path: {imagePath}");
                            items.Add(item);
                        }
                    }
                }
                finally
                {
                    this.dataLink.CloseConnection();
                }
            }

            return items;
        }

        public async Task<List<Item>> GetListedItemsByGameAsync(Game game)
        {
            var items = new List<Item>();
            const string query = @"
                    SELECT i.ItemId, i.ItemName, i.Price, i.Description, i.IsListed,
                        g.game_id, g.name as GameTitle, g.Genre, g.description as GameDescription,
                        g.price as GamePrice, g.status as GameStatus
                    FROM Items i
                    JOIN UserInventory ui ON i.ItemId = ui.ItemId
                    JOIN Games g ON ui.GameId = g.game_id
                    WHERE g.game_id = @GameId AND i.IsListed = 1";

            try
            {
                using (var command = new SqlCommand(query, this.dataLink.GetConnection()))
                {
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    this.dataLink.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var gameObject = new Game
                            {
                                GameTitle = reader.GetString(reader.GetOrdinal("GameTitle")),
                                Price = (decimal)reader.GetDecimal(reader.GetOrdinal("GamePrice")),
                                GameDescription = reader.GetString(reader.GetOrdinal("GameDescription")),
                                GameId = reader.GetInt32(reader.GetOrdinal("game_id")),
                                Status = reader.GetString(reader.GetOrdinal("GameStatus")),
                            };
                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                gameObject,
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

        public async Task MakeItemListableAsync(Game game, Item item)
        {
            // AddListingAsync(Item item)
            using (var command = new SqlCommand(
                @"UPDATE Items 
                SET IsListed = 1, Price = @Price
                WHERE ItemId = @ItemId", this.dataLink.GetConnection()))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@Price", item.Price);

                try
                {
                    this.dataLink.OpenConnection();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    this.dataLink.CloseConnection();
                }
            }
        }

        public async Task MakeItemNotListableAsync(Game game, Item item)
        {
            // await this.databaseConnector.RemoveListingAsync(item);
            using (var command = new SqlCommand(
                    @"UPDATE Items 
                    SET IsListed = 0
                    WHERE ItemId = @ItemId", this.dataLink.GetConnection()))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);

                try
                {
                    this.dataLink.CloseConnection();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    this.dataLink.CloseConnection();
                }
            }
        }

        public async Task UpdateItemPriceAsync(Game game, Item item)
        {
            using (var command = new SqlCommand(
                @"UPDATE Items 
                SET Price = @Price
                WHERE ItemId = @ItemId", this.dataLink.GetConnection()))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@Price", item.Price);

                try
                {
                    this.dataLink.OpenConnection();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    this.dataLink.CloseConnection();
                }
            }
        }

        public User? GetCurrentUser()
        {
            return this.user;
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using (var command = new SqlCommand("SELECT user_id as UserId, username as UserName FROM Users", this.dataLink.GetConnection()))
            {
                try
                {
                    this.dataLink.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            };

                            users.Add(user);
                        }

                        if (users.Count == 0)
                        {
                            // If no users exist, create test users

                            // NU AR TREBUI SA INTRE DELOC AICI
                            this.dataLink.CloseConnection();

                            // this.InsertTestUsers();
                            // return await this.GetAllUsersAsync(); // Recursive call to get the newly inserted users
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