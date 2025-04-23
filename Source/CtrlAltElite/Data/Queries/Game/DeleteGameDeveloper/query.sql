DROP PROCEDURE IF EXISTS DeleteGameDeveloper
GO

CREATE PROCEDURE DeleteGameDeveloper
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM games
    WHERE game_id = @game_id;
END;
GO