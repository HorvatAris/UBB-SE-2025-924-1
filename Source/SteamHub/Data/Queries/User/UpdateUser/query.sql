CREATE PROCEDURE UpdateUser
    @UserId INT,
    @UserName NVARCHAR(255),
    @WalletBalance DECIMAL(10,2),
    @PointBalance DECIMAL(10,2),
    @IsDeveloper BIT,
    @useremail NVARCHAR(255)
AS
BEGIN
    UPDATE users
    SET 
        username = @UserName,
        balance = @WalletBalance,
        point_balance = @PointBalance,
        is_developer = @IsDeveloper,
        email=@useremail
    WHERE user_id = @UserId;
END
