CREATE PROCEDURE CreateUser
    @Username NVARCHAR(30),
    @Balance DECIMAL(10, 2),
    @PointBalance DECIMAL(10, 2),
    @isDeveloper BIT,
    @useremail NVARCHAR(255)
AS
BEGIN
    INSERT INTO Users (Username, Balance, PointBalance, isDeveloper, email)
    VALUES (@Username, @Balance, @PointBalance, @isDeveloper, @useremail)
END;