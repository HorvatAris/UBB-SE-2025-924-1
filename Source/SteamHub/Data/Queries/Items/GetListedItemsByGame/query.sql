CREATE PROCEDURE GetListedItemsByGame
    @GameId INT
AS
BEGIN
    SELECT 
        i.ItemId, 
        i.ItemName, 
        i.Price, 
        i.Description, 
        i.IsListed,
        g.game_id, 
        g.name AS GameTitle, 
        g.description AS GameDescription,
        g.price AS GamePrice, 
        g.status AS GameStatus
    FROM Items i
    JOIN UserInventory ui ON i.ItemId = ui.ItemId
    JOIN Games g ON ui.game_id = g.GameId
    WHERE g.game_id = @GameId AND i.IsListed = 1;
END
