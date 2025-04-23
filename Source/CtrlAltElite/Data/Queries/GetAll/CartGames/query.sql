CREATE PROCEDURE GetAllCartGames
    @user_id INT
AS
BEGIN
    SELECT g.game_id, g.name, g.price, g.publisher_id, g.description, g.image_url 
    FROM games g
    LEFT JOIN games_users gu ON g.game_id = gu.game_id AND gu.user_id = @user_id
    WHERE gu.is_purchased = 1 AND g.status = 'Available';
END;