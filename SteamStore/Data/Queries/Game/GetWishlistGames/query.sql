CREATE PROCEDURE GetWishlistGames
    @user_id INT
AS
BEGIN
    SELECT g.game_id, g.name, g.price, g.description, g.image_url, 
           g.minimum_requirements, g.recommended_requirements, g.status,
           g.rating, g.discount
    FROM games g
    INNER JOIN games_users gu ON g.game_id = gu.game_id
    WHERE gu.user_id = @user_id AND gu.isInWishlist = 1 AND g.status = 'Approved';
END; 