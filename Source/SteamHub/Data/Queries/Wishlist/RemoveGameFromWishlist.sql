DROP PROCEDURE IF EXISTS RemoveGameFromWishlist;
GO

CREATE PROCEDURE RemoveGameFromWishlist
    @user_id INT,
    @game_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND isInWishlist = 1)
    BEGIN
        RAISERROR ('Game is not in wishlist', 16, 1);
        RETURN;
    END

    UPDATE games_users
    SET isInWishlist = 0
    WHERE user_id = @user_id AND game_id = @game_id;
END;
GO 