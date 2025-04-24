CREATE PROCEDURE GetItemsFromInventory
    @GameId INT,
    @UserId INT
AS
BEGIN
    SELECT 
        i.ItemId,
        i.ItemName,
        i.Price,
        i.Description,
        i.IsListed
    FROM Items i
    JOIN UserInventory ui ON i.ItemId = ui.ItemId
    WHERE ui.game_id = @GameId AND ui.user_id = @UserId;
END
