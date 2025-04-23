CREATE PROCEDURE RemoveGameFromWishlist
	@user_id INT,
	@game_id INT
	AS
	BEGIN
		IF EXISTS (SELECT * FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
			BEGIN
				UPDATE games_users
				SET isInWishlist = 0
				WHERE user_id = @user_id AND game_id = @game_id;
			END
		ELSE
			BEGIN
				RAISERROR('Game is not in wishlist', 16, 1);
			END
	END;