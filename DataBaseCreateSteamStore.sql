
USE SteamStore


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

CREATE TABLE games (
    game_id INT PRIMARY KEY,
    name NVARCHAR(255),
    price DECIMAL(10,2),
    publisher_id INT FOREIGN KEY REFERENCES users(user_id),
    description NVARCHAR(MAX),
    image_url NVARCHAR(MAX),
	minimum_requirements NVARCHAR(MAX),
	recommended_requirements NVARCHAR(MAX),
	status NVARCHAR(MAX)
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

INSERT INTO users (user_id, username, balance, point_balance, is_developer)
VALUES 
(1, 'john_doe', 100.00, 500.00, 0),
(2, 'jane_smith', 150.00, 300.00, 1),
(3, 'alex_brown', 50.00, 150.00, 0),
(4, 'mary_jones', 200.00, 1000.00, 1);
GO

INSERT INTO games (game_id, name, price, publisher_id, description, image_url, minimum_requirements, recommended_requirements, status)
VALUES 
(1, 'Space Invaders', 9.99, 1, 'A classic arcade shooter where you defend Earth from alien invaders.', 'https://example.com/space_invaders.jpg', '2GB RAM, 1.2GHz Processor', '4GB RAM, 2.4GHz Processor', 'Available'),
(2, 'Fantasy Quest', 29.99, 2, 'An epic adventure RPG set in a magical world.', 'https://example.com/fantasy_quest.jpg', '4GB RAM, 2.0GHz Processor', '8GB RAM, 3.0GHz Processor', 'Available'),
(3, 'Racing Turbo', 19.99, 1, 'A fast-paced racing game with stunning graphics and intense action.', 'https://example.com/racing_turbo.jpg', '2GB RAM, 1.5GHz Processor', '4GB RAM, 2.5GHz Processor', 'Unavailable');
GO
