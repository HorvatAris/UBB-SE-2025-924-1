CREATE PROCEDURE isGamePurchased
@game_id INT,
@user_id INT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT 1 FROM games_users WHERE game_id = @game_id AND user_id = @user_id AND is_purchased = 1)
		SELECT 1 AS Result;
	ELSE
		SELECT 0 AS Result;
END;
