DROP PROCEDURE IF EXISTS GetWishlistGames;
GO

CREATE PROCEDURE GetWishlistGames
    @user_id INT
AS
BEGIN
    SELECT g.game_id, g.name, g.price, g.publisher_id, g.description, g.image_url, g.discount, g.status,
           gu.is_purchased, gu.isInCart, gu.isInWishlist
    FROM games g
    LEFT JOIN games_users gu ON g.game_id = gu.game_id AND gu.user_id = @user_id
    WHERE gu.isInWishlist = 1 AND g.status = 'Approved';
END;
GO 