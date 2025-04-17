CREATE PROCEDURE addGameToPurchased
	@user_id INT,
	@game_id INT
	AS
	BEGIN
		IF EXISTS (SELECT * FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
			BEGIN
				UPDATE games_users
				SET is_purchased = 1
				WHERE user_id = @user_id AND game_id = @game_id;
			END
		ELSE
			BEGIN
				INSERT INTO games_users (user_id, game_id, is_purchased)
				VALUES (@user_id, @game_id, 1);
			END
	END;