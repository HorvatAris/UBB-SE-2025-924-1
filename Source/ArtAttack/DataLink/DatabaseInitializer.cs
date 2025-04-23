// <copyright file="DatabaseInitializer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.DataLink
{
    using System;
    using System.IO;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Initializes the database.
    /// </summary>
    public static class DatabaseInitializer
    {
        private static readonly string MasterConnectionString = @"Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <exception cref="FileNotFoundException"> Thrown if database initialization script not found. </exception>
        public static void InitializeDatabase()
        {
            try
            {
                // First check if the database is already initialized
                using (var databaseConnection = new SqlConnection(MasterConnectionString))
                {
                    databaseConnection.Open();
                    using (var checkIfDatabaseExistscommand = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = 'SteampunksDB'", databaseConnection))
                    {
                        var doesDatabasebExists = (int)checkIfDatabaseExistscommand.ExecuteScalar() > 0;
                        if (doesDatabasebExists)
                        {
                            // Check if tables exist
                            using (var steampunksConnection = new SqlConnection(@"Server=localhost;Database=SteampunksDB;Trusted_Connection=True;TrustServerCertificate=True;"))
                            {
                                steampunksConnection.Open();
                                using (var checkIfTablesExistCommand = new SqlCommand(@" SELECT COUNT(*) FROM sys.tables WHERE name IN ('Users', 'Games', 'Items', 'UserInventory')", steampunksConnection))
                                {
                                    var doTablesExist = (int)checkIfTablesExistCommand.ExecuteScalar() == 4;
                                    if (doTablesExist)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Database is already initialized.");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataLink", "Scripts", "InitializeDatabase.sql");

                if (!File.Exists(scriptPath))
                {
                    System.Diagnostics.Debug.WriteLine($"SQL script not found at path: {scriptPath}");
                    throw new FileNotFoundException("Database initialization script not found.", scriptPath);
                }

                string script = File.ReadAllText(scriptPath);
                System.Diagnostics.Debug.WriteLine("Successfully read SQL script file.");

                using (var databaseConnection = new SqlConnection(MasterConnectionString))
                {
                    try
                    {
                        databaseConnection.Open();
                        System.Diagnostics.Debug.WriteLine("Successfully opened connection to master database.");

                        // Split the script on GO statements
                        string[] commands = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string command in commands)
                        {
                            if (!string.IsNullOrWhiteSpace(command))
                            {
                                using (var sqlCommand = new SqlCommand(command, databaseConnection))
                                {
                                    try
                                    {
                                        sqlCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception sqlCommandException)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error executing SQL command: {sqlCommandException.Message}");
                                        System.Diagnostics.Debug.WriteLine($"Command text: {command}");
                                        throw;
                                    }
                                }
                            }
                        }

                        System.Diagnostics.Debug.WriteLine("Successfully executed all SQL commands.");
                    }
                    catch (Exception databaseConnectionException)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error connecting to database: {databaseConnectionException.Message}");
                        throw;
                    }
                }
            }
            catch (Exception databaseInitializationException)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {databaseInitializationException.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds sample data.
        /// </summary>
        public static void AddSampleData()
        {
            var databaseConnector = new DatabaseConnector();

            // Add CS2 with skins
            databaseConnector.AddGameWithItems(
                gameTitle: "Counter-Strike 2",
                gamePrice: 0.0f,
                genre: "FPS",
                description: "The next evolution of Counter-Strike, featuring updated graphics and refined gameplay.",
                itemsToBeAdded: new[] { ("AK-47 | Asiimov", 50.0f, "A sci-fi themed skin for the AK-47"), ("M4A4 | Howl", 1500.0f, "A rare and coveted M4A4 skin"), ("AWP | Dragon Lore", 10000.0f, "The legendary Dragon Lore skin"), ("Desert Eagle | Blaze", 450.0f, "A blazing hot Desert Eagle skin"), ("Butterfly Knife | Fade", 2000.0f, "A rainbow-colored butterfly knife") });

            // Add Dota 2 with items
            databaseConnector.AddGameWithItems(
                gameTitle: "Dota 2",
                gamePrice: 0.0f,
                genre: "MOBA",
                description: "A complex MOBA game with over 100 unique heroes.",
                itemsToBeAdded: new[] { ("Dragonclaw Hook", 750.0f, "A legendary hook for Pudge"), ("Arcana: Demon Eater", 35.0f, "Special effects for Shadow Fiend"), ("Baby Roshan", 150.0f, "A cute courier skin"), ("Ethereal Flames War Dog", 1000.0f, "A rare courier with special effects") });

            System.Diagnostics.Debug.WriteLine("Sample data added successfully!");
        }
    }
}