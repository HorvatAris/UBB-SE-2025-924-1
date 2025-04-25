DROP PROCEDURE IF EXISTS GetAllUnvalidated
GO

CREATE PROCEDURE GetAllUnvalidated
    @publisher_id INT
AS
BEGIN    
    SELECT *
    FROM games
    WHERE status = 'Pending' AND publisher_id <> @publisher_id;
END;
GO
