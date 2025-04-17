CREATE PROCEDURE GetAllGames  
AS  
BEGIN  
    SELECT game_id, name, price, publisher_id, description, image_url, minimum_requirements, recommended_requirements, status 
    FROM Games;  
END;
go
