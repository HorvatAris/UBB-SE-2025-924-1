DROP PROCEDURE IF EXISTS InsertGameTags;
GO

CREATE PROCEDURE InsertGameTags
    @game_id INT,
    @tag_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM game_tags WHERE game_id = @game_id AND tag_id = @tag_id)
    BEGIN
        INSERT INTO game_tags (game_id, tag_id)
        VALUES (@game_id, @tag_id);
    END
END;
GO 