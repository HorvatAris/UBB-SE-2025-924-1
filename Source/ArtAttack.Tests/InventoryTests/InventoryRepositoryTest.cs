using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using NUnit.Framework;
using Steampunks.Repository.Inventory;
using Steampunks.Domain.Entities;
using Steampunks.DataLink;
using Steampunks.Utils;

namespace Steampunks.Repository.InventoryTests
{
    public class StubDatabaseConnector : IDatabaseConnector
    {
        private readonly string connectionString;
        private SqlConnection? connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnector"/> class using the connection string defined in the configuration.
        /// </summary>
        public StubDatabaseConnector()
        {
            // Local MSSQL connection string
            this.connectionString = Configuration.TESTCONNECTIONSTRINGSONIA;
        }

        /// <summary>
        /// Gets the current SQL connection if open or creates and returns a new one if it's null or closed.
        /// </summary>
        /// <returns>The current or newly initialized <see cref="SqlConnection"/>.</returns>
        public SqlConnection GetConnection()
        {
            if (this.connection == null || this.connection.State == ConnectionState.Closed)
            {
                this.connection = new SqlConnection(this.connectionString);
            }

            return this.connection;
        }

        /// <summary>
        /// Always returns a new instance of <see cref="SqlConnection"/>.
        /// </summary>
        /// <returns>A new <see cref="SqlConnection"/> object.</returns>
        public SqlConnection GetNewConnection()
        {
            return new SqlConnection(this.connectionString);
        }

        /// <summary>
        /// Opens the connection if it is not already open.
        /// </summary>
        public void OpenConnection()
        {
            if (this.connection?.State != ConnectionState.Open)
            {
                this.connection?.Open();
            }
        }

        /// <summary>
        /// Asynchronously opens the connection if it is not already open.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OpenConnectionAsync()
        {
            if (this.connection == null)
            {
                this.connection = new SqlConnection(this.connectionString);
            }

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }
        }

        /// <summary>
        /// Closes the connection if it is not already closed.
        /// </summary>
        public void CloseConnection()
        {
            if (this.connection?.State != ConnectionState.Closed)
            {
                this.connection?.Close();
            }

            this.connection = null;
        }

        /// <summary>
        /// Asynchronously closes the connection if it is not already closed.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CloseConnectionAsync()
        {
            if (this.connection?.State != ConnectionState.Closed)
            {
                await Task.Run(() => this.connection?.Close());
            }
        }

        public User? GetCurrentUser()
        {
            using (var command = new SqlCommand("SELECT TOP 1 UserId, Username FROM Users", this.GetConnection()))
            {
                try
                {
                    this.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new User(reader.GetString(reader.GetOrdinal("Username")));
                            user.SetUserId(reader.GetInt32(reader.GetOrdinal("UserId")));
                            return user;
                        }

                        return null;
                    }
                }
                finally
                {
                    this.CloseConnection();
                }
            }
        }

        public string GetItemImagePath(Item item)
        {
            try
            {
                // Get the game folder name based on the game title
                string gameFolder = item.Game.Title.ToLower() switch
                {
                    "counter-strike 2" => "cs2",
                    "dota 2" => "dota2",
                    "team fortress 2" => "tf2",
                    _ => item.Game.Title.ToLower().Replace(" ", string.Empty).Replace(":", string.Empty)
                };

                // Return a path to the image based on the ItemId
                var path = $"ms-appx:///Assets/img/games/{gameFolder}/{item.ItemId}.png";
                System.Diagnostics.Debug.WriteLine($"Generated image path for item {item.ItemId} ({item.ItemName}) from {item.Game.Title}: {path}");
                return path;
            }
            catch (Exception getItemImagePathException)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetItemImagePath: {getItemImagePathException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {getItemImagePathException.StackTrace}");
                return "ms-appx:///Assets/img/games/default-item.png";
            }
        }
    }
        [TestFixture]
        public class InventoryRepositoryAdditionalIntegrationTests
        {
            private IInventoryRepository repository;
            private IDatabaseConnector databaseConnector;
            private const string TestConnectionString = Configuration.TESTCONNECTIONSTRINGSONIA; // Points to test database

            [OneTimeSetUp]
            public async Task OneTimeSetUp()
            {
                // Instantiate the connector and repository.
                databaseConnector = new StubDatabaseConnector();
                repository = new InventoryRepository(databaseConnector);

                // Create test tables and synonyms so that production queries resolve to these test tables.
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        IF OBJECT_ID('dbo.TestUsers', 'U') IS NOT NULL DROP TABLE dbo.TestUsers;
                        CREATE TABLE dbo.TestUsers (
                            UserId INT PRIMARY KEY,
                            Username NVARCHAR(100) NOT NULL,
                            WalletBalance MONEY NULL,
                            PointBalance INT NULL,
                            IsDeveloper BIT NULL
                        );

                        IF OBJECT_ID('dbo.TestGames', 'U') IS NOT NULL DROP TABLE dbo.TestGames;
                        CREATE TABLE dbo.TestGames (
                            GameId INT PRIMARY KEY,
                            Title NVARCHAR(200) NOT NULL,
                            Price FLOAT NOT NULL,
                            Genre NVARCHAR(100) NOT NULL,
                            Description NVARCHAR(500) NOT NULL,
                            Status NVARCHAR(50) NOT NULL,
                            RecommendedSpecs FLOAT NULL,
                            MinimumSpecs FLOAT NULL
                        );

                        IF OBJECT_ID('dbo.TestItems', 'U') IS NOT NULL DROP TABLE dbo.TestItems;
                        CREATE TABLE dbo.TestItems (
                            ItemId INT IDENTITY(1,1) PRIMARY KEY,
                            CorrespondingGameId INT NOT NULL,
                            ItemName NVARCHAR(200) NOT NULL,
                            Price FLOAT NOT NULL,
                            Description NVARCHAR(500) NOT NULL,
                            IsListed BIT NOT NULL
                        );

                        IF OBJECT_ID('dbo.TestUserInventory', 'U') IS NOT NULL DROP TABLE dbo.TestUserInventory;
                        CREATE TABLE dbo.TestUserInventory (
                            UserId INT NOT NULL,
                            GameId INT NOT NULL,
                            ItemId INT NOT NULL,
                            PRIMARY KEY (UserId, GameId, ItemId)
                        );

                        -- Create synonyms to map production table names to test tables.
                        IF OBJECT_ID('dbo.Users', 'SN') IS NOT NULL DROP SYNONYM dbo.Users;
                        CREATE SYNONYM dbo.Users FOR dbo.TestUsers;

                        IF OBJECT_ID('dbo.Games', 'SN') IS NOT NULL DROP SYNONYM dbo.Games;
                        CREATE SYNONYM dbo.Games FOR dbo.TestGames;

                        IF OBJECT_ID('dbo.Items', 'SN') IS NOT NULL DROP SYNONYM dbo.Items;
                        CREATE SYNONYM dbo.Items FOR dbo.TestItems;

                        IF OBJECT_ID('dbo.UserInventory', 'SN') IS NOT NULL DROP SYNONYM dbo.UserInventory;
                        CREATE SYNONYM dbo.UserInventory FOR dbo.TestUserInventory;
                    ";
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }
            }

            [SetUp]
            public async Task SetUp()
            {
                // Before each test, clear all test tables.
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        DELETE FROM dbo.TestUserInventory;
                        DELETE FROM dbo.TestItems;
                        DELETE FROM dbo.TestGames;
                        DELETE FROM dbo.TestUsers;
                    ";
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }
            }
        [Test]
        public async Task SellItemAsync_ValidItem_CompletesSale()
        {
            // Arrange: Create a Game and an Item.
            var game = new Game("Sell Game", 15.99f, "Action", "Some Description");
            game.SetGameId(300);
            var item = new Item("Sellable Item", game, 9.99f, "To Sell");

            int insertedItemId;
            // Insert a dummy row into dbo.TestItems with IsListed = 0 (unsold).
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO dbo.TestItems (CorrespondingGameId, ItemName, Price, Description, IsListed)
                        OUTPUT INSERTED.ItemId
                        VALUES (@gameId, @itemName, @price, @description, @isListed);
                    ";
                    command.Parameters.Add(new SqlParameter("@gameId", game.GameId));
                    command.Parameters.Add(new SqlParameter("@itemName", "Sellable Item"));
                    command.Parameters.Add(new SqlParameter("@price", 9.99));
                    command.Parameters.Add(new SqlParameter("@description", "To Sell"));
                    command.Parameters.Add(new SqlParameter("@isListed", false));
                    insertedItemId = (int)await command.ExecuteScalarAsync();
                }
                connection.Close();
            }
            item.SetItemId(insertedItemId);

            // Act: Call SellItemAsync to mark the item as sold.
            bool result = await repository.SellItemAsync(item);

            // Assert: Verify that SellItemAsync returns true.
            Assert.IsTrue(result, "SellItemAsync should return true upon successful sale.");

            // Assert: Check that the item is marked as sold (IsListed = 1).
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT IsListed FROM dbo.TestItems WHERE ItemId = @itemId;";
                    command.Parameters.Add(new SqlParameter("@itemId", insertedItemId));

                    bool isListed = (bool)await command.ExecuteScalarAsync();
                    Assert.IsTrue(isListed, "Item should have been marked as sold (IsListed = 1).");
                }
                connection.Close();
            }
        }
        [Test]
        public void GetItemsFromInventoryAsync_NullGame_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await repository.GetItemsFromInventoryAsync(null));
        }

        [Test]
        public async Task AddItemToInventoryAsync_ValidParameters_InsertsItem()
        {
            // Arrange: Create a Game, a User, and an Item.
            var game = new Game("Test Game", 9.99f, "RPG", "Game Description");
            game.SetGameId(101);
            var user = new User("newUser");
            user.SetUserId(200);
            var item = new Item("New Item", game, 5.99f, "Item Desc");

            // Insert the dummy Game into dbo.TestGames (without SET IDENTITY_INSERT as GameId is not identity).
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO dbo.TestGames (GameId, Title, Price, Genre, Description, Status, RecommendedSpecs, MinimumSpecs)
                        VALUES (@GameId, @Title, @Price, @Genre, @Description, @Status, NULL, NULL);
                    ";
                    command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                    command.Parameters.Add(new SqlParameter("@Title", game.Title));
                    command.Parameters.Add(new SqlParameter("@Price", game.Price));
                    command.Parameters.Add(new SqlParameter("@Genre", game.Genre));
                    command.Parameters.Add(new SqlParameter("@Description", game.Description));
                    command.Parameters.Add(new SqlParameter("@Status", "Active"));
                    await command.ExecuteNonQueryAsync();
                }

                // Insert the dummy User into dbo.TestUsers.
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO dbo.TestUsers (UserId, Username, WalletBalance, PointBalance, IsDeveloper)
                        VALUES (@UserId, @Username, 0, 0, 0);
                    ";
                    command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                    command.Parameters.Add(new SqlParameter("@Username", user.Username));
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
            }

            // Act: Call the repository method to add the item.
            await repository.AddItemToInventoryAsync(game, item, user);

            // Assert: Verify that a linking row exists in dbo.TestUserInventory.
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COUNT(*) 
                        FROM dbo.TestUserInventory 
                        WHERE UserId = @UserId AND GameId = @GameId AND ItemId = @ItemId;
                    ";
                    command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                    command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                    command.Parameters.Add(new SqlParameter("@ItemId", item.ItemId));

                    int count = (int)await command.ExecuteScalarAsync();
                    Assert.AreEqual(1, count, "Item should have been inserted into the TestUserInventory table.");
                }
                connection.Close();
            }
        }

        [Test]
        public async Task GetItemsFromInventoryAsync_ValidGame_ReturnsCorrectItems()
        {
            // Arrange: Create a Game instance.
            var game = new Game("Test Game", 9.99f, "Adventure", "Test Description");
            game.SetGameId(100);

            // Insert a dummy user into dbo.TestUsers so that GetCurrentUser returns a valid user.
            var testUser = new User("testuser");
            testUser.SetUserId(50);
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                INSERT INTO dbo.TestUsers (UserId, Username, WalletBalance, PointBalance, IsDeveloper)
                VALUES (@UserId, @Username, 0, 0, 0);
            ";
                    command.Parameters.Add(new SqlParameter("@UserId", testUser.UserId));
                    command.Parameters.Add(new SqlParameter("@Username", testUser.Username));
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
            }

            // Insert a dummy item into dbo.TestItems and get the generated ItemId.
            int insertedItemId;
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                INSERT INTO dbo.TestItems (CorrespondingGameId, ItemName, Price, Description, IsListed)
                OUTPUT INSERTED.ItemId
                VALUES (@gameId, @itemName, @price, @description, @isListed);
            ";
                    command.Parameters.Add(new SqlParameter("@gameId", game.GameId));
                    command.Parameters.Add(new SqlParameter("@itemName", "Test Item"));
                    command.Parameters.Add(new SqlParameter("@price", 19.99));
                    command.Parameters.Add(new SqlParameter("@description", "Item Description"));
                    command.Parameters.Add(new SqlParameter("@isListed", false));
                    insertedItemId = (int)await command.ExecuteScalarAsync();
                }
                connection.Close();
            }

            // Insert a linking row into dbo.TestUserInventory so that the JOIN in the repository query finds it.
            using (var connection = new SqlConnection(TestConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                INSERT INTO dbo.TestUserInventory (UserId, GameId, ItemId)
                VALUES (@UserId, @gameId, @ItemId);
            ";
                    command.Parameters.Add(new SqlParameter("@UserId", testUser.UserId));
                    command.Parameters.Add(new SqlParameter("@gameId", game.GameId));
                    command.Parameters.Add(new SqlParameter("@ItemId", insertedItemId));
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
            }

            // Act: Retrieve items via the repository.
            var items = await repository.GetItemsFromInventoryAsync(game);

            // Assert: Verify that one item is returned with expected details.
            Assert.IsNotNull(items, "Returned item list should not be null");
            Assert.AreEqual(1, items.Count, "There should be one item returned");

            var item = items[0];
            Assert.AreEqual("Test Item", item.ItemName, "ItemName should match");
            Assert.AreEqual(19.99f, item.Price, "Price should match");
            Assert.AreEqual("Item Description", item.Description, "Description should match");
            Assert.IsFalse(item.IsListed, "IsListed should be false as per inserted data");
        }

        #region GetUserInventoryAsync Tests

        [Test]
            public async Task GetUserInventoryAsync_ValidUser_ReturnsCorrectItemCount()
            {
                // Arrange: Insert a dummy game, item, and linking row.
                var user = new User("testuser");
                user.SetUserId(1);

                // Insert a game into TestGames.
                var game = new Game("Test Game", 9.99f, "Adventure", "Game Desc");
                game.SetGameId(10);
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestGames (GameId, Title, Price, Genre, Description, Status, RecommendedSpecs, MinimumSpecs)
                        VALUES (@GameId, @Title, @Price, @Genre, @Description, @Status, NULL, NULL);
                    ";
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@Title", game.Title));
                        command.Parameters.Add(new SqlParameter("@Price", game.Price));
                        command.Parameters.Add(new SqlParameter("@Genre", game.Genre));
                        command.Parameters.Add(new SqlParameter("@Description", game.Description));
                        command.Parameters.Add(new SqlParameter("@Status", "Active"));
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }

                // Insert an item into TestItems.
                int insertedItemId;
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestItems (CorrespondingGameId, ItemName, Price, Description, IsListed)
                        OUTPUT INSERTED.ItemId
                        VALUES (@GameId, @ItemName, @Price, @Description, @IsListed);
                    ";
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemName", "InventoryItem"));
                        command.Parameters.Add(new SqlParameter("@Price", 20.0));
                        command.Parameters.Add(new SqlParameter("@Description", "Item Desc"));
                        command.Parameters.Add(new SqlParameter("@IsListed", false));
                        insertedItemId = (int)await command.ExecuteScalarAsync();
                    }
                    connection.Close();
                }

                // Insert linking row into TestUserInventory.
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestUserInventory (UserId, GameId, ItemId)
                        VALUES (@UserId, @GameId, @ItemId);
                    ";
                        command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemId", insertedItemId));
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }

                // Act: Call GetUserInventoryAsync.
                var items = await repository.GetUserInventoryAsync(user.UserId);

                // Assert: Check that exactly one item is returned.
                Assert.AreEqual(1, items.Count, "There should be one item in the user inventory.");
            }

            #endregion

            #region GetAllItemsFromInventoryAsync Tests

            [Test]
            public async Task GetAllItemsFromInventoryAsync_ValidUser_ReturnsCorrectItemCount()
            {
                // Arrange: Insert a dummy user.
                var user = new User("allItemsUser");
                user.SetUserId(2);

                // Insert a game and an item linked to this user.
                var game = new Game("AllItems Game", 19.99f, "RPG", "Game Desc");
                game.SetGameId(20);
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();

                    // Insert game.
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestGames (GameId, Title, Price, Genre, Description, Status, RecommendedSpecs, MinimumSpecs)
                        VALUES (@GameId, @Title, @Price, @Genre, @Description, @Status, NULL, NULL);
                    ";
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@Title", game.Title));
                        command.Parameters.Add(new SqlParameter("@Price", game.Price));
                        command.Parameters.Add(new SqlParameter("@Genre", game.Genre));
                        command.Parameters.Add(new SqlParameter("@Description", game.Description));
                        command.Parameters.Add(new SqlParameter("@Status", "Active"));
                        await command.ExecuteNonQueryAsync();
                    }

                    // Insert an item into TestItems.
                    int insertedItemId;
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestItems (CorrespondingGameId, ItemName, Price, Description, IsListed)
                        OUTPUT INSERTED.ItemId
                        VALUES (@GameId, @ItemName, @Price, @Description, @IsListed);
                    ";
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemName", "AllItemsItem"));
                        command.Parameters.Add(new SqlParameter("@Price", 25.5));
                        command.Parameters.Add(new SqlParameter("@Description", "Item Desc"));
                        command.Parameters.Add(new SqlParameter("@IsListed", false));
                        insertedItemId = (int)await command.ExecuteScalarAsync();
                    }

                    // Insert linking row into TestUserInventory.
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestUserInventory (UserId, GameId, ItemId)
                        VALUES (@UserId, @GameId, @ItemId);
                    ";
                        command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemId", insertedItemId));
                        await command.ExecuteNonQueryAsync();
                    }

                    connection.Close();
                }

                // Act: Call GetAllItemsFromInventoryAsync.
                var items = await repository.GetAllItemsFromInventoryAsync(user);

                // Assert: Check that exactly one item is returned.
                Assert.AreEqual(1, items.Count, "There should be one item returned for the user.");
            }

            #endregion

            #region RemoveItemFromInventoryAsync Tests

            [Test]
            public async Task RemoveItemFromInventoryAsync_ValidData_RemovesLinkingRow()
            {
                // Arrange: Insert a dummy user, game, and item.
                var user = new User("removeUser");
                user.SetUserId(3);
                var game = new Game("Remove Game", 9.99f, "Action", "Game Desc");
                game.SetGameId(30);
                var item = new Item("RemoveItem", game, 10.0f, "Item Desc");
                int insertedItemId;

                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();

                    // Insert game.
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestGames (GameId, Title, Price, Genre, Description, Status, RecommendedSpecs, MinimumSpecs)
                        VALUES (@GameId, @Title, @Price, @Genre, @Description, @Status, NULL, NULL);
                    ";
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@Title", game.Title));
                        command.Parameters.Add(new SqlParameter("@Price", game.Price));
                        command.Parameters.Add(new SqlParameter("@Genre", game.Genre));
                        command.Parameters.Add(new SqlParameter("@Description", game.Description));
                        command.Parameters.Add(new SqlParameter("@Status", "Active"));
                        await command.ExecuteNonQueryAsync();
                    }

                    // Insert item.
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestItems (CorrespondingGameId, ItemName, Price, Description, IsListed)
                        OUTPUT INSERTED.ItemId
                        VALUES (@GameId, @ItemName, @Price, @Description, @IsListed);
                    ";
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemName", "RemoveItem"));
                        command.Parameters.Add(new SqlParameter("@Price", 10.0));
                        command.Parameters.Add(new SqlParameter("@Description", "Item Desc"));
                        command.Parameters.Add(new SqlParameter("@IsListed", false));
                        insertedItemId = (int)await command.ExecuteScalarAsync();
                    }
                    item.SetItemId(insertedItemId);

                    // Insert user.
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestUsers (UserId, Username, WalletBalance, PointBalance, IsDeveloper)
                        VALUES (@UserId, @Username, 0, 0, 0);
                    ";
                        command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                        command.Parameters.Add(new SqlParameter("@Username", user.Username));
                        await command.ExecuteNonQueryAsync();
                    }

                    // Insert linking row into TestUserInventory.
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestUserInventory (UserId, GameId, ItemId)
                        VALUES (@UserId, @GameId, @ItemId);
                    ";
                        command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemId", insertedItemId));
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }

                // Act: Call RemoveItemFromInventoryAsync.
                await repository.RemoveItemFromInventoryAsync(game, item, user);

                // Assert: Query TestUserInventory and verify that no row exists for the given link.
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT COUNT(*) FROM dbo.TestUserInventory
                        WHERE UserId = @UserId AND GameId = @GameId AND ItemId = @ItemId;
                    ";
                        command.Parameters.Add(new SqlParameter("@UserId", user.UserId));
                        command.Parameters.Add(new SqlParameter("@GameId", game.GameId));
                        command.Parameters.Add(new SqlParameter("@ItemId", item.ItemId));

                        int count = (int)await command.ExecuteScalarAsync();
                        Assert.AreEqual(0, count, "The linking row should have been deleted.");
                    }
                    connection.Close();
                }
            }

            #endregion

            #region GetAllUsersAsync Tests

            [Test]
            public async Task GetAllUsersAsync_ReturnsCorrectUserCount()
            {
                // Arrange: Insert two dummy users into TestUsers.
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO dbo.TestUsers (UserId, Username, WalletBalance, PointBalance, IsDeveloper)
                        VALUES (@UserId1, @Username1, 0, 0, 0);
                        INSERT INTO dbo.TestUsers (UserId, Username, WalletBalance, PointBalance, IsDeveloper)
                        VALUES (@UserId2, @Username2, 0, 0, 0);
                    ";
                        command.Parameters.Add(new SqlParameter("@UserId1", 101));
                        command.Parameters.Add(new SqlParameter("@Username1", "UserOne"));
                        command.Parameters.Add(new SqlParameter("@UserId2", 102));
                        command.Parameters.Add(new SqlParameter("@Username2", "UserTwo"));
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }

                // Act: Call GetAllUsersAsync.
                var users = await repository.GetAllUsersAsync();

                // Assert: Verify that two users are returned.
                Assert.AreEqual(2, users.Count, "There should be two users returned.");
            }

            #endregion

            [OneTimeTearDown]
            public async Task OneTimeTearDown()
            {
                // Clean up: Drop synonyms and test tables.
                using (var connection = new SqlConnection(TestConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        // Drop synonyms.
                        command.CommandText = @"
                        IF OBJECT_ID('dbo.Users', 'SN') IS NOT NULL DROP SYNONYM dbo.Users;
                        IF OBJECT_ID('dbo.Games', 'SN') IS NOT NULL DROP SYNONYM dbo.Games;
                        IF OBJECT_ID('dbo.Items', 'SN') IS NOT NULL DROP SYNONYM dbo.Items;
                        IF OBJECT_ID('dbo.UserInventory', 'SN') IS NOT NULL DROP SYNONYM dbo.UserInventory;
                    ";
                        await command.ExecuteNonQueryAsync();

                        // Drop test tables.
                        command.CommandText = @"
                        IF OBJECT_ID('dbo.TestUserInventory', 'U') IS NOT NULL DROP TABLE dbo.TestUserInventory;
                        IF OBJECT_ID('dbo.TestItems', 'U') IS NOT NULL DROP TABLE dbo.TestItems;
                        IF OBJECT_ID('dbo.TestGames', 'U') IS NOT NULL DROP TABLE dbo.TestGames;
                        IF OBJECT_ID('dbo.TestUsers', 'U') IS NOT NULL DROP TABLE dbo.TestUsers;
                    ";
                        await command.ExecuteNonQueryAsync();
                    }
                    connection.Close();
                }
            }
        }
}

