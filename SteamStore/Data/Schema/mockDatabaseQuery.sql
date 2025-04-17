USE Steam
GO


DROP TABLE IF EXISTS users_items;
DROP TABLE IF EXISTS point_items;
DROP TABLE IF EXISTS store_transaction;
DROP TABLE IF EXISTS games_users;
DROP TABLE IF EXISTS game_tags;
DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS status;
DROP TABLE IF EXISTS Specifications;
DROP TABLE IF EXISTS specifications;
DROP TABLE IF EXISTS game_reviews;
DROP TABLE IF EXISTS games;
DROP TABLE IF EXISTS Games;
DROP TABLE IF EXISTS users;
GO

CREATE TABLE users (
    user_id INT PRIMARY KEY,
	username NVARCHAR(255),
    balance DECIMAL(10,2),
    point_balance DECIMAL(10,2),
    is_developer BIT
);

-- Check if users already exist before inserting new ones
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 1)
BEGIN
    -- Insert initial users with reset point balances if they don't exist
    INSERT INTO users (user_id, username, balance, point_balance, is_developer)
    VALUES 
    (1, 'John Doe', 999999.99, 999999.99, 1),   -- Developer with many points
    (2, 'Regular User', 1000, 5000, 0),         -- Regular user with 5000 points
    (3, 'Another User', 500, 2500, 0);          -- Another user with fewer points
END
ELSE
BEGIN
    -- Reset point balances for existing users
    UPDATE users SET point_balance = 999999.99 WHERE user_id = 1;
    UPDATE users SET point_balance = 5000 WHERE user_id = 2;
    UPDATE users SET point_balance = 2500 WHERE user_id = 3;
END

CREATE TABLE games (
    game_id INT PRIMARY KEY,
    name NVARCHAR(255),
    price DECIMAL(10,2),
    publisher_id INT FOREIGN KEY REFERENCES users(user_id),
    description NVARCHAR(MAX),
    image_url NVARCHAR(MAX),
	trailer_url NVARCHAR(MAX),
	gameplay_url NVARCHAR(MAX),
	minimum_requirements NVARCHAR(MAX),
	recommended_requirements NVARCHAR(MAX),
	status NVARCHAR(MAX),
	discount INT,
	reject_message NVARCHAR(MAX) NULL
);

CREATE TABLE game_reviews (
    id INT PRIMARY KEY,
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    rating DECIMAL(10, 2),
    comment NVARCHAR(MAX),
    username NVARCHAR(255)
);

CREATE TABLE tags (
    tag_id INT PRIMARY KEY,
    tag_name NVARCHAR(255)
);

CREATE TABLE game_tags (
    tag_id INT FOREIGN KEY REFERENCES tags(tag_id),
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    PRIMARY KEY (tag_id, game_id)
);

CREATE TABLE games_users (
    user_id INT FOREIGN KEY REFERENCES users(user_id),
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    isInWishlist BIT,
    is_purchased BIT,
	isInCart BIT,
    PRIMARY KEY (user_id, game_id)
);

CREATE TABLE store_transaction (
    game_id INT FOREIGN KEY REFERENCES games(game_id),
    user_id INT FOREIGN KEY REFERENCES users(user_id),
    date DATETIME,
    amount DECIMAL(10, 2),
    withMoney BIT
);

CREATE TABLE point_items (
    point_item_id INT PRIMARY KEY,
    name NVARCHAR(255),
    description NVARCHAR(MAX),
    price DECIMAL(10, 2),
    image_url NVARCHAR(MAX)
);

CREATE TABLE users_items (
    user_id INT FOREIGN KEY REFERENCES users(user_id),
    item_id INT FOREIGN KEY REFERENCES point_items(point_item_id)
);
GO

-- Stored Procedures
go
DROP PROCEDURE IF EXISTS GetUserById;
go
CREATE PROCEDURE GetUserById
    @UserId INT
AS
BEGIN
    SELECT * FROM users WHERE user_id = @UserId;
END;
go


go
DROP PROCEDURE IF EXISTS getUserGames;
go
create procedure getUserGames 
	@uid int
	as
begin
	select g.* from Games g
	inner join games_users gu on gu.game_id=g.game_id
	inner join users u on u.user_id=gu.user_id
	where @uid=u.user_id and g.status='Approved'
end

go

DROP PROCEDURE IF EXISTS GetGameOwnerCount;
GO

CREATE PROCEDURE GetGameOwnerCount
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS OwnerCount
    FROM games_users
    WHERE game_id = @game_id;
END;
GO 

DROP PROCEDURE IF EXISTS DeleteGameReviews;
GO

CREATE PROCEDURE DeleteGameReviews
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM game_reviews
    WHERE game_id = @game_id;
END;
GO 

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

DROP PROCEDURE IF EXISTS DeleteGameFromUserLibraries;
GO

CREATE PROCEDURE DeleteGameFromUserLibraries
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM games_users
    WHERE game_id = @game_id;
END;
GO 

DROP PROCEDURE IF EXISTS getAllTags;
go
CREATE PROCEDURE getAllTags as
begin
	Select * from tags;
end
go
DROP PROCEDURE IF EXISTS getGameTags;
go
CREATE PROCEDURE getGameTags
    @gid int
AS
BEGIN
    SELECT t.tag_name , t.tag_id
    FROM Games g
    INNER JOIN game_tags gt ON gt.game_id = g.game_id
    INNER JOIN tags t ON t.tag_id = gt.tag_id
    WHERE g.game_id=@gid; 
END;
go
DROP PROCEDURE IF EXISTS getGameRating;
go
CREATE PROCEDURE getGameRating
	@gid int
AS
BEGIN
    SELECT AVG(gr.rating) 
    FROM Games g
    INNER JOIN game_reviews gr ON gr.game_id=g.game_id
    WHERE g.game_id = @gid; 
END;
go
DROP PROCEDURE IF EXISTS GetAllGames;
GO

CREATE PROCEDURE GetAllGames  
AS  
BEGIN  
    SELECT game_id, name, price, publisher_id, description, image_url, trailer_url, gameplay_url, minimum_requirements, recommended_requirements, status, discount 
    FROM games
	where status='Approved';
END;
GO

DROP PROCEDURE IF EXISTS GetAllCartGames;
GO

CREATE PROCEDURE GetAllCartGames
    @user_id INT
AS
BEGIN
    SELECT g.game_id, g.name, g.price, g.publisher_id, g.description, g.image_url, gu.is_purchased, gu.isInCart
    FROM games g
    LEFT JOIN games_users gu ON g.game_id = gu.game_id AND gu.user_id = @user_id
    WHERE gu.isInCart = 1 AND g.status = 'Approved';
END;
GO

DROP PROCEDURE IF EXISTS AddGameToCart;
GO

CREATE PROCEDURE AddGameToCart
    @user_id INT,
    @game_id INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND isInCart = 1)
    BEGIN
        RAISERROR ('The game is already in the cart.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND is_purchased = 1)
    BEGIN
        RAISERROR ('The game is already purchased.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
    BEGIN
        UPDATE games_users
        SET isInCart = 1
        WHERE user_id = @user_id AND game_id = @game_id;
    END
    ELSE
    BEGIN
        INSERT INTO games_users (user_id, game_id, isInCart)
        VALUES (@user_id, @game_id, 1);
    END
END;
GO

DROP PROCEDURE IF EXISTS RemoveGameFromCart;
GO

CREATE PROCEDURE RemoveGameFromCart
@user_id INT,
@game_id INT
AS
BEGIN
	IF EXISTS (SELECT * FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
		BEGIN
			UPDATE games_users
			SET isInCart = 0
			WHERE user_id = @user_id AND game_id = @game_id;
		END
	ELSE
		BEGIN
			INSERT INTO games_users (user_id, game_id, is_purchased)
			VALUES (@user_id, @game_id, 0);
		END
END;
GO

DROP PROCEDURE IF EXISTS RemoveGameFromWishlist
GO

CREATE PROCEDURE RemoveGameFromWishlist
	@user_id INT,
	@game_id INT
	AS
	BEGIN
		IF EXISTS (SELECT * FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
			BEGIN
				UPDATE games_users
				SET isInWishlist = 0
				WHERE user_id = @user_id AND game_id = @game_id;
			END
		ELSE
			BEGIN
				RAISERROR('Game is not in wishlist', 16, 1);
			END
	END;
GO

DROP PROCEDURE IF EXISTS addGameToPurchased
GO

CREATE PROCEDURE addGameToPurchased
	@user_id INT,
	@game_id INT
	AS
	BEGIN
		IF EXISTS (SELECT * FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
			BEGIN
				UPDATE games_users
				SET is_purchased = 1
				WHERE user_id = @user_id AND game_id = @game_id;
			END
		ELSE
			BEGIN
				INSERT INTO games_users (user_id, game_id, is_purchased)
				VALUES (@user_id, @game_id, 1);
			END
	END;
GO

go
DROP PROCEDURE IF EXISTS getNoOfRecentSalesForGame;
go
create procedure getNoOfRecentSalesForGame 
	@gid int
	as
begin
	SELECT count(*)
	FROM Games g
	INNER JOIN store_transaction st ON st.game_id = g.game_id
	WHERE g.game_id=@gid and st.date > DATEADD(DAY, -7, GETDATE());
end

go
DROP PROCEDURE IF EXISTS addGameToWishlist
GO

CREATE PROCEDURE addGameToWishlist 
    @user_id INT,
    @game_id INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND is_purchased = 1)
    BEGIN
        RAISERROR('Failed to add game to your wishlist: Game already owned', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id AND isInWishlist = 1)
    BEGIN
        RAISERROR('Failed to add game to your wishlist: Already in wishlist', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM games_users WHERE user_id = @user_id AND game_id = @game_id)
    BEGIN
        UPDATE games_users
        SET isInWishlist = 1
        WHERE user_id = @user_id AND game_id = @game_id;
    END
    ELSE
    BEGIN
        INSERT INTO games_users (user_id, game_id, isInWishlist)
        VALUES (@user_id, @game_id, 1);
    END
END;
GO

DROP PROCEDURE IF EXISTS ValidateGame
GO

CREATE PROCEDURE ValidateGame
    @game_id INT
AS
BEGIN 
    SET NOCOUNT ON;
    UPDATE games
    SET status = 'Approved',
        reject_message = NULL
    WHERE game_id = @game_id AND status = 'Pending';
END;
GO


DROP PROCEDURE IF EXISTS InsertGame;
GO

CREATE PROCEDURE InsertGame
    @game_id INT,
    @name NVARCHAR(255),
    @price DECIMAL(10,2),
    @publisher_id INT,
    @description NVARCHAR(MAX),
    @image_url NVARCHAR(MAX),
    @trailer_url NVARCHAR(MAX),
    @gameplay_url NVARCHAR(MAX),
    @minimum_requirements NVARCHAR(MAX),
    @recommended_requirements NVARCHAR(MAX),
    @status NVARCHAR(MAX),
    @discount INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO games (game_id, name, price, publisher_id, description, image_url, 
                      trailer_url, gameplay_url, minimum_requirements, 
                      recommended_requirements, status, discount, reject_message)
    VALUES (@game_id, @name, @price, @publisher_id, @description, @image_url, 
            @trailer_url, @gameplay_url, @minimum_requirements, 
            @recommended_requirements, @status, @discount, NULL);
END;
GO

DROP PROCEDURE IF EXISTS InsertGameTags;
GO

CREATE PROCEDURE InsertGameTags
    @game_id INT,
    @tag_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM game_tags WHERE game_id = @game_id AND tag_id = @tag_id)
    BEGIN
        INSERT INTO game_tags (game_id, tag_id)
        VALUES (@game_id, @tag_id);
    END
END;
GO 

DROP PROCEDURE IF EXISTS GetAllUnvalidated
GO

CREATE PROCEDURE GetAllUnvalidated
    @publisher_id INT
AS
BEGIN    
    SELECT *
    FROM games
    WHERE status = 'Pending' AND publisher_id <> @publisher_id;
END;
GO

DROP PROCEDURE IF EXISTS DeleteGameDeveloper
GO

CREATE PROCEDURE DeleteGameDeveloper
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM games
    WHERE game_id = @game_id;
END;
GO

DROP PROCEDURE IF EXISTS GetDeveloperGames
GO

CREATE PROCEDURE GetDeveloperGames
    @publisher_id INT
AS
BEGIN    
    SELECT *
    FROM games
    WHERE publisher_id = @publisher_id;
END;
GO

DROP PROCEDURE IF EXISTS UpdateGame
GO

CREATE PROCEDURE UpdateGame
    @game_id INT,
    @name NVARCHAR(255),
    @price DECIMAL(10,2),
    @publisher_id INT,
    @description NVARCHAR(MAX),
    @image_url NVARCHAR(MAX),
    @trailer_url NVARCHAR(MAX),
    @gameplay_url NVARCHAR(MAX),
    @minimum_requirements NVARCHAR(MAX),
    @recommended_requirements NVARCHAR(MAX),
    @status NVARCHAR(MAX),
    @discount INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE games
    SET name = @name,
        price = @price,
        publisher_id = @publisher_id,
        description = @description,
        image_url = @image_url,
        trailer_url = @trailer_url,
        gameplay_url = @gameplay_url,
        minimum_requirements = @minimum_requirements,
        recommended_requirements = @recommended_requirements,
        status = @status,
        discount = @discount,
        reject_message = NULL
    WHERE game_id = @game_id;
END;
GO


DROP PROCEDURE IF EXISTS RejectGame
GO

CREATE PROCEDURE RejectGame
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE games
    SET status = 'Rejected'
    WHERE game_id = @game_id;
END;
GO

DROP PROCEDURE IF EXISTS RejectGameWithMessage
GO

CREATE PROCEDURE RejectGameWithMessage
    @game_id INT,
    @rejection_message NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE games
    SET status = 'Rejected',
        reject_message = @rejection_message
    WHERE game_id = @game_id;
END;
GO

DROP PROCEDURE IF EXISTS GetRejectionMessage
GO

CREATE PROCEDURE GetRejectionMessage
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT reject_message
    FROM games
    WHERE game_id = @game_id;
END;
GO

DROP PROCEDURE IF EXISTS isGamePurchased
GO

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
go

DROP PROCEDURE IF EXISTS DeleteGameTags;
GO

CREATE PROCEDURE DeleteGameTags
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM game_tags
    WHERE game_id = @game_id;
END;
GO 

DROP PROCEDURE IF EXISTS IsGameIdInUse;
GO

CREATE PROCEDURE IsGameIdInUse
    @game_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM games WHERE game_id = @game_id)
        SELECT 1 AS Result;
    ELSE
        SELECT 0 AS Result;
END;
GO 

DROP PROCEDURE IF EXISTS GetWishlistGames;
GO

CREATE PROCEDURE GetWishlistGames
    @user_id INT
AS
BEGIN
    SELECT g.game_id, g.name, g.price, g.description, g.image_url, 
           g.minimum_requirements, g.recommended_requirements, g.status,
           g.discount, AVG(gr.rating) as rating
    FROM games g
    INNER JOIN games_users gu ON g.game_id = gu.game_id
    LEFT JOIN game_reviews gr ON g.game_id = gr.game_id
    WHERE gu.user_id = @user_id AND gu.isInWishlist = 1 AND g.status = 'Approved'
    GROUP BY g.game_id, g.name, g.price, g.description, g.image_url, 
             g.minimum_requirements, g.recommended_requirements, g.status,
             g.discount;
END;
GO

-- Insert mock users
-- NOTE: This is now commented out because we already inserted users at the beginning of the script
/* 
INSERT INTO users (user_id, username, balance, point_balance, is_developer)
VALUES
(1, 'john_doe', 100.00, 500.00, 0),
(2, 'jane_smith', 150.00, 300.00, 1),
(3, 'alex_brown', 50.00, 150.00, 0),
(4, 'Behaviour Interactive', 200.00, 1000.00, 1),
(5, 'Valve Corporation', 150.00, 300.00, 1),
(6, 'Nintendo', 250.00, 800.00, 1),
(7, 'Hempuli Oy', 100.00, 500.00, 1),
(8, 'Mobius Digital', 120.00, 600.00, 1),
(9, 'Mojang Studios', 300.00, 900.00, 1),
(10, 'Unknown Worlds Entertainment', 180.00, 700.00, 1),
(11, 'mary_jones', 200.00, 1000.00, 1);
*/

-- Additionally, let's insert publisher users if they don't exist
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 4)
BEGIN
    INSERT INTO users (user_id, username, balance, point_balance, is_developer)
    VALUES
    (4, 'Behaviour Interactive', 200.00, 1000.00, 1),
    (5, 'Valve Corporation', 150.00, 300.00, 1),
    (6, 'Nintendo', 250.00, 800.00, 1),
    (7, 'Hempuli Oy', 100.00, 500.00, 1),
    (8, 'Mobius Digital', 120.00, 600.00, 1),
    (9, 'Mojang Studios', 300.00, 900.00, 1),
    (10, 'Unknown Worlds Entertainment', 180.00, 700.00, 1),
    (11, 'mary_jones', 200.00, 1000.00, 1);
END

-- Insert mock games
delete from games
INSERT INTO games (game_id, name, price, publisher_id, description, image_url, trailer_url, gameplay_url, minimum_requirements, recommended_requirements, status, discount, reject_message)
VALUES 
(1, 'Risk of Rain 2', 24.99, 1, 'A rogue-like third-person shooter where players fight through hordes of monsters to escape an alien planet.', 'https://upload.wikimedia.org/wikipedia/en/c/c1/Risk_of_Rain_2.jpg', 'https://www.youtube.com/watch?v=pJ-aR--gScM', 'https://www.youtube.com/watch?v=Cwk3qmD28CE', '4GB RAM, 2.5GHz Processor, GTX 580', '8GB RAM, 3.0GHz Processor, GTX 680', 'Rejected',20,'Minimum requirements are too high'),
(2, 'Dead by Daylight', 19.99, 1, 'A multiplayer horror game where survivors must evade a killer.', 'https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/11986720-4999-4524-9809-1a25313ee2e5/dg8ii9d-d3f5eb42-9041-4ddc-954a-c3f9359e914e.png?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOjdlMGQxODg5ODIyNjQzNzNhNWYwZDQxNWVhMGQyNmUwIiwiaXNzIjoidXJuOmFwcDo3ZTBkMTg4OTgyMjY0MzczYTVmMGQ0MTVlYTBkMjZlMCIsIm9iaiI6W1t7InBhdGgiOiJcL2ZcLzExOTg2NzIwLTQ5OTktNDUyNC05ODA5LTFhMjUzMTNlZTJlNVwvZGc4aWk5ZC1kM2Y1ZWI0Mi05MDQxLTRkZGMtOTU0YS1jM2Y5MzU5ZTkxNGUucG5nIn1dXSwiYXVkIjpbInVybjpzZXJ2aWNlOmZpbGUuZG93bmxvYWQiXX0.XGf2I0nx7hyCw6EFGJ5lEdexo3Uj5emUoC6texzl3A4', 'https://www.youtube.com/watch?v=JGhIXLO3ul8', 'https://www.youtube.com/watch?v=3wUHKO0ieyY', '8GB RAM, i3-4170, GTX 760', '16GB RAM, i5-6500, GTX 1060', 'Pending',40,NULL),
(3, 'Counter-Strike 2', 20.99, 1, 'A tactical first-person shooter featuring team-based gameplay.', 'https://sm.ign.com/ign_nordic/cover/c/counter-st/counter-strike-2_jc2d.jpg', 'https://www.youtube.com/watch?v=c80dVYcL69E', 'https://www.youtube.com/watch?v=P22HqM9w500', '8GB RAM, i5-2500K, GTX 660', '16GB RAM, i7-7700K, GTX 1060', 'Approved',50,NULL),
(4, 'Half-Life 2', 9.99, 1, 'A story-driven first-person shooter that revolutionized the genre.', 'https://media.moddb.com/images/mods/1/47/46951/d1jhx20-dc797b78-5feb-4005-b206-.1.jpg', 'https://www.youtube.com/watch?v=UKA7JkV51Jw', 'https://www.youtube.com/watch?v=jElU1mD8JnI', '512MB RAM, 1.7GHz Processor, DirectX 8 GPU', '1GB RAM, 3.0GHz Processor, DirectX 9 GPU', 'Approved',60,NULL),
(5, 'Mario', 59.99, 1, 'A classic platformer adventure with iconic characters and worlds.', 'https://play-lh.googleusercontent.com/3ZKfMRp_QrdN-LzsZTbXdXBH-LS1iykSg9ikNq_8T2ppc92ltNbFxS-tORxw2-6kGA', 'https://www.youtube.com/watch?v=TnGl01FkMMo', 'https://www.youtube.com/watch?v=rLl9XBg7wSs', 'N/A', 'N/A', 'Approved',70,NULL),
(6, 'The Legend of Zelda', 59.99, 1, 'An epic adventure game where heroes save the kingdom of Hyrule.', 'https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg', 'https://www.youtube.com/watch?v=_X2h3SF7gd4', 'https://www.youtube.com/watch?v=wW7jkBJ_yK0', 'N/A', 'N/A', 'Approved',30,NULL),
(7, 'Baba Is You', 14.99, 2, 'A puzzle game where you change the rules to solve challenges.', 'https://is5-ssl.mzstatic.com/image/thumb/Purple113/v4/9e/30/61/9e3061a5-b2f0-87ad-9e90-563f37729be5/source/256x256bb.jpg', 'https://www.youtube.com/watch?v=z3_yA4HTJfs', 'https://www.youtube.com/watch?v=dAiX8s-Eu7w', '2GB RAM, 1.0GHz Processor', '4GB RAM, 2.0GHz Processor', 'Pending',20,NULL),
(8, 'Portal 2', 9.99, 2, 'A mind-bending puzzle-platformer with a dark sense of humor.', 'https://steamuserimages-a.akamaihd.net/ugc/789750822988419716/AB9DA751B588ADA1597DB3318BFED932F994683F/?imw=5000&imh=5000&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=false', 'https://www.youtube.com/watch?v=tax4e4hBBZc', 'https://www.youtube.com/watch?v=ts-j0nFf2e0', '2GB RAM, 1.7GHz Processor, DirectX 9 GPU', '4GB RAM, 3.0GHz Processor, GTX 760', 'Pending',10,NULL),
(9, 'Outer Wilds', 24.99, 2, 'An exploration-based game where you unravel cosmic mysteries.', 'https://img-eshop.cdn.nintendo.net/i/bc850a322c0b2e2b410bf462993fffa602a32803eafd1805ef22774ac634c779.jpg', 'https://www.youtube.com/watch?v=d9u6KYVq5kw', 'https://www.youtube.com/watch?v=huL_TawYrMs', '6GB RAM, i5-2300, GTX 560', '8GB RAM, i7-6700, GTX 970', 'Pending',15,NULL),
(10, 'Minecraft', 29.99, 2, 'A sandbox game that lets you build and explore infinite worlds.', 'https://cdn2.steamgriddb.com/icon/f0b57183da91a7972b2b3c06b0db5542/32/512x512.png', 'https://www.youtube.com/watch?v=MmB9b5njVbA', 'https://www.youtube.com/watch?v=ANgI2o_Jinc', '4GB RAM, Intel HD 4000', '8GB RAM, GTX 1060', 'Pending',14,NULL),
(11, 'Subnautica', 29.99, 2, 'An underwater survival adventure set on an alien ocean planet.', 'https://www.nintendo.com/eu/media/images/11_square_images/games_18/nintendo_switch_5/SQ_NSwitch_Subnautica_image500w.jpg', 'https://www.youtube.com/watch?v=Rz2SNm8VguE', 'https://www.youtube.com/watch?v=diS8RCYwSCg', '8GB RAM, i5-4590, GTX 550', '16GB RAM, i7-7700K, GTX 1060', 'Pending',13,NULL),
(12, 'Space Invaders', 9.99, 2, 'A classic arcade shooter where you defend Earth from alien invaders.', 'https://static.wikia.nocookie.net/classics/images/a/a1/Space_Invaders_Logo.jpeg/revision/latest?cb=20210725054724', 'https://www.youtube.com/watch?v=5a2sRVL3fIY', 'https://www.youtube.com/watch?v=MU4psw3ccUI', '2GB RAM, 1.2GHz Processor', '4GB RAM, 2.4GHz Processor', 'Pending',13,NULL),
(13, 'Fantasy Quest', 29.99, 2, 'An epic adventure RPG set in a magical world.', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTPRuOKdjy9y8-lChDfNMqrurbqEBzs0oGYvw&s', 'https://www.youtube.com/watch?v=1_rYK41w988', 'https://www.youtube.com/watch?v=0Jt5ASpS-9w', '4GB RAM, 2.0GHz Processor', '8GB RAM, 3.0GHz Processor', 'Pending',20,NULL),
(14, 'Racing Turbo', 19.99, 2, 'A fast-paced racing game with stunning graphics and intense action.', 'https://play-lh.googleusercontent.com/QUM3PMNJvBPb0J5ovrt1WYefhq4ik3LNhIhBDWCSZ_qthzm5F7ODHqfkBoLVhiS0Rdau', 'https://www.youtube.com/watch?v=vyok9RcFBxI', 'https://www.youtube.com/watch?v=azWAsEuILlk&list=PLPrREYqN5Wqb1rEcMUsRNhpOGD74-fR8d', '2GB RAM, 1.5GHz Processor', '4GB RAM, 2.5GHz Processor', 'Pending',20,NULL);
GO

-- Insert tags into the tags table
INSERT INTO tags (tag_id, tag_name)
VALUES
    (1, 'Rogue-Like'),
    (2, 'Third-Person Shooter'),
    (3, 'Multiplayer'),
    (4, 'Horror'),
    (5, 'First-Person Shooter'),
    (6, 'Action'),
    (7, 'Platformer'),
    (8, 'Adventure'),
    (9, 'Puzzle'),
    (10, 'Exploration'),
    (11, 'Sandbox'),
    (12, 'Survival'),
    (13, 'Arcade'),
    (14, 'RPG'),
    (15, 'Racing');


-- Associate tags with games in the game_tags table
INSERT INTO game_tags (tag_id, game_id)
VALUES
    -- Risk of Rain 2
    (1, 1),
    (2, 1),
    (6, 1),
    -- Dead by Daylight
    (3, 2),
    (4, 2),
    (6, 2),
    -- Counter-Strike 2
    (3, 3),
    (5, 3),
    (6, 3),
    -- Half-Life 2
    (5, 4),
    (6, 4),
    (8, 4),
    -- Mario
    (7, 5),
    (6, 5),
    (8, 5),
    -- The Legend of Zelda
    (8, 6),
    (6, 6),
    (14, 6),
    -- Baba Is You
    (9, 7),
    (8, 7),
    -- Portal 2
    (9, 8),
    (5, 8),
    (6, 8),
    -- Outer Wilds
    (10, 9),
    (8, 9),
    -- Minecraft
    (11, 10),
    (12, 10),
    (8, 10),
    -- Subnautica
    (12, 11),
    (10, 11),
    (8, 11),
    -- Space Invaders
    (13, 12),
    (6, 12),
    -- Fantasy Quest
    (14, 13),
    (8, 13),
    -- Racing Turbo
    (15, 14),
    (6, 14);

INSERT INTO game_reviews (id, game_id, rating, comment, username) 
VALUES 
-- Risk of Rain 2
(1, 1, 4.5, 'Great rogue-like action!', 'gamer123'),
(2, 1, 4.0, 'Fun but tough learning curve.', 'roguelover'),
(3, 1, 4.7, 'Addictive gameplay!', 'space_runner'),
(4, 1, 4.6, 'Co-op is awesome!', 'teamplayer'),

-- Dead by Daylight
(5, 2, 4.8, 'Terrifying and thrilling!', 'horror_fan'),
(6, 2, 4.2, 'Best multiplayer horror game.', 'survivor_pro'),
(7, 2, 4.5, 'Killers are fun to play!', 'slasher_king'),
(8, 2, 4.3, 'Needs better matchmaking.', 'ghosted'),

-- Counter-Strike 2
(9, 3, 4.7, 'Tactical and competitive.', 'cs_master'),
(10, 3, 4.6, 'Improved graphics and gameplay.', 'fps_guy'),
(11, 3, 4.9, 'A must-play shooter!', 'headshot_ace'),

-- Half-Life 2
(12, 4, 5.0, 'One of the best FPS ever!', 'hl2_legend'),
(13, 4, 4.9, 'Story and gameplay are top-notch.', 'gamer_dude'),
(14, 4, 5.0, 'Gordon Freeman is iconic!', 'lambda_fan'),

-- Mario
(15, 5, 4.9, 'Mario never disappoints!', 'nintendo_fan'),
(16, 5, 4.7, 'Classic platformer fun.', 'retro_gamer'),
(17, 5, 5.0, 'A masterpiece!', 'jumpman'),
(18, 5, 4.6, 'Great for all ages.', 'family_gamer'),

-- The Legend of Zelda
(19, 6, 5.0, 'Epic adventure, must play!', 'zelda_fan'),
(20, 6, 4.8, 'Beautiful and engaging.', 'rpg_lover'),
(21, 6, 5.0, 'Breath of the Wild is GOAT.', 'hyrule_warrior'),
(22, 6, 4.7, 'Great puzzles and combat.', 'sword_master'),

-- Baba Is You
(23, 7, 4.5, 'Unique and clever puzzles.', 'puzzle_master'),
(24, 7, 4.3, 'Mind-bending gameplay.', 'indie_gamer'),
(25, 7, 4.6, 'Innovative mechanics.', 'logic_wiz'),

-- Portal 2
(26, 8, 4.9, 'Incredible puzzle design.', 'portal_lover'),
(27, 8, 4.8, 'Hilarious and challenging.', 'glados_fan'),
(28, 8, 5.0, 'Perfect co-op mode!', 'coop_champ'),
(29, 8, 4.7, 'Wish it was longer!', 'puzzle_freak'),

-- Outer Wilds
(30, 9, 4.7, 'Amazing exploration.', 'space_explorer'),
(31, 9, 4.6, 'A masterpiece of discovery.', 'curious_gamer'),
(32, 9, 4.8, 'Great music and story.', 'astro_wanderer'),

-- Minecraft
(33, 10, 5.0, 'Endless creativity!', 'block_builder'),
(34, 10, 4.8, 'Addictive and fun.', 'mine_crafter'),
(35, 10, 5.0, 'Survival mode is the best.', 'crafting_pro'),
(36, 10, 4.9, 'Best sandbox game ever.', 'pixel_adventurer'),

-- Subnautica
(37, 11, 4.7, 'Underwater survival at its best.', 'deep_diver'),
(38, 11, 4.6, 'Great atmosphere and exploration.', 'ocean_explorer'),
(39, 11, 4.9, 'Beautiful and immersive.', 'sea_survivor'),
(40, 11, 4.5, 'Scary deep-sea creatures!', 'aquatic_fear');

-- Delete existing transactions to avoid duplicates/issues
DELETE FROM store_transaction;
GO

-- Then insert mock transactions
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES 
-- Counter-Strike 2
(3, 1, GETDATE(), 19.99, 1),
(3, 2, DATEADD(DAY, -1, GETDATE()), 19.99, 1),
(3, 3, GETDATE(), 19.99, 1);
GO

-- Remainder of transactions to avoid overwhelming the insert
-- Insert more transaction data in smaller batches
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
(3, 4, GETDATE(), 19.99, 1),
(3, 5, GETDATE(), 19.99, 1),
(3, 6, DATEADD(DAY, -12, GETDATE()), 19.99, 1),
(3, 7, DATEADD(DAY, -15, GETDATE()), 19.99, 1);
GO

-- More transactions in batches
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
--Minecraft
(10, 1, GETDATE(), 29.99, 1),
(10, 2, GETDATE(), 29.99, 1),
(10, 3, DATEADD(DAY, -1, GETDATE()), 29.99, 1),
(10, 4, GETDATE(), 29.99, 1),
(10, 5, DATEADD(DAY, -2, GETDATE()), 29.99, 1),
(10, 6, DATEADD(DAY, -10, GETDATE()), 29.99, 1);
GO

--Dead by Daylight
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
(2, 1, GETDATE(), 29.99, 1),
(2, 2, DATEADD(DAY, -1, GETDATE()), 29.99, 1),
(2, 3, DATEADD(DAY, -2, GETDATE()), 29.99, 1),
(2, 4, GETDATE(), 29.99, 1),
(2, 5, GETDATE(), 29.99, 1),
(2, 6, DATEADD(DAY, -11, GETDATE()), 29.99, 1);
GO

-- Other games transactions - batch 1
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
-- Risk of Rain 2
(1, 1, DATEADD(DAY, -10, GETDATE()), 9.99, 1),
(1, 2, DATEADD(DAY, -12, GETDATE()), 9.99, 1),
(1, 3, GETDATE(), 9.99, 1),
--Half-Life 2
(4, 1, DATEADD(DAY, -20, GETDATE()), 39.99, 1),
(4, 2, DATEADD(DAY, -9, GETDATE()), 39.99, 1);
GO

-- Other games transactions - batch 2
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
-- The Legend of Zelda
(6, 1, DATEADD(DAY, -13, GETDATE()), 59.99, 1),
(6, 2, DATEADD(DAY, -8, GETDATE()), 59.99, 1),
-- Portal 2
(8, 1, GETDATE(), 9.99, 1),
(8, 2, DATEADD(DAY, -11, GETDATE()), 9.99, 1),
(8, 3, DATEADD(DAY, -14, GETDATE()), 9.99, 1);
GO

-- Other games transactions - batch 3
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
-- Baba Is You
(7, 1, DATEADD(DAY, -22, GETDATE()), 14.99, 1),
(7, 2, DATEADD(DAY, -9, GETDATE()), 14.99, 1),
-- Outer Wilds
(9, 1, DATEADD(DAY, -18, GETDATE()), 24.99, 1),
(9, 2, GETDATE(), 24.99, 1);
GO

-- Other games transactions - batch 4
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
-- Subnautica
(11, 1, DATEADD(DAY, -16, GETDATE()), 29.99, 1),
(11, 2, GETDATE(), 29.99, 1),
--Space Invaders
(12, 1, DATEADD(DAY, -19, GETDATE()), 9.99, 1),
(12, 2, DATEADD(DAY, -1, GETDATE()), 9.99, 1);
GO

-- Other games transactions - batch 5
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES
--  Fantasy Quest
(13, 1, DATEADD(DAY, -14, GETDATE()), 29.99, 1),
(13, 2, GETDATE(), 29.99, 1),
--  Racing Turbo
(14, 1, DATEADD(DAY, -11, GETDATE()), 19.99, 1),
(14, 2, DATEADD(DAY, -1, GETDATE()), 19.99, 1),
(14, 3, GETDATE(), 19.99, 1);
GO

-- Ensure user 1 has purchased at least 5 games
-- First check if entries already exist
IF NOT EXISTS (SELECT 1 FROM games_users WHERE user_id = 1 AND game_id = 1)
BEGIN
    INSERT INTO games_users (user_id, game_id, isInWishlist, is_purchased, isInCart)
    VALUES 
    (1, 1, 0, 0, 0),
    (1, 2, 0, 0, 0),
    (1, 3, 0, 0, 0),
    (1, 4, 0, 0, 0),
    (1, 5, 0, 0, 0);
END
GO

-- Add transactions for user 1 if they don't already exist
-- Delete old transactions to avoid foreign key issues
DELETE FROM store_transaction WHERE user_id = 1 AND game_id IN (1, 2, 3, 4, 5);

-- Now insert fresh transactions
INSERT INTO store_transaction (game_id, user_id, date, amount, withMoney)
VALUES 
(1, 1, GETDATE(), 9.99, 1),
(2, 1, GETDATE(), 29.99, 1),
(3, 1, GETDATE(), 19.99, 1),
(4, 1, GETDATE(), 39.99, 1),
(5, 1, GETDATE(), 24.99, 1);
GO

-- Drop existing Point Shop tables if they exist
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UserInventoryItems')
    DROP TABLE UserInventoryItems;
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PointShopItems')
    DROP TABLE PointShopItems;
GO

-- Create Point Shop Tables
CREATE TABLE PointShopItems (
    ItemId INT PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    ImagePath NVARCHAR(MAX),
    PointPrice DECIMAL(10, 2) NOT NULL,
    ItemType NVARCHAR(50) NOT NULL -- E.g., "ProfileBackground", "Avatar", "Emoticon", etc.
);

CREATE TABLE UserInventoryItems (
    UserId INT,
    ItemId INT,
    PurchaseDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 0,
    PRIMARY KEY (UserId, ItemId),
    FOREIGN KEY (ItemId) REFERENCES PointShopItems(ItemId)
);
GO

-- Drop existing Point Shop procedures if they exist
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetAllPointShopItems')
    DROP PROCEDURE GetAllPointShopItems;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetUserPointShopItems')
    DROP PROCEDURE GetUserPointShopItems;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PurchasePointShopItem')
    DROP PROCEDURE PurchasePointShopItem;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ActivatePointShopItem')
    DROP PROCEDURE ActivatePointShopItem;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DeactivatePointShopItem')
    DROP PROCEDURE DeactivatePointShopItem;
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'UpdateUserPointBalance')
    DROP PROCEDURE UpdateUserPointBalance;
GO

-- Create Point Shop stored procedures
CREATE PROCEDURE GetAllPointShopItems
AS
BEGIN
    SELECT * FROM PointShopItems;
END
GO

CREATE PROCEDURE GetUserPointShopItems
    @UserId INT
AS
BEGIN
    SELECT i.ItemId, i.Name, i.Description, i.ImagePath, i.PointPrice, i.ItemType, ui.IsActive
    FROM PointShopItems i
    INNER JOIN UserInventoryItems ui ON i.ItemId = ui.ItemId
    WHERE ui.UserId = @UserId;
END
GO

CREATE PROCEDURE PurchasePointShopItem
    @UserId INT,
    @ItemId INT
AS
BEGIN
    -- Check if user already owns this item
    IF EXISTS (SELECT 1 FROM UserInventoryItems WHERE UserId = @UserId AND ItemId = @ItemId)
    BEGIN
        RAISERROR('User already owns this item', 16, 1);
        RETURN;
    END

    -- Insert the item into user's inventory
    INSERT INTO UserInventoryItems (UserId, ItemId, PurchaseDate, IsActive)
    VALUES (@UserId, @ItemId, GETDATE(), 0);
END
GO

CREATE PROCEDURE ActivatePointShopItem
    @UserId INT,
    @ItemId INT
AS
BEGIN
    DECLARE @ItemType NVARCHAR(50);
    
    -- Get the type of the item being activated
    SELECT @ItemType = ItemType 
    FROM PointShopItems 
    WHERE ItemId = @ItemId;
    
    -- Deactivate all other items of the same type for this user
    UPDATE UserInventoryItems
    SET IsActive = 0
    FROM UserInventoryItems ui
    INNER JOIN PointShopItems i ON ui.ItemId = i.ItemId
    WHERE ui.UserId = @UserId 
    AND i.ItemType = @ItemType;
    
    -- Activate the selected item
    UPDATE UserInventoryItems
    SET IsActive = 1
    WHERE UserId = @UserId AND ItemId = @ItemId;
END
GO

CREATE PROCEDURE DeactivatePointShopItem
    @UserId INT,
    @ItemId INT
AS
BEGIN
    UPDATE UserInventoryItems
    SET IsActive = 0
    WHERE UserId = @UserId AND ItemId = @ItemId;
END
GO

CREATE PROCEDURE UpdateUserPointBalance
    @UserId INT,
    @PointBalance DECIMAL(10, 2)
AS
BEGIN
    UPDATE users
    SET point_balance = @PointBalance
    WHERE user_id = @UserId;
END
GO

-- Insert sample data for Point Shop
INSERT INTO PointShopItems (ItemId, Name, Description, ImagePath, PointPrice, ItemType)
VALUES
(1, 'Blue Profile Background', 'A cool blue background for your profile', 'https://picsum.photos/id/1/200/200', 1000, 'ProfileBackground'),
(2, 'Red Profile Background', 'A vibrant red background for your profile', 'https://picsum.photos/id/20/200/200', 1000, 'ProfileBackground'),
(3, 'Golden Avatar Frame', 'A golden frame for your avatar image', 'https://picsum.photos/id/30/200/200', 2000, 'AvatarFrame'),
(4, 'Silver Avatar Frame', 'A silver frame for your avatar image', 'https://picsum.photos/id/40/200/200', 1500, 'AvatarFrame'),
(5, 'Happy Emoticon', 'Express yourself with this happy emoticon', 'https://picsum.photos/id/50/200/200', 500, 'Emoticon'),
(6, 'Sad Emoticon', 'Express yourself with this sad emoticon', 'https://picsum.photos/id/60/200/200', 500, 'Emoticon'),
(7, 'Gamer Avatar', 'Cool gamer avatar for your profile', 'https://picsum.photos/id/70/200/200', 1200, 'Avatar'),
(8, 'Ninja Avatar', 'Stealthy ninja avatar for your profile', 'https://picsum.photos/id/80/200/200', 1200, 'Avatar'),
(9, 'Space Mini-Profile', 'Space-themed mini profile', 'https://picsum.photos/id/90/200/200', 3000, 'MiniProfile'),
(10, 'Fantasy Mini-Profile', 'Fantasy-themed mini profile', 'https://picsum.photos/id/100/200/200', 3000, 'MiniProfile');

-- Add some items to test user's inventory
-- First check if these items already exist
IF NOT EXISTS (SELECT 1 FROM UserInventoryItems WHERE UserId = 1 AND ItemId = 1)
BEGIN
    INSERT INTO UserInventoryItems (UserId, ItemId, PurchaseDate, IsActive)
    VALUES
    (1, 1, GETDATE(), 1),  -- Blue background (active)
    (1, 3, GETDATE(), 0),  -- Golden frame (inactive)
    (1, 5, GETDATE(), 1);  -- Happy emoticon (active)
END
ELSE
BEGIN
    -- Reset user inventory to initial state on each script run
    -- Delete any newly purchased items and keep only the initial items
    DELETE FROM UserInventoryItems 
    WHERE UserId = 1 
    AND ItemId NOT IN (1, 3, 5);
    
    -- Reset the activation status
    UPDATE UserInventoryItems SET IsActive = 1 WHERE UserId = 1 AND ItemId = 1;
    UPDATE UserInventoryItems SET IsActive = 0 WHERE UserId = 1 AND ItemId = 3;
    UPDATE UserInventoryItems SET IsActive = 1 WHERE UserId = 1 AND ItemId = 5;
END

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ResetUserInventoryToDefault')
    DROP PROCEDURE ResetUserInventoryToDefault;
Go

CREATE PROCEDURE ResetUserInventoryToDefault
    @UserId INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM UserInventoryItems 
        WHERE UserId = @UserId AND ItemId = 1
    )
    BEGIN
        INSERT INTO UserInventoryItems (UserId, ItemId, PurchaseDate, IsActive)
        VALUES
            (@UserId, 1, GETDATE(), 1),  -- Blue background
            (@UserId, 3, GETDATE(), 0),  -- Golden frame
            (@UserId, 5, GETDATE(), 1);  -- Happy emoticon
    END
    ELSE
    BEGIN
        DELETE FROM UserInventoryItems 
        WHERE UserId = @UserId AND ItemId NOT IN (1, 3, 5);

        UPDATE UserInventoryItems SET IsActive = 1 WHERE UserId = @UserId AND ItemId = 1;
        UPDATE UserInventoryItems SET IsActive = 0 WHERE UserId = @UserId AND ItemId = 3;
        UPDATE UserInventoryItems SET IsActive = 1 WHERE UserId = @UserId AND ItemId = 5;
    END
END

--EXEC GetAllUnvalidated @publisher_id = "1";

--EXEC ValidateGame @game_id = "7";

--EXEC GetAllUnvalidated @publisher_id = "2";

--SELECT * FROM games;
--EXEC getGameTags @gid = "133";
--EXEC getGameTags @gid = "4142";

--EXEC ValidateGame @game_id = "45";

--EXEC getAllTags;
--EXEC RejectGameWithMessage @game_id = "", @rejection_message = "Test test test";

