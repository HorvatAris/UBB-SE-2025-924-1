CREATE PROCEDURE getGameTags
    @gid int
AS
BEGIN
    SELECT t.tag_name, t.tag_id
    FROM Games g
    INNER JOIN game_tags gt ON gt.game_id = g.game_id
    INNER JOIN tags t ON t.tag_id = gt.tag_id
    WHERE g.game_id=@gid; 
END;
