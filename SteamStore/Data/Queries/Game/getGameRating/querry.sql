CREATE PROCEDURE getGameRating
	@gid int
AS
BEGIN
    SELECT AVG(gr.rating) 
    FROM Games g
    INNER JOIN game_reviews gr ON gr.game_id=g.game_id
    WHERE g.game_id = @gid; 
END;