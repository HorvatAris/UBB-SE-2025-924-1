DROP PROCEDURE IF EXISTS InsertGame;
GO

CREATE PROCEDURE InsertGame
    @game_id INT,
    @name NVARCHAR(255),
    @price DECIMAL(10,2),
    @publisher_id INT,
    @description NVARCHAR(MAX),
    @image_url NVARCHAR(MAX),
    @trailer_url NVARCHAR(MAX),
    @gameplay_url NVARCHAR(MAX),
    @minimum_requirements NVARCHAR(MAX),
    @recommended_requirements NVARCHAR(MAX),
    @status NVARCHAR(MAX),
    @discount INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO games (game_id, name, price, publisher_id, description, image_url, 
                      trailer_url, gameplay_url, minimum_requirements, 
                      recommended_requirements, status, discount, reject_message)
    VALUES (@game_id, @name, @price, @publisher_id, @description, @image_url, 
            @trailer_url, @gameplay_url, @minimum_requirements, 
            @recommended_requirements, @status, @discount, NULL);
END;
GO