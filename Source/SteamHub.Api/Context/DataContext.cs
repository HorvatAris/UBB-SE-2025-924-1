namespace SteamHub.Api.Context
{
    using Azure;
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;
    using Models;
    using System.Reflection.Emit;
    using SteamHub.ApiContract.Models.Game;
    using Game = SteamHub.Api.Entities.Game;


    public class DataContext : DbContext
    {
        private readonly IConfiguration configuration;

        public DataContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        public DataContext()
        {
        }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PointShopItem> PointShopItems { get; set; }

        public DbSet<UserPointShopItemInventory> UserPointShopInventories { get; set; }

        public DbSet<UsersGames> UsersGames { get; set; }
        public DbSet<StoreTransaction> StoreTransactions { get; set; }
        public DbSet<ItemTrade> ItemTrades { get; set; }
        public DbSet<UserInventory> UserInventories { get; set; }
        public DbSet<ItemTradeDetail> ItemTradeDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));
            }
        }  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Role>()
                .Property(role => role.Id).ValueGeneratedNever();

            builder.Entity<Role>().HasData(Enum.GetValues(typeof(RoleEnum))
                .Cast<RoleEnum>()
                .Select(role => new Role
                {
                    Id = role,
                    Name = role.ToString()
                }));

            var tagSeed = new List<Tag>
            {
                new Tag { TagId = 1, TagName = "Rogue-Like" },
                new Tag { TagId = 2, TagName = "Third-Person Shooter" },
                new Tag { TagId = 3, TagName = "Multiplayer" },
                new Tag { TagId = 4, TagName = "Horror" },
                new Tag { TagId = 5, TagName = "First-Person Shooter" },
                new Tag { TagId = 6, TagName = "Action" },
                new Tag { TagId = 7, TagName = "Platformer" },
                new Tag { TagId = 8, TagName = "Adventure" },
                new Tag { TagId = 9, TagName = "Puzzle" },
                new Tag { TagId = 10, TagName = "Exploration" },
                new Tag { TagId = 11, TagName = "Sandbox" },
                new Tag { TagId = 12, TagName = "Survival" },
                new Tag { TagId = 13, TagName = "Arcade" },
                new Tag { TagId = 14, TagName = "RPG" },
                new Tag { TagId = 15, TagName = "Racing" },
                new Tag { TagId = 16, TagName = "Action RPG" },
                new Tag { TagId = 17, TagName = "Battle Royale" },
            };

            var usersSeed = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Email = "gabe.newell@valvestudio.com",
                    PointsBalance = 6000,
                    UserName = "GabeN",
                    RoleId = RoleEnum.Developer,
                    WalletBalance = 500
                },
                new User
                {
                    UserId = 2,
                    Email = "mathias.new@cdprojektred.com",
                    PointsBalance = 5000,
                    UserName = "MattN",
                    RoleId = RoleEnum.Developer,
                    WalletBalance = 420
                },
                new User
                {
                    UserId = 3,
                    Email = "john.chen@thatgamecompany.com",
                    PointsBalance = 5000,
                    UserName = "JohnC",
                    RoleId = RoleEnum.Developer,
                    WalletBalance = 390
                },
                new User
                {
                    UserId = 4,
                    Email = "alice.johnson@example.com",
                    PointsBalance = 6000,
                    UserName = "AliceJ",
                    RoleId = RoleEnum.User,
                    WalletBalance = 780
                },
                new User
                {
                    UserId = 5,
                    Email = "liam.garcia@example.com",
                    PointsBalance = 7000,
                    UserName = "LiamG",
                    RoleId = RoleEnum.User,
                    WalletBalance = 5500
                },
                new User
                {
                    UserId = 6,
                    Email = "sophie.williams@example.com",
                    PointsBalance = 6000,
                    UserName = "SophieW",
                    RoleId = RoleEnum.User,
                    WalletBalance = 950
                },
                new User
                {
                    UserId = 7,
                    Email = "noah.smith@example.com",
                    PointsBalance = 4000,
                    UserName = "NoahS",
                    RoleId = RoleEnum.User,
                    WalletBalance = 3300
                },
                new User
                {
                    UserId = 8,
                    Email = "emily.brown@example.com",
                    PointsBalance = 5000,
                    UserName = "EmilyB",
                    RoleId = RoleEnum.User,
                    WalletBalance = 1100
                }
            };

            builder.Entity<Tag>().HasData(tagSeed);
            builder.Entity<User>().HasData(usersSeed);

            builder.Entity<GameStatus>()
                .Property(gameStatus => gameStatus.Id).ValueGeneratedNever();

            builder.Entity<GameStatus>().HasData(Enum.GetValues(typeof(GameStatusEnum))
                .Cast<GameStatusEnum>()
                .Select(gameStatus => new GameStatus
                {
                    Id = gameStatus,
                    Name = gameStatus.ToString()
                }));

            var gameSeed = new List<Game>
            {
                new Game
                {
                    GameId = 1,
                    Name = "Risk of Rain 2",
                    Description = "A rogue-like third-person shooter where players fight through hordes of monsters to escape an alien planet.",
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/c/c1/Risk_of_Rain_2.jpg",
                    Price = 24.99m,
                    MinimumRequirements = "4GB RAM, 2.5GHz Processor, GTX 580",
                    RecommendedRequirements = "8GB RAM, 3.0GHz Processor, GTX 680",
                    StatusId = GameStatusEnum.Rejected,
                    RejectMessage = "Minimum requirements are too high",
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=pJ-aR--gScM",
                    GameplayPath = "https://www.youtube.com/watch?v=Cwk3qmD28CE",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 2,
                    Name = "Dead by Daylight",
                    Description = "A multiplayer horror game where survivors must evade a killer.",
                    ImagePath = "https://pbs.twimg.com/media/FOEzJiXX0AcxBTi.jpg",
                    Price = 19.99m,
                    MinimumRequirements = "8GB RAM, i3-4170, GTX 760",
                    RecommendedRequirements = "16GB RAM, i5-6500, GTX 1060",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=JGhIXLO3ul8",
                    GameplayPath = "https://www.youtube.com/watch?v=3wUHKO0ieyY",
                    Discount = 0.40m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 3,
                    Name = "Counter-Strike 2",
                    Description = "A tactical first-person shooter featuring team-based gameplay.",
                    ImagePath = "https://sm.ign.com/ign_nordic/cover/c/counter-st/counter-strike-2_jc2d.jpg",
                    Price = 20.99m,
                    MinimumRequirements = "8GB RAM, i5-2500K, GTX 660",
                    RecommendedRequirements = "16GB RAM, i7-7700K, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.9m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=c80dVYcL69E",
                    GameplayPath = "https://www.youtube.com/watch?v=P22HqM9w500",
                    Discount = 0.50m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 4,
                    Name = "Half-Life 2",
                    Description = "A story-driven first-person shooter that revolutionized the genre.",
                    ImagePath = "https://media.moddb.com/images/mods/1/47/46951/d1jhx20-dc797b78-5feb-4005-b206-.1.jpg",
                    Price = 9.99m,
                    MinimumRequirements = "512MB RAM, 1.7GHz Processor, DirectX 8 GPU",
                    RecommendedRequirements = "1GB RAM, 3.0GHz Processor, DirectX 9 GPU",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.1m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=UKA7JkV51Jw",
                    GameplayPath = "https://www.youtube.com/watch?v=jElU1mD8JnI",
                    Discount = 0.60m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 5,
                    Name = "Mario",
                    Description = "A classic platformer adventure with iconic characters and worlds.",
                    ImagePath = "https://play-lh.googleusercontent.com/3ZKfMRp_QrdN-LzsZTbXdXBH-LS1iykSg9ikNq_8T2ppc92ltNbFxS-tORxw2-6kGA",
                    Price = 59.99m,
                    MinimumRequirements = "N/A",
                    RecommendedRequirements = "N/A",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 5.0m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=TnGl01FkMMo",
                    GameplayPath = "https://www.youtube.com/watch?v=rLl9XBg7wSs",
                    Discount = 0.70m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 6,
                    Name = "The Legend of Zelda",
                    Description = "An epic adventure game where heroes save the kingdom of Hyrule.",
                    ImagePath = "https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg",
                    Price = 59.99m,
                    MinimumRequirements = "N/A",
                    RecommendedRequirements = "N/A",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.5m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=_X2h3SF7gd4",
                    GameplayPath = "https://www.youtube.com/watch?v=wW7jkBJ_yK0",
                    Discount = 0.30m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 7,
                    Name = "Baba Is You",
                    Description = "A puzzle game where you change the rules to solve challenges.",
                    ImagePath = "https://is5-ssl.mzstatic.com/image/thumb/Purple113/v4/9e/30/61/9e3061a5-b2f0-87ad-9e90-563f37729be5/source/256x256bb.jpg",
                    Price = 14.99m,
                    MinimumRequirements = "2GB RAM, 1.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 2.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 3.9m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=z3_yA4HTJfs",
                    GameplayPath = "https://www.youtube.com/watch?v=dAiX8s-Eu7w",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 8,
                    Name = "Portal 2",
                    Description = "A mind-bending puzzle-platformer with a dark sense of humor.",
                    ImagePath = "https://cdn2.steamgriddb.com/icon_thumb/0994c8d1d6bc62cc56e9112d2303266b.png",
                    Price = 9.99m,
                    MinimumRequirements = "2GB RAM, 1.7GHz Processor, DirectX 9 GPU",
                    RecommendedRequirements = "4GB RAM, 3.0GHz Processor, GTX 760",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=tax4e4hBBZc",
                    GameplayPath = "https://www.youtube.com/watch?v=ts-j0nFf2e0",
                    Discount = 0.10m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 9,
                    Name = "Outer Wilds",
                    Description = "An exploration-based game where you unravel cosmic mysteries.",
                    ImagePath = "https://images.nintendolife.com/62a79995ed766/outer-wilds-echoes-of-the-eye-cover.cover_large.jpg",
                    Price = 24.99m,
                    MinimumRequirements = "6GB RAM, i5-2300, GTX 560",
                    RecommendedRequirements = "8GB RAM, i7-6700, GTX 970",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=d9u6KYVq5kw",
                    GameplayPath = "https://www.youtube.com/watch?v=huL_TawYrMs",
                    Discount = 0.15m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 10,
                    Name = "Hades",
                    Description = "A rogue-like dungeon crawler where you defy the god of the dead.",
                    ImagePath = "https://image.api.playstation.com/vulcan/ap/rnd/202104/0517/9AcM3vy5t77zPiJyKHwRfnNT.png",
                    Price = 24.99m,
                    MinimumRequirements = "4GB RAM, Dual Core 2.4GHz, Intel HD 5000",
                    RecommendedRequirements = "8GB RAM, Dual Core 3.0GHz, GTX 760",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.0m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=91sW0DMkZzI",
                    GameplayPath = "https://www.youtube.com/watch?v=4fVO0qUBe4E",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 11,
                    Name = "Slay the Spire",
                    Description = "A deck-building rogue-like where strategy is key to survival.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP3717/CUSA15338_00/Sn5xbNutqfQdWYIjbeCIN0bwTJOV7UPG.png",
                    Price = 19.99m,
                    MinimumRequirements = "2GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 3.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.0m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=75qT5KOs-Ew",
                    GameplayPath = "https://www.youtube.com/watch?v=JO3EIPtw-4I",
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 12,
                    Name = "Celeste",
                    Description = "A platformer about climbing a mountain and facing inner demons.",
                    ImagePath = "https://images.nintendolife.com/ef02c2e24c59e/celeste-cover.cover_large.jpg",
                    Price = 19.99m,
                    MinimumRequirements = "2GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 2.4GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 3.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=iofYDsP2vhQ",
                    GameplayPath = "https://www.youtube.com/watch?v=FfRjHZWSYqY",
                    Discount = 0.30m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 13,
                    Name = "Hollow Knight",
                    Description = "An action-adventure game set in a beautifully animated underground world.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP1805/CUSA13285_00/DmwPWlU0468FbsjrtI92FhQz1xBYMoog.png",
                    Price = 14.99m,
                    MinimumRequirements = "4GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "8GB RAM, 3.2GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.1m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=UAO2urG23S4",
                    GameplayPath = "https://www.youtube.com/watch?v=UAO2urG23S4",
                    Discount = 0.35m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 14,
                    Name = "Stardew Valley",
                    Description = "A farming simulator RPG where you build a life in the countryside.",
                    ImagePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQHWjybGuWhdyJqjmtziGvtHvCnQf23yY0R6g&s",
                    Price = 14.99m,
                    MinimumRequirements = "2GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 3.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.1m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=ot7uXNQskhs",
                    GameplayPath = "https://www.youtube.com/watch?v=ot7uXNQskhs",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 15,
                    Name = "Minecraft",
                    Description = "A sandbox game that lets you build and explore infinite worlds.",
                    ImagePath = "https://cdn2.steamgriddb.com/icon/f0b57183da91a7972b2b3c06b0db5542/32/512x512.png",
                    Price = 29.99m,
                    MinimumRequirements = "4GB RAM, Intel HD 4000",
                    RecommendedRequirements = "8GB RAM, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 1420,
                    TrailerPath = "https://www.youtube.com/watch?v=MmB9b5njVbA",
                    GameplayPath = "https://www.youtube.com/watch?v=ANgI2o_Jinc",
                    Discount = 0.14m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 16,
                    Name = "Don't Starve",
                    Description = "A survival game in a dark and whimsical world filled with strange creatures.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP2107/CUSA00327_00/i5qwqMWJj33IIr2m9TM29GQNnFCi4ZqI.png?w=440",
                    Price = 9.99m,
                    MinimumRequirements = "1.7GHz Processor, 1GB RAM",
                    RecommendedRequirements = "2.0GHz Processor, 2GB RAM",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 3.6m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=ochPlhMFk84",
                    GameplayPath = "https://www.youtube.com/watch?v=htXgxyLpPMg",
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 17,
                    Name = "Cuphead",
                    Description = "A classic run and gun game with hand-drawn animations and tough bosses.",
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/thumb/e/eb/Cuphead_%28artwork%29.png/250px-Cuphead_%28artwork%29.png",
                    Price = 19.99m,
                    MinimumRequirements = "3GB RAM, Intel Core2 Duo E8400",
                    RecommendedRequirements = "4GB RAM, i3-3240",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=NN-9SQXoi50",
                    GameplayPath = "https://www.youtube.com/watch?v=DNIMD8ZpMSQ",
                    Discount = 0.40m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 18,
                    Name = "Limbo",
                    Description = "A black-and-white puzzle platformer with a haunting atmosphere.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP2054/CUSA01369_00/W45kellY9yrwSDpmQEL9tFqZQW7N4FEz.png?w=440",
                    Price = 9.99m,
                    MinimumRequirements = "512MB RAM, 1.5GHz Processor",
                    RecommendedRequirements = "2GB RAM, 2.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.6m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=Y4HSyVXKYz8",
                    GameplayPath = "https://www.youtube.com/watch?v=dYeuLZY7fZk",
                    Discount = 0.30m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 19,
                    Name = "Cyberstrike 2077",
                    Description = "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.",
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg",
                    Price = 59.99m,
                    MinimumRequirements = "Intel i5-3570K, 8GB RAM, GTX 780",
                    RecommendedRequirements = "Intel i7-4790, 12GB RAM, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 950,
                    TrailerPath = "https://www.youtube.com/watch?v=FknHjl7eQ6o",
                    GameplayPath = "https://www.youtube.com/watch?v=8X2kIfS6fb8",
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[2].UserId
                },
                new Game
                {
                    GameId = 20,
                    Name = "Shadow of Valhalla",
                    Description = "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.",
                    ImagePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQDtZKyDW9Jrnh8ix-Y38qG5fddbUgYEW7yxA&s",
                    Price = 44.99m,
                    MinimumRequirements = "Intel i5-4460, 8GB RAM, GTX 960",
                    RecommendedRequirements = "Intel i7-6700K, 16GB RAM, GTX 1080",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.5m,
                    NumberOfRecentPurchases = 780,
                    TrailerPath = "https://www.youtube.com/watch?v=ssrNcwxALS4",
                    GameplayPath = "https://www.youtube.com/watch?v=gncB1_e9n8E",
                    Discount = 0.10m,
                    PublisherUserId = usersSeed[2].UserId
                }
            };

            builder.Entity<Game>().HasData(gameSeed);

            builder.Entity<Game>()
                .HasMany(game => game.Tags)
                .WithMany(tag => tag.Games)
                .UsingEntity<Dictionary<string, object>>("GameTag",
                    x => x.HasData(
                            // Risk of Rain 2
                            new { GamesGameId = gameSeed[0].GameId, TagsTagId = tagSeed[0].TagId }, // Rogue-Like
                            new { GamesGameId = gameSeed[0].GameId, TagsTagId = tagSeed[1].TagId }, // Third-Person Shooter

                            // Dead by Daylight
                            new { GamesGameId = gameSeed[1].GameId, TagsTagId = tagSeed[2].TagId }, // Multiplayer
                            new { GamesGameId = gameSeed[1].GameId, TagsTagId = tagSeed[3].TagId }, // Horror

                            // Counter-Strike 2
                            new { GamesGameId = gameSeed[2].GameId, TagsTagId = tagSeed[2].TagId }, // Multiplayer
                            new { GamesGameId = gameSeed[2].GameId, TagsTagId = tagSeed[4].TagId }, // FPS
                            new { GamesGameId = gameSeed[2].GameId, TagsTagId = tagSeed[5].TagId }, // Action

                            // Half-Life 2
                            new { GamesGameId = gameSeed[3].GameId, TagsTagId = tagSeed[4].TagId }, // FPS
                            new { GamesGameId = gameSeed[3].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Mario
                            new { GamesGameId = gameSeed[4].GameId, TagsTagId = tagSeed[6].TagId }, // Platformer
                            new { GamesGameId = gameSeed[4].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Zelda
                            new { GamesGameId = gameSeed[5].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure
                            new { GamesGameId = gameSeed[5].GameId, TagsTagId = tagSeed[13].TagId }, // RPG

                            // Baba Is You
                            new { GamesGameId = gameSeed[6].GameId, TagsTagId = tagSeed[8].TagId }, // Puzzle

                            // Portal 2
                            new { GamesGameId = gameSeed[7].GameId, TagsTagId = tagSeed[8].TagId }, // Puzzle
                            new { GamesGameId = gameSeed[7].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Outer Wilds
                            new { GamesGameId = gameSeed[8].GameId, TagsTagId = tagSeed[9].TagId }, // Exploration
                            new { GamesGameId = gameSeed[8].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Hades
                            new { GamesGameId = gameSeed[9].GameId, TagsTagId = tagSeed[5].TagId }, // Action
                            new { GamesGameId = gameSeed[9].GameId, TagsTagId = tagSeed[0].TagId }, // Rogue-Like

                            // Slay the Spire
                            new { GamesGameId = gameSeed[10].GameId, TagsTagId = tagSeed[0].TagId }, // Rogue-Like
                            new { GamesGameId = gameSeed[10].GameId, TagsTagId = tagSeed[8].TagId }, // Puzzle

                            // Celeste
                            new { GamesGameId = gameSeed[11].GameId, TagsTagId = tagSeed[6].TagId }, // Platformer

                            // Hollow Knight
                            new { GamesGameId = gameSeed[12].GameId, TagsTagId = tagSeed[5].TagId },  // Action
                            new { GamesGameId = gameSeed[12].GameId, TagsTagId = tagSeed[6].TagId },  // Platformer

                            // Stardew Valley
                            new { GamesGameId = gameSeed[13].GameId, TagsTagId = tagSeed[11].TagId }, // Survival
                            new { GamesGameId = gameSeed[13].GameId, TagsTagId = tagSeed[7].TagId },  // Adventure

                            // Minecraft
                            new { GamesGameId = gameSeed[14].GameId, TagsTagId = tagSeed[10].TagId }, // Sandbox
                            new { GamesGameId = gameSeed[14].GameId, TagsTagId = tagSeed[11].TagId }, // Survival

                            // Don't Starve
                            new { GamesGameId = gameSeed[15].GameId, TagsTagId = tagSeed[11].TagId }, // Survival
                            new { GamesGameId = gameSeed[15].GameId, TagsTagId = tagSeed[7].TagId },  // Adventure

                            // Cuphead
                            new { GamesGameId = gameSeed[16].GameId, TagsTagId = tagSeed[5].TagId },  // Action
                            new { GamesGameId = gameSeed[16].GameId, TagsTagId = tagSeed[6].TagId },  // Platformer

                            // Limbo
                            new { GamesGameId = gameSeed[17].GameId, TagsTagId = tagSeed[8].TagId },  // Puzzle
                            new { GamesGameId = gameSeed[17].GameId, TagsTagId = tagSeed[6].TagId },  // Platformer

                            // Cyberstrike 2077
                            new { GamesGameId = gameSeed[18].GameId, TagsTagId = tagSeed[5].TagId },   // Action
                            new { GamesGameId = gameSeed[18].GameId, TagsTagId = tagSeed[13].TagId },  // RPG
                            new { GamesGameId = gameSeed[18].GameId, TagsTagId = tagSeed[15].TagId },  // Action RPG
    
                            // Shadow of Valhalla
                            new { GamesGameId = gameSeed[19].GameId, TagsTagId = tagSeed[15].TagId }  // Action RPG
                        ));

            builder.Entity<Item>()
                .HasOne(item => item.Game)
                .WithMany(game => game.Items)
                .HasForeignKey(item => item.CorrespondingGameId);
            
            var itemsSeed =  new List<Item>
            {
                // Items for Counter-Strike 2
                new Item
                {
                    ItemId = 1,
                    ItemName = "AK-47 | Redline Skin",
                    CorrespondingGameId = 3,
                    Price = 29.99f,
                    Description = "A sleek and aggressive finish for your AK-47.",
                    IsListed = false,
                    ImagePath = "https://steamcdn-a.akamaihd.net/apps/730/icons/econ/default_generated/weapon_ak47_cu_ak47_cobra_light_large.7494bfdf4855fd4e6a2dbd983ed0a243c80ef830.png"
                },
                new Item
                {
                    ItemId = 2,
                    ItemName = "Desert Eagle | Blaze Skin",
                    CorrespondingGameId = 3,
                    Price = 34.99f,
                    Description = "Legendary pistol skin with a fiery design.",
                    IsListed = false,
                    ImagePath = "https://steamcdn-a.akamaihd.net/apps/730/icons/econ/default_generated/weapon_deagle_aa_flames_light_large.dd140c3b359c16ccd8e918ca6ad0b2628151fe1c.png"
                },

                // Items for Half-Life 2
                new Item
                {
                    ItemId = 3,
                    ItemName = "Gravity Gun Replica",
                    CorrespondingGameId = 4,
                    Price = 49.99f,
                    Description = "Iconic weapon that manipulates objects with physics.",
                    IsListed = false,
                    ImagePath = "https://www.toyark.com/wp-content/uploads/2013/05/Half-Life-2-Gravity-Gun-007.jpg"
                },
                new Item
                {
                    ItemId = 4,
                    ItemName = "HEV Suit Gloves",
                    CorrespondingGameId = 4,
                    Price = 19.99f,
                    Description = "Protective gloves from the HEV suit worn by Gordon Freeman.",
                    IsListed = false,
                    ImagePath = "https://preview.redd.it/hl2-revision-update-the-grabbity-gloves-v0-ftz143vjmqcb1.jpg?width=640&crop=smart&auto=webp&s=9b3738a0f4bce98cc6a38b34e6ec319d03c05dd0"
                },

                // Items for Mario - 5
                new Item
                {
                    ItemId = 5,
                    ItemName = "Fire Flower",
                    CorrespondingGameId = 5,
                    Price = 14.99f,
                    Description = "A soft collectible version of the iconic power-up.",
                    IsListed = false,
                    ImagePath = "https://mario.wiki.gallery/images/thumb/7/7e/New_Super_Mario_Bros._U_Deluxe_Fire_Flower.png/1200px-New_Super_Mario_Bros._U_Deluxe_Fire_Flower.png"
                },
                new Item
                {
                    ItemId = 6,
                    ItemName = "Mario Cap",
                    CorrespondingGameId = 5,
                    Price = 24.99f,
                    Description = "The classic red cap worn by Mario himself.",
                    IsListed = false,
                    ImagePath = "https://static.wikia.nocookie.net/mario/images/c/cd/Mario_Cap.png/revision/latest?cb=20180310022043"
                },

                // Items for Zelda - 6
                new Item
                {
                    ItemId = 7,
                    ItemName = "Master Sword Replica",
                    CorrespondingGameId = 6,
                    Price = 69.99f,
                    Description = "Faithful replica of Link's legendary blade.",
                    IsListed = false,
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/f/f9/Master_Sword_Lead.png"
                },
                new Item
                {
                    ItemId = 8,
                    ItemName = "Hylian Shield",
                    CorrespondingGameId = 6,
                    Price = 59.99f,
                    Description = "Sturdy shield bearing the crest of Hyrule.",
                    IsListed = false,
                    ImagePath = "https://theswordstall.co.uk/cdn/shop/files/Legend-Of-Zelda-Deluxe-Hylian-Shield-Full-Metal-3.jpg?v=1723552799&width=750"
                },

                // Items for Minecraft - 15
                new Item
                {
                    ItemId = 9,
                    ItemName = "Diamond Pickaxe",
                    CorrespondingGameId = 15,
                    Price = 9.99f,
                    Description = "Miniature version of the famous mining tool.",
                    IsListed = false,
                    ImagePath = "https://static.posters.cz/image/1300/merch/replica-minecraft-diamond-pickaxe-i94007.jpg"
                },
                new Item
                {
                    ItemId = 10,
                    ItemName = "Creeper Plush",
                    CorrespondingGameId = 15,
                    Price = 19.99f,
                    Description = "Soft plush of the infamous explosive mob.",
                    IsListed = false,
                    ImagePath = "https://feltright.com/cdn/shop/files/minecraft-creeper.jpg?v=1720033057&width=800"
                },


                // Items for Cyberstrike 2077
                new Item
                {
                    ItemId = 11,
                    ItemName = "Cybernetic Gauntlet",
                    CorrespondingGameId = 19,
                    Price = 34.99f,
                    Description = "A high-tech gauntlet to hack and crush foes in Cyberstrike 2077.",
                    IsListed = false,
                    ImagePath = "https://static.wikia.nocookie.net/shop-heroes/images/4/4a/Gauntlets_Cybernetic_Gauntlets_Blueprint.png/revision/latest?cb=20200724020856"
                },
                new Item
                {
                    ItemId = 12,
                    ItemName = "Neon Visor",
                    CorrespondingGameId = 19,
                    Price = 24.99f,
                    Description = "A visor that enhances your vision in the neon-lit battles of Cyberstrike 2077.",
                    IsListed = false,
                    ImagePath = "https://www.motocentral.co.uk/cdn/shop/files/Ruroc-EOX-Cyberstrike_-From-Moto-Central-_-Fast-Free-UK-Delivery-257043288_1024x.jpg?v=1744036882"
                },

                // Items for Shadow of Valhalla
                new Item
                {
                    ItemId = 13,
                    ItemName = "Viking Axe",
                    CorrespondingGameId = 20,
                    Price = 44.99f,
                    Description = "A mighty axe for the warriors of Shadow of Valhalla.",
                    IsListed = false,
                    ImagePath = "https://valhalla-vikings.co.uk/cdn/shop/products/il_fullxfull.3370240260_td4v.jpg?v=1679150085&width=1080"
                },
                new Item
                {
                    ItemId = 14,
                    ItemName = "Valhalla Shield",
                    CorrespondingGameId = 20,
                    Price = 34.99f,
                    Description = "A robust shield forged for the bravest of fighters in Shadow of Valhalla.",
                    IsListed = false,
                    ImagePath = "https://www.vikingsroar.com/cdn/shop/products/d7f00df1f2c5a9059ec5dd319139da24.webp?v=1652049514"
                }
            };
            builder.Entity<Item>().HasData(itemsSeed);

              
            var pointShopItemsSeed = new List<PointShopItem>
            {
                new PointShopItem {
                    PointShopItemId = 1,
                    Name = "Blue Profile Background",
                    Description = "A cool blue background for your profile",
                    ImagePath = "https://picsum.photos/id/1/200/200",
                    PointPrice = 1000,
                    ItemType = "ProfileBackground"
                },
                new PointShopItem {
                    PointShopItemId = 2,
                    Name = "Red Profile Background",
                    Description = "A vibrant red background for your profile",
                    ImagePath = "https://picsum.photos/id/20/200/200",
                    PointPrice = 1000,
                    ItemType = "ProfileBackground"
                },
                new PointShopItem {
                    PointShopItemId = 3,
                    Name = "Golden Avatar Frame",
                    Description = "A golden frame for your avatar image",
                    ImagePath = "https://picsum.photos/id/30/200/200",
                    PointPrice = 2000,
                    ItemType = "AvatarFrame"
                },
                new PointShopItem {
                    PointShopItemId = 4,
                    Name = "Silver Avatar Frame",
                    Description = "A silver frame for your avatar image",
                    ImagePath = "https://picsum.photos/id/40/200/200",
                    PointPrice = 1500,
                    ItemType = "AvatarFrame"
                },
                new PointShopItem {
                    PointShopItemId = 5,
                    Name = "Happy Emoticon",
                    Description = "Express yourself with this happy emoticon",
                    ImagePath = "https://picsum.photos/id/50/200/200",
                    PointPrice = 500,
                    ItemType = "Emoticon"
                },
                new PointShopItem {
                    PointShopItemId = 6,
                    Name = "Sad Emoticon",
                    Description = "Express yourself with this sad emoticon",
                    ImagePath = "https://picsum.photos/id/60/200/200",
                    PointPrice = 500,
                    ItemType = "Emoticon"
                },
                new PointShopItem {
                    PointShopItemId = 7,
                    Name = "Gamer Avatar",
                    Description = "Cool gamer avatar for your profile",
                    ImagePath = "https://picsum.photos/id/70/200/200",
                    PointPrice = 1200,
                    ItemType = "Avatar"
                },
                new PointShopItem {
                    PointShopItemId = 8,
                    Name = "Ninja Avatar",
                    Description = "Stealthy ninja avatar for your profile",
                    ImagePath = "https://picsum.photos/id/80/200/200",
                    PointPrice = 1200,
                    ItemType = "Avatar"
                },
                new PointShopItem {
                    PointShopItemId = 9,
                    Name = "Space Mini-Profile",
                    Description = "Space-themed mini profile",
                    ImagePath = "https://picsum.photos/id/90/200/200",
                    PointPrice = 3000,
                    ItemType = "MiniProfile"
                },
                new PointShopItem {
                    PointShopItemId = 10,
                    Name = "Fantasy Mini-Profile",
                    Description = "Fantasy-themed mini profile",
                    ImagePath = "https://picsum.photos/id/100/200/200",
                    PointPrice = 3000,
                    ItemType = "MiniProfile"
                }
            };

            builder.Entity<PointShopItem>().HasData(pointShopItemsSeed);

            var userInventorySeed = new List<UserPointShopItemInventory>
            {
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 1,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 5,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 5,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 5,
                    PointShopItemId = 6,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 6,
                    PointShopItemId = 3,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 7,
                    PointShopItemId = 4,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                }
            };

            builder.Entity<UserPointShopItemInventory>().HasData(userInventorySeed);

            builder.Entity<UsersGames>()
                .HasOne(userGames => userGames.User)
                .WithMany()
                .HasForeignKey(userGames => userGames.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsersGames>()
               .HasOne(userGames => userGames.Game)
               .WithMany()
               .HasForeignKey(userGames => userGames.GameId)
               .OnDelete(DeleteBehavior.Cascade);

           var usersGamesSeed = new List<UsersGames>
            {
                new UsersGames
                {
                    UserId = 4,
                    GameId = 3,
                    IsInWishlist = true,
                    IsPurchased = false,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 4,
                    GameId = 5,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 4,
                    GameId = 6,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 4,
                    GameId = 15,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 5,
                    GameId = 5,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 5,
                    GameId = 6,
                    IsInWishlist = false,
                    IsPurchased = false,
                    IsInCart = true
                },
                new UsersGames
                {
                    UserId = 5,
                    GameId = 19,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 6,
                    GameId = 20,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 7,
                    GameId = 20,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 8,
                    GameId = 15,
                    IsInWishlist = false,
                    IsPurchased = false,
                    IsInCart = true
                },
            };

            builder.Entity<UsersGames>().HasData(usersGamesSeed);

            builder.Entity<StoreTransaction>()
                .HasOne(storeTransaction => storeTransaction.User)
                .WithMany(users => users.StoreTransactions)
                .HasForeignKey(storeTransaction => storeTransaction.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StoreTransaction>()
                .HasOne(storeTransaction => storeTransaction.Game)
                .WithMany(game => game.StoreTransactions)
                .HasForeignKey(storeTransaction => storeTransaction.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            var storeTransactionsSeed = new List<StoreTransaction>
            {
                new StoreTransaction
                {
                    StoreTransactionId = 1,
                    UserId = 4,
                    GameId = 5,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)14.99,
                    WithMoney = true
                },
                new StoreTransaction
                {
                    StoreTransactionId = 2,
                    UserId = 7,
                    GameId = 20,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)34.99,
                    WithMoney = false
                },
                new StoreTransaction
                {
                    StoreTransactionId = 3,
                    UserId = 4,
                    GameId = 15,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)29.99,
                    WithMoney = true
                },
            };

            builder.Entity<StoreTransaction>().HasData(storeTransactionsSeed);

            builder.Entity<ItemTrade>()
                .HasOne(itemTrade => itemTrade.SourceUser)
                .WithMany()
                .HasForeignKey(itemTrade => itemTrade.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemTrade>()
                .HasOne(itemTrade => itemTrade.DestinationUser)
                .WithMany()
                .HasForeignKey(itemTrade => itemTrade.DestinationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemTrade>()
                .HasOne(itemTrade => itemTrade.GameOfTrade)
                .WithMany()
                .HasForeignKey(itemTrade => itemTrade.GameOfTradeId)
                .OnDelete(DeleteBehavior.Cascade);

            var itemTradesSeed = new List<ItemTrade>
            {
                new ItemTrade
                {
                    TradeId = 1,
                    SourceUserId = 4,
                    DestinationUserId = 8,
                    GameOfTradeId = 6,
                    TradeDescription = "Trade 1: AliceJ offers Legend of Zelda to EmilyB",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = false,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 2,
                    SourceUserId = 5,
                    DestinationUserId = 4,
                    GameOfTradeId = 19,
                    TradeDescription = "Trade 2: LiamG offers Cyberstrike 2077 to AliceJ",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 3,
                    SourceUserId = 7,
                    DestinationUserId = 6,
                    GameOfTradeId = 20,
                    TradeDescription = "Trade 3: NoahS offers Shadow of Valhalla to SophieW",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Completed,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = true
                },
            };

            builder.Entity<ItemTrade>().HasData(itemTradesSeed);

            var userInventoryTableSeed = new List<UserInventory>
            {
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 5,
                    GameId = 5,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,

                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 9,
                    GameId = 15,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 7,
                    GameId = 6,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 6,
                    GameId = 5,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 8,
                    GameId = 6,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 12,
                    GameId = 19,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 6,
                    ItemId = 13,
                    GameId = 20,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 10,
                    GameId = 15,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
            };

            // have the delete cascaded only for games
            builder.Entity<UserInventory>()
                .HasOne(userInventory => userInventory.Item)
                .WithMany()
                .HasForeignKey(userInventory => userInventory.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserInventory>()
                .HasOne(userInventory => userInventory.User)
                .WithMany()
                .HasForeignKey(userInventory => userInventory.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserInventory>().HasData(userInventoryTableSeed);

            builder.Entity<ItemTradeDetail>()
            .HasKey(itemTradeDetails => new { itemTradeDetails.TradeId, itemTradeDetails.ItemId });

            builder.Entity<ItemTradeDetail>()
                .HasOne(itemTradeDetails => itemTradeDetails.ItemTrade)
                .WithMany(itemTrade => itemTrade.ItemTradeDetails)
                .HasForeignKey(itemTradeDetails => itemTradeDetails.TradeId)
                .OnDelete(DeleteBehavior.Restrict);

            var itemTradeDetailsSeed = new List<ItemTradeDetail>
            { 
                new ItemTradeDetail { TradeId = 1, ItemId = 7, IsSourceUserItem = true },
                new ItemTradeDetail { TradeId = 2, ItemId = 12, IsSourceUserItem = true },
                new ItemTradeDetail { TradeId = 3, ItemId = 13, IsSourceUserItem = true},
            };

            builder.Entity<ItemTradeDetail>().HasData(itemTradeDetailsSeed);
        }
    }
}
