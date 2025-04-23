DROP PROCEDURE IF EXISTS GetGameOwnerCount;
GO

CREATE PROCEDURE GetGameOwnerCount
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS OwnerCount
    FROM games_users
    WHERE game_id = @game_id;
END;
GO 