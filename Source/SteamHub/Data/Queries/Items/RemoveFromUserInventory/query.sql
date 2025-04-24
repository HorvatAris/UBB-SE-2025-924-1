 drop procedure if exists RemoveFromUserInventory
 go
 
 CREATE PROCEDURE RemoveFromUserInventory
        @UserId INT,
        @ItemId INT,
        @GameId INT
    AS
    BEGIN
        DELETE FROM UserInventory 
        WHERE UserId = @UserId AND ItemId = @ItemId AND GameId = @GameId;
    END