CREATE PROCEDURE GetUserInventory
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
    JOIN Games g ON i.CorrespondingGameId = g.game_id
    JOIN UserInventory ui ON i.ItemId = ui.ItemId AND g.game_id = ui.game_id
    WHERE ui.user_id = @UserId
    ORDER BY g.name, i.price;
END
