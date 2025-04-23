DROP PROCEDURE IF EXISTS IsGameIdInUse;
GO

CREATE PROCEDURE IsGameIdInUse
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM games WHERE game_id = @game_id)
        SELECT 1 AS Result;
    ELSE
        SELECT 0 AS Result;
END;
GO 