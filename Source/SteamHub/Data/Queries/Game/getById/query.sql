CREATE PROCEDURE GetGameById
    @game_id INT
AS
BEGIN
    SELECT * FROM games WHERE game_id = @game_id;
END;