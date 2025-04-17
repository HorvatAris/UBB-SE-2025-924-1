DROP PROCEDURE IF EXISTS RejectGame
GO

CREATE PROCEDURE RejectGame
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE games
    SET status = 'Rejected'
    WHERE game_id = @game_id;
END;
GO