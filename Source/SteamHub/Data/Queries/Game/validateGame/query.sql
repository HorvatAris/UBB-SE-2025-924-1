CREATE PROCEDURE ValidateGame
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE games
    SET status = 'Approved',
        reject_message = NULL
    WHERE game_id = @game_id AND status = 'Pending';
END;