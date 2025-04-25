CREATE PROCEDURE GetAllUsers
AS
BEGIN
    SELECT 
        user_id AS UserId,
        username AS UserName,
        balance AS WalletBalance,
        point_balance AS PointBalance,
        is_developer AS IsDeveloper,
        email as useremail
    FROM users;
END
