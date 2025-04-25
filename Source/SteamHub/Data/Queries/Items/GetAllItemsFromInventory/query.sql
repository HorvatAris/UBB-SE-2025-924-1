CREATE PROCEDURE GetAllItemsFromInventory
    @UserId INT
AS
BEGIN
    SELECT 
        i.ItemId,
        i.ItemName,
        i.Price,
        i.Description,
        i.IsListed,
        g.game_id,
        g.name,
        g.description AS GameDescription,
        g.price AS GamePrice,
        g.status AS GameStatus
    FROM Items i
    JOIN UserInventory ui ON i.ItemId = ui.ItemId
    JOIN Games g ON ui.game_id = g.game_id
    WHERE ui.user_id = @UserId;
END
