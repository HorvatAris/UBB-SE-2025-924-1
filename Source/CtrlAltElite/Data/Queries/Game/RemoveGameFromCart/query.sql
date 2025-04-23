CREATE PROCEDURE RemoveGameFromCart
@user_id INT,
@game_id INT
AS
BEGIN
	IF EXISTS (SELECT * FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
		BEGIN
			UPDATE games_users
			SET isInCart = 0
			WHERE user_id = @user_id AND game_id = @game_id;
		END
	ELSE
		BEGIN
			INSERT INTO games_users (user_id, game_id, is_purchased)
			VALUES (@user_id, @game_id, 0);
		END
END;