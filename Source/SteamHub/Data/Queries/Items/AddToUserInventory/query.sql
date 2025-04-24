drop procedure if exists AddToUserInventory
go

 CREATE PROCEDURE AddToUserInventory
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