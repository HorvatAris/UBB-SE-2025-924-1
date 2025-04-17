DROP PROCEDURE IF EXISTS DeleteGameTags;
GO

CREATE PROCEDURE DeleteGameTags
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM game_tags
    WHERE game_id = @game_id;
END;
GO 