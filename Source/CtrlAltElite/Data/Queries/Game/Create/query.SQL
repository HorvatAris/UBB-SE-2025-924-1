CREATE PROCEDURE CreateGame
    @name NVARCHAR(100),
    @price DECIMAL(18, 2),
    @publisher_id BIGINT,
    @description NVARCHAR(1000),
    @image_url NVARCHAR(1000)
AS
BEGIN
    INSERT INTO Games (name, price, publisher_id, description, image_url)
    VALUES (@name, @price, @publisher_id, @description, @image_url);
END;
