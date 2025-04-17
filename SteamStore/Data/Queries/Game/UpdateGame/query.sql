DROP PROCEDURE IF EXISTS UpdateGame
GO

CREATE PROCEDURE UpdateGame
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
    
    UPDATE games
    SET name = @name,
        price = @price,
        publisher_id = @publisher_id,
        description = @description,
        image_url = @image_url,
        trailer_url = @trailer_url,
        gameplay_url = @gameplay_url,
        minimum_requirements = @minimum_requirements,
        recommended_requirements = @recommended_requirements,
        status = @status,
        discount = @discount,
        reject_message = NULL
    WHERE game_id = @game_id;
END;
GO
