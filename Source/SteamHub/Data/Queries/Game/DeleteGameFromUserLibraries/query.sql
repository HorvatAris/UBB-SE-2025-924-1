DROP PROCEDURE IF EXISTS DeleteGameFromUserLibraries;
GO

CREATE PROCEDURE DeleteGameFromUserLibraries
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM games_users
    WHERE game_id = @game_id;
END;
GO 