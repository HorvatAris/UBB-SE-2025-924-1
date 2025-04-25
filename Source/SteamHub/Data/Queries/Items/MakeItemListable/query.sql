CREATE PROCEDURE MakeItemListable
    @ItemId INT,
    @Price FLOAT
AS
BEGIN
    UPDATE Items
    SET IsListed = 1, Price = @Price
    WHERE ItemId = @ItemId;
END
