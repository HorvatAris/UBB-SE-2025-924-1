CREATE PROCEDURE CreateUser
    @Username NVARCHAR(30),
    @Balance DECIMAL(10, 2),
    @PointBalance DECIMAL(10, 2),
    @isDeveloper BIT
AS
BEGIN
    INSERT INTO Users (Username, Balance, PointBalance, isDeveloper)
    VALUES (@Username, @Balance, @PointBalance, @isDeveloper)
END;