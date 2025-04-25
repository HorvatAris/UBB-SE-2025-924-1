USE [master]
GO
/****** Object:  Database [SteampunksDB]    Script Date: 3/27/2025 4:14:06 PM ******/
CREATE DATABASE [SteampunksDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SteampunksDB', FILENAME = N'C:\Users\Public\SteampunksDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SteampunksDB_log', FILENAME = N'C:\Users\Public\SteampunksDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO

USE SteampunksDB;
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        WalletBalance FLOAT NOT NULL DEFAULT 0,
        PointBalance FLOAT NOT NULL DEFAULT 0,
        IsDeveloper BIT NOT NULL DEFAULT 0
    );
END
GO

-- Create Games table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Games')
BEGIN
    CREATE TABLE Games (
        GameId INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(100) NOT NULL,
        Price FLOAT NOT NULL,
        Genre NVARCHAR(50) NOT NULL,
        Description NVARCHAR(MAX),
        Status NVARCHAR(20) NOT NULL,
        RecommendedSpecs FLOAT,
        MinimumSpecs FLOAT
    );
END
GO

-- Create Items table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Items')
BEGIN
    CREATE TABLE Items (
        ItemId INT PRIMARY KEY IDENTITY(1,1),
        ItemName NVARCHAR(100) NOT NULL,
        CorrespondingGameId INT FOREIGN KEY REFERENCES Games(GameId),
        Price FLOAT NOT NULL,
        Description NVARCHAR(MAX),
        IsListed BIT NOT NULL DEFAULT 0
    );
END
GO

-- Create UserInventory table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserInventory')
BEGIN
    CREATE TABLE UserInventory (
        UserId INT FOREIGN KEY REFERENCES Users(UserId),
        ItemId INT FOREIGN KEY REFERENCES Items(ItemId),
        GameId INT FOREIGN KEY REFERENCES Games(GameId),
        AcquiredDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        PRIMARY KEY (UserId, ItemId, GameId)
    );
END
GO

-- Create Reviews table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
BEGIN
    CREATE TABLE Reviews (
        ReviewId INT PRIMARY KEY IDENTITY(1,1),
        ReviewerId INT FOREIGN KEY REFERENCES Users(UserId),
        GameId INT FOREIGN KEY REFERENCES Games(GameId),
        Content NVARCHAR(MAX),
        Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
        ReviewDate DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Create GameTrades table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GameTrades')
BEGIN
    CREATE TABLE GameTrades (
        TradeId INT PRIMARY KEY IDENTITY(1,1),
        SourceUserId INT FOREIGN KEY REFERENCES Users(UserId),
        DestinationUserId INT FOREIGN KEY REFERENCES Users(UserId),
        GameId INT FOREIGN KEY REFERENCES Games(GameId),
        TradeDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        TradeDescription NVARCHAR(MAX),
        AcceptedBySourceUser BIT NOT NULL DEFAULT 0,
        AcceptedByDestinationUser BIT NOT NULL DEFAULT 0,
        TradeStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending'
    );
END
GO

-- Create ItemTrades table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ItemTrades')
BEGIN
    CREATE TABLE ItemTrades (
        TradeId INT PRIMARY KEY IDENTITY(1,1),
        SourceUserId INT FOREIGN KEY REFERENCES Users(UserId),
        DestinationUserId INT FOREIGN KEY REFERENCES Users(UserId),
        GameOfTradeId INT FOREIGN KEY REFERENCES Games(GameId),
        TradeDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        TradeDescription NVARCHAR(MAX),
        TradeStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        AcceptedBySourceUser BIT NOT NULL DEFAULT 0,
        AcceptedByDestinationUser BIT NOT NULL DEFAULT 0
    );
END
GO

-- Create ItemTradeDetails table for many-to-many relationship between ItemTrades and Items
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ItemTradeDetails')
BEGIN
    CREATE TABLE ItemTradeDetails (
        TradeId INT FOREIGN KEY REFERENCES ItemTrades(TradeId),
        ItemId INT FOREIGN KEY REFERENCES Items(ItemId),
        IsSourceUserItem BIT NOT NULL, -- True if item is from source user, False if from destination user
        PRIMARY KEY (TradeId, ItemId)
    );
END
GO

-- Create stored procedure for user registration
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_RegisterUser')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_RegisterUser
        @Username NVARCHAR(50),
        @IsDeveloper BIT = 0
    AS
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
        BEGIN
            INSERT INTO Users (Username, IsDeveloper)
            VALUES (@Username, @IsDeveloper);
            SELECT SCOPE_IDENTITY() AS UserId;
        END
        ELSE
            THROW 50000, ''Username already exists'', 1;
    END
    ');
END
GO

-- Create stored procedure for adding games with items
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_AddGameWithItems')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_AddGameWithItems
        @GameTitle NVARCHAR(100),
        @GamePrice FLOAT,
        @Genre NVARCHAR(50),
        @Description NVARCHAR(MAX),
        @ItemName NVARCHAR(100),
        @ItemPrice FLOAT,
        @ItemDescription NVARCHAR(MAX)
    AS
    BEGIN
        DECLARE @GameId INT;
        
        -- Check if game exists
        SELECT @GameId = GameId FROM Games WHERE Title = @GameTitle;
        
        -- If game doesn''t exist, create it
        IF @GameId IS NULL
        BEGIN
            INSERT INTO Games (Title, Price, Genre, Description, Status)
            VALUES (@GameTitle, @GamePrice, @Genre, @Description, ''Available'');
            
            SET @GameId = SCOPE_IDENTITY();
        END
        
        -- Insert the item
        INSERT INTO Items (ItemName, CorrespondingGameId, Price, Description, IsListed)
        VALUES (@ItemName, @GameId, @ItemPrice, @ItemDescription, 0);
        
        SELECT @GameId AS GameId;
    END
    ');
END
GO

-- Create stored procedure for adding items to user inventory
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_AddToUserInventory')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_AddToUserInventory
        @UserId INT,
        @ItemId INT,
        @GameId INT
    AS
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM UserInventory WHERE UserId = @UserId AND ItemId = @ItemId AND GameId = @GameId)
        BEGIN
            INSERT INTO UserInventory (UserId, ItemId, GameId)
            VALUES (@UserId, @ItemId, @GameId);
        END
    END
    ');
END
GO

-- Create stored procedure for removing items from user inventory
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_RemoveFromUserInventory')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_RemoveFromUserInventory
        @UserId INT,
        @ItemId INT,
        @GameId INT
    AS
    BEGIN
        DELETE FROM UserInventory 
        WHERE UserId = @UserId AND ItemId = @ItemId AND GameId = @GameId;
    END
    ');
END
GO

-- Check if tables are empty before inserting test data
IF NOT EXISTS (SELECT TOP 1 1 FROM Users)
BEGIN
    -- Clean up existing data
    DELETE FROM UserInventory;
    DELETE FROM Items;
    DELETE FROM Games;
    DELETE FROM Users;
    DBCC CHECKIDENT ('UserInventory', RESEED, 0);
    DBCC CHECKIDENT ('Items', RESEED, 0);
    DBCC CHECKIDENT ('Games', RESEED, 0);
    DBCC CHECKIDENT ('Users', RESEED, 0);
    

    -- Insert test users
    INSERT INTO Users (Username, WalletBalance, PointBalance, IsDeveloper)
    VALUES 
    ('TestUser1', 1000, 100, 0),
    ('TestUser2', 1000, 100, 0),
    ('TestUser3', 1000, 100, 0);
    

    -- Insert games
    INSERT INTO Games (Title, Price, Genre, Description, Status)
    VALUES 
    ('Counter-Strike 2', 0.00, 'FPS', 'The latest version of Counter-Strike', 'Available'),
    ('Dota 2', 0.00, 'MOBA', 'A competitive multiplayer game', 'Available'),
    ('Team Fortress 2', 0.00, 'FPS', 'A team-based multiplayer game', 'Available');
    

    -- Insert items
    INSERT INTO Items (ItemName, CorrespondingGameId, Price, Description, IsListed)
    VALUES 
    -- CS2 Items
    ('AK-47 | Asiimov', 1, 150.00, 'A rare and valuable AK-47 skin with a futuristic design', 0),
    ('M4A4 | Howl', 1, 1000.00, 'One of the most expensive and sought-after CS2 skins', 0),
    ('AWP | Dragon Lore', 1, 2000.00, 'A legendary AWP skin with a dragon design', 0),
    ('Karambit | Fade', 1, 800.00, 'A beautiful karambit knife with a fade pattern', 0),
    ('M4A1-S | Knight', 1, 600.00, 'A rare M4A1-S skin with a knight theme', 0),

    -- Dota 2 Items
    ('Dragonclaw Hook', 2, 1200.00, 'A legendary Pudge hook with dragon design', 0),
    ('Timebreaker', 2, 800.00, 'A rare Faceless Void weapon', 0),
    ('Golden Baby Roshan', 2, 1500.00, 'A rare courier with golden design', 0),
    ('Inscribed Golden Baby Roshan', 2, 2000.00, 'An even rarer version of the Golden Baby Roshan', 0),
    ('Dragonclaw Hook (Unusual)', 2, 2000.00, 'A special version of the Dragonclaw Hook with particle effects', 0),

    -- TF2 Items
    ('Unusual Team Captain', 3, 1000.00, 'A rare hat with particle effects', 0),
    ('Burning Flames Modest Pile of Hat', 3, 2000.00, 'One of the most valuable TF2 hats', 0),
    ('Earbuds', 3, 400.00, 'A classic TF2 cosmetic item', 0),
    ('Unusual Burning Flames Team Captain', 3, 3000.00, 'A combination of two rare items', 0),
    ('Unusual Scorching Flames Team Captain', 3, 2500.00, 'Another rare variant of the Team Captain', 0);
    

    -- Insert mock items into UserInventory
    INSERT INTO UserInventory (UserId, ItemId, GameId, AcquiredDate)
    VALUES 
    -- TestUser1's inventory
    (1, 1, 1, GETDATE()),  -- AK-47 | Asiimov
    (1, 2, 1, GETDATE()),  -- M4A4 | Howl
    (1, 3, 1, GETDATE()),  -- AWP | Dragon Lore
    (1, 6, 2, GETDATE()),  -- Dragonclaw Hook
    (1, 7, 2, GETDATE()),  -- Timebreaker

    -- TestUser2's inventory
    (2, 11, 3, GETDATE()),  -- Unusual Team Captain
    (2, 12, 3, GETDATE()),  -- Burning Flames Modest Pile of Hat
    (2, 13, 3, GETDATE()),  -- Earbuds
    (2, 4, 1, GETDATE()),   -- Karambit | Fade
    (2, 5, 1, GETDATE()),   -- M4A1-S | Knight

    -- TestUser3's inventory
    (3, 1, 1, GETDATE()),   -- AK-47 | Asiimov
    (3, 6, 2, GETDATE()),   -- Dragonclaw Hook
    (3, 11, 3, GETDATE()),  -- Unusual Team Captain
    (3, 4, 1, GETDATE()),   -- Karambit | Fade
    (3, 5, 1, GETDATE());   -- M4A1-S | Knight
END