DROP PROCEDURE IF EXISTS DeleteGameTransactions;
GO

CREATE PROCEDURE DeleteGameTransactions
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM store_transaction
    WHERE game_id = @game_id;
END;
GO 