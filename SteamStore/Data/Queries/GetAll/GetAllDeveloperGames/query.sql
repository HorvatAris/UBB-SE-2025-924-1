DROP PROCEDURE IF EXISTS GetDeveloperGames
GO

CREATE PROCEDURE GetDeveloperGames
    @publisher_id INT
AS
BEGIN    
    SELECT *
    FROM games
    WHERE publisher_id = @publisher_id;
END;
GO
