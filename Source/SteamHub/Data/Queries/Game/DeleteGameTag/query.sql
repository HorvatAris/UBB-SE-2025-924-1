DROP PROCEDURE IF EXISTS DeleteGameTag;
GO

CREATE PROCEDURE DeleteGameTag
    @game_id INT,
    @tag_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM game_tags
    WHERE game_id = @game_id AND tag_id = @tag_id;
END;
GO 