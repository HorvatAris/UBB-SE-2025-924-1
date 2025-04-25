CREATE PROCEDURE MakeItemNotListable
    @ItemId INT
AS
BEGIN
    UPDATE Items
    SET IsListed = 0
    WHERE ItemId = @ItemId;
END
