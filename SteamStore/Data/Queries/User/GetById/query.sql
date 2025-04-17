CREATE PROCEDURE GetUserById
    @UserId INT
AS
BEGIN
    SELECT * FROM users WHERE user_id = @UserId;
END;