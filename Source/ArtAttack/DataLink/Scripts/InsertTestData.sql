USE SteampunksDB;
GO

DELETE FROM UserInventory;
DELETE FROM Items;
DELETE FROM Games;
DELETE FROM Users;
GO

DBCC CHECKIDENT ('Items', RESEED, 0);
DBCC CHECKIDENT ('Games', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
GO

-- Insert test users
INSERT INTO Users (UserName, WalletBalance, PointBalance, IsDeveloper)
VALUES 
('TestUser1', 1000, 100, 0),
('TestUser2', 1000, 100, 0),
('TestUser3', 1000, 100, 0);
GO

-- Insert games
INSERT INTO Games (GameTitle, Price, Genre, GameDescription, Status)
VALUES 
('Counter-Strike 2', 0.00, 'FPS', 'The latest version of Counter-Strike', 'Available'),
('Dota 2', 0.00, 'MOBA', 'A competitive multiplayer game', 'Available'),
('Team Fortress 2', 0.00, 'FPS', 'A team-based multiplayer game', 'Available');
GO

-- Insert items
INSERT INTO Items (ItemName, CorrespondingGameId, Price, Description, IsListed)
VALUES 
-- CS2 Items
('AK-47 | Asiimov', 1, 150.00, 'A rare and valuable AK-47 skin with a futuristic design', 0),
('M4A4 | Howl', 1, 1000.00, 'One of the most expensive and sought-after CS2 skins', 0),
('AWP | Dragon Lore', 1, 2000.00, 'A legendary AWP skin with a dragon design', 0),
('Karambit | Fade', 1, 800.00, 'A beautiful karambit knife with a fade pattern', 0),
('M4A1-S | Knight', 1, 600.00, 'A rare M4A1-S skin with a knight theme', 0),

-- Dota 2 Items
('Dragonclaw Hook', 2, 1200.00, 'A legendary Pudge hook with dragon design', 0),
('Timebreaker', 2, 800.00, 'A rare Faceless Void weapon', 0),
('Golden Baby Roshan', 2, 1500.00, 'A rare courier with golden design', 0),
('Inscribed Golden Baby Roshan', 2, 2000.00, 'An even rarer version of the Golden Baby Roshan', 0),
('Dragonclaw Hook (Unusual)', 2, 2000.00, 'A special version of the Dragonclaw Hook with particle effects', 0),

-- TF2 Items
('Unusual Team Captain', 3, 1000.00, 'A rare hat with particle effects', 0),
('Burning Flames Modest Pile of Hat', 3, 2000.00, 'One of the most valuable TF2 hats', 0),
('Earbuds', 3, 400.00, 'A classic TF2 cosmetic item', 0),
('Unusual Burning Flames Team Captain', 3, 3000.00, 'A combination of two rare items', 0),
('Unusual Scorching Flames Team Captain', 3, 2500.00, 'Another rare variant of the Team Captain', 0);
GO

-- Insert mock items into UserInventory
INSERT INTO UserInventory (UserId, ItemId, GameId, AcquiredDate)
VALUES 
-- TestUser1's inventory
(1, 1, 1, GETDATE()),  -- AK-47 | Asiimov
(1, 2, 1, GETDATE()),  -- M4A4 | Howl
(1, 3, 1, GETDATE()),  -- AWP | Dragon Lore
(1, 6, 2, GETDATE()),  -- Dragonclaw Hook
(1, 7, 2, GETDATE()),  -- Timebreaker

-- TestUser2's inventory
(2, 11, 3, GETDATE()),  -- Unusual Team Captain
(2, 12, 3, GETDATE()),  -- Burning Flames Modest Pile of Hat
(2, 13, 3, GETDATE()),  -- Earbuds
(2, 4, 1, GETDATE()),   -- Karambit | Fade
(2, 5, 1, GETDATE()),   -- M4A1-S | Knight

-- TestUser3's inventory
(3, 1, 1, GETDATE()),   -- AK-47 | Asiimov
(3, 6, 2, GETDATE()),   -- Dragonclaw Hook
(3, 11, 3, GETDATE()),  -- Unusual Team Captain
(3, 4, 1, GETDATE()),   -- Karambit | Fade
(3, 5, 1, GETDATE());   -- M4A1-S | Knight
GO