CREATE PROCEDURE AddGameToCart
    @user_id INT,
    @game_id INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND isInCart = 1)
    BEGIN
        RAISERROR ('The game is already in the cart.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND is_purchased = 1)
    BEGIN
        RAISERROR ('The game is already purchased.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
    BEGIN
        UPDATE games_users
        SET isInCart = 1
        WHERE user_id = @user_id AND game_id = @game_id;
    END
    ELSE
    BEGIN
        INSERT INTO games_users (user_id, game_id, isInCart)
        VALUES (@user_id, @game_id, 1);
    END
END;
