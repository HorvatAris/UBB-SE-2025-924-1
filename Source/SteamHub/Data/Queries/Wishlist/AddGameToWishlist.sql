DROP PROCEDURE IF EXISTS AddGameToWishlist;
GO

CREATE PROCEDURE AddGameToWishlist
    @user_id INT,
    @game_id INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND isInWishlist = 1)
    BEGIN
        RAISERROR ('Game is already in wishlist', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND is_purchased = 1)
    BEGIN
        RAISERROR ('Game is already purchased', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
    BEGIN
        INSERT INTO games_users (user_id, game_id, isInWishlist, is_purchased, isInCart)
        VALUES (@user_id, @game_id, 1, 0, 0);
    END
    ELSE
    BEGIN
        UPDATE games_users
        SET isInWishlist = 1
        WHERE user_id = @user_id AND game_id = @game_id;
    END
END;
GO 