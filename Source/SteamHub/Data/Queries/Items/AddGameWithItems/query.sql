drop procedure if exists AddGameWithItems
go

CREATE PROCEDURE AddGameWithItems
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
        @discount INT,
        @ItemName NVARCHAR(100),
        @ItemPrice FLOAT,
        @ItemDescription NVARCHAR(MAX)
    AS
    BEGIN
        DECLARE @GameId INT;
        
        -- Check if game exists
        SELECT @GameId = game_id FROM Games WHERE Name = @name;
        
        -- If game doesn''t exist, create it
        IF @GameId IS NULL
        BEGIN
            
        INSERT INTO games (game_id, name, price, publisher_id, description, image_url, 
                          trailer_url, gameplay_url, minimum_requirements, 
                          recommended_requirements, status, discount, reject_message)
        VALUES (@game_id, @name, @price, @publisher_id, @description, @image_url, 
                @trailer_url, @gameplay_url, @minimum_requirements, 
                @recommended_requirements, 'Available', @discount, NULL);
            
            SET @GameId = SCOPE_IDENTITY();
        END
        
        -- Insert the item
        INSERT INTO Items (ItemName, CorrespondingGameId, Price, Description, IsListed)
        VALUES (@ItemName, @GameId, @ItemPrice, @ItemDescription, 0);
        
        SELECT @GameId AS GameId;
    END