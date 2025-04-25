CREATE TABLE users (
    user_id INT PRIMARY KEY,
	username NVARCHAR(255),
    balance DECIMAL(10,2),
    point_balance DECIMAL(10,2),
    is_developer BIT,
    email NVARCHAR(255)
);

CREATE TABLE games (
game_id INT PRIMARY KEY,
name NVARCHAR(255),
price DECIMAL(10,2),
publisher_id INT FOREIGN KEY REFERENCES users(user_id),
description NVARCHAR(MAX),
image_url NVARCHAR(MAX),
trailer_url NVARCHAR(MAX),
gameplay_url NVARCHAR(MAX),
minimum_requirements NVARCHAR(MAX),
recommended_requirements NVARCHAR(MAX),
status NVARCHAR(MAX),
discount INT,
reject_message NVARCHAR(MAX) NULL
);

CREATE TABLE tags (
    tag_id INT PRIMARY KEY,
    tag_name NVARCHAR(255)
);

CREATE TABLE game_tags (
    tag_id INT FOREIGN KEY REFERENCES tags(tag_id),
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    PRIMARY KEY (tag_id, game_id)
);

CREATE TABLE games_users (
    user_id INT FOREIGN KEY REFERENCES users(user_id),
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    isInWishlist BIT,
    is_purchased BIT,
     isInCart BIT,
    PRIMARY KEY (user_id, game_id)
);

CREATE TABLE store_transaction (
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    user_id INT FOREIGN KEY REFERENCES users(user_id),
    date DATETIME,
    amount DECIMAL(10, 2),
    withMoney BIT
);

CREATE TABLE Items (
    ItemId INT PRIMARY KEY IDENTITY(1,1),
    ItemName NVARCHAR(100) NOT NULL,
    CorrespondingGameId INT FOREIGN KEY REFERENCES Games(game_id),
    Price FLOAT NOT NULL,
    Description NVARCHAR(MAX),
    IsListed BIT NOT NULL DEFAULT 0
);
CREATE TABLE UserInventory (
        UserId INT FOREIGN KEY REFERENCES Users(user_id),
        ItemId INT FOREIGN KEY REFERENCES Items(ItemId),
        GameId INT FOREIGN KEY REFERENCES Games(game_id),
        AcquiredDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        PRIMARY KEY (UserId, ItemId, GameId)
    );
CREATE TABLE ItemTrades (
        TradeId INT PRIMARY KEY IDENTITY(1,1),
        SourceUserId INT FOREIGN KEY REFERENCES Users(user_id),
        DestinationUserId INT FOREIGN KEY REFERENCES Users(user_id),
        GameOfTradeId INT FOREIGN KEY REFERENCES Games(game_id),
        TradeDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        TradeDescription NVARCHAR(MAX),
        TradeStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        AcceptedBySourceUser BIT NOT NULL DEFAULT 0,
        AcceptedByDestinationUser BIT NOT NULL DEFAULT 0
    );

CREATE TABLE ItemTradeDetails (
        TradeId INT FOREIGN KEY REFERENCES ItemTrades(TradeId),
        ItemId INT FOREIGN KEY REFERENCES Items(ItemId),
        IsSourceUserItem BIT NOT NULL, -- True if item is from source user, False if from destination user
        PRIMARY KEY (TradeId, ItemId)
    );
go

CREATE TABLE PointShopItems (
    ItemId INT PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    ImagePath NVARCHAR(MAX),
    PointPrice DECIMAL(10, 2) NOT NULL,
    ItemType NVARCHAR(50) NOT NULL -- E.g., "ProfileBackground", "Avatar", "Emoticon", etc.
);

CREATE TABLE UserInventoryItems (
    UserId INT,
    ItemId INT,
    PurchaseDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 0,
    PRIMARY KEY (UserId, ItemId),
    FOREIGN KEY (ItemId) REFERENCES PointShopItems(ItemId)
);
GO