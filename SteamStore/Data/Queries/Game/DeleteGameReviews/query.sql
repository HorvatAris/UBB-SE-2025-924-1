DROP PROCEDURE IF EXISTS DeleteGameReviews;
GO

CREATE PROCEDURE DeleteGameReviews
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM game_reviews
    WHERE game_id = @game_id;
END;
GO 