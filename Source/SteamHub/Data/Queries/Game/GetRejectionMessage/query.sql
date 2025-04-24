DROP PROCEDURE IF EXISTS GetRejectionMessage
GO

CREATE PROCEDURE GetRejectionMessage
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT reject_message
    FROM games
    WHERE game_id = @game_id;
END;
GO