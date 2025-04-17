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