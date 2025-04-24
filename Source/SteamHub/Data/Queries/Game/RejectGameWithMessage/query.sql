DROP PROCEDURE IF EXISTS RejectGameWithMessage
GO

CREATE PROCEDURE RejectGameWithMessage
    @game_id INT,
    @rejection_message NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE games
    SET status = 'Rejected',
        reject_message = @rejection_message
    WHERE game_id = @game_id;
END;
GO