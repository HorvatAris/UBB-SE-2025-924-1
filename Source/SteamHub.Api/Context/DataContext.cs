namespace SteamHub.Api.Context
{
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Models;

public class DataContext : DbContext
{

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
        public DbSet<PointShopItem> PointShopItems { get; set; }

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
                .Property(e => e.Id).ValueGeneratedNever();

            builder.Entity<Role>().HasData(Enum.GetValues(typeof(RoleEnum))
                .Cast<RoleEnum>()
                .Select(e => new Role
                {
                    Id = e,
                    Name = e.ToString()
                }));

            var testTagSeed = new List<Tag>
            {
                new Tag { TagId = 1, TagName = "Tag1" },
                new Tag { TagId = 2, TagName = "Rogue-Like" },
                new Tag { TagId = 3, TagName = "Third-Person Shooter" },
                new Tag { TagId = 4, TagName = "Multiplayer" },
                new Tag { TagId = 5, TagName = "Horror" },
                new Tag { TagId = 6, TagName = "First-Person Shooter" },
                new Tag { TagId = 7, TagName = "Action" },
                new Tag { TagId = 8, TagName = "Platformer" },
                new Tag { TagId = 9, TagName = "Adventure" },
                new Tag { TagId = 10, TagName = "Puzzle" },
                new Tag { TagId = 11, TagName = "Exploration" },
                new Tag { TagId = 12, TagName = "Sandbox" },
                new Tag { TagId = 13, TagName = "Survival" },
                new Tag { TagId = 14, TagName = "Arcade" },
                new Tag { TagId = 15, TagName = "RPG" },
                new Tag { TagId = 16, TagName = "Racing" }
            };

            var usersSeed = new List<User>
            {
                new User
                {
                    UserId = 1, Email = "user1@gmail.com", PointsBalance = 100, UserName = "User1",
                    RoleId = RoleEnum.User,
                    WalletBalance = 56
                },
                new User
                {
                    UserId = 2, Email = "user2@gmail.com", PointsBalance = 45, UserName = "User2",
                    RoleId = RoleEnum.User,
                    WalletBalance = 78
                },
                new User
                {
                    UserId = 3, Email = "user3@gmail.com", PointsBalance = 234, UserName = "User3",
                    RoleId = RoleEnum.Developer, WalletBalance = 21
                },
                new User
                {
                    UserId = 4, Email = "user4@gmail.com", PointsBalance = 34, UserName = "User4",
                    RoleId = RoleEnum.Developer, WalletBalance = 455
                },
            };

            builder.Entity<Tag>().HasData(testTagSeed);
            builder.Entity<User>().HasData(usersSeed);

            builder.Entity<GameStatus>()
                .Property(e => e.Id).ValueGeneratedNever();

            builder.Entity<GameStatus>().HasData(Enum.GetValues(typeof(GameStatusEnum))
                .Cast<GameStatusEnum>()
                .Select(e => new GameStatus
                {
                    Id = e,
                    Name = e.ToString()
                }));

            var testGameSeed = new List<Game>
            {
                new Game
                {
                    GameId = 1,
                    Name = "Legends of Etheria",
                    Description =
                        "An epic open-world RPG set in a mystical realm full of adventure, magic, and danger.",
                    ImagePath =
                        "https://upload.wikimedia.org/wikipedia/en/5/5e/Elden_Ring_Box_art.jpg", // Elden Ring box art (public)
                    Price = 49.99m,
                    MinimumRequirements = "Intel i5-8400, 8GB RAM, GTX 1060",
                    RecommendedRequirements = "Intel i7-8700K, 16GB RAM, RTX 2070",
                    StatusId = GameStatusEnum.Rejected,
                    RejectMessage = "rejected game",
                    Rating = 4.7m,
                    NumberOfRecentPurchases = 1200,
                    TrailerPath = "https://www.youtube.com/watch?v=E3Huy2cdih0", // Elden Ring official trailer
                    GameplayPath = "https://www.youtube.com/watch?v=AKXiKBnzpBQ", // Elden Ring gameplay preview
                    Discount = 0.15m,
                    PublisherUserId = usersSeed[2].UserId
                },
                new Game
                {
                    GameId = 2,
                    Name = "Cyberstrike 2077",
                    Description = "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.",
                    ImagePath =
                        "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg", // Cyberpunk 2077 box art
                    Price = 59.99m,
                    MinimumRequirements = "Intel i5-3570K, 8GB RAM, GTX 780",
                    RecommendedRequirements = "Intel i7-4790, 12GB RAM, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 950,
                    TrailerPath = "https://www.youtube.com/watch?v=FknHjl7eQ6o", // Cyberpunk 2077 Official Trailer
                    GameplayPath = "https://www.youtube.com/watch?v=8X2kIfS6fb8", // Cyberpunk 2077 Gameplay Demo
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[3].UserId
                },
                new Game
                {
                    GameId = 3,
                    Name = "Shadow of Valhalla",
                    Description = "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.",
                    ImagePath =
                        "https://upload.wikimedia.org/wikipedia/en/6/6d/Assassin%27s_Creed_Valhalla_cover.jpg", // Assassin's Creed Valhalla cover
                    Price = 44.99m,
                    MinimumRequirements = "Intel i5-4460, 8GB RAM, GTX 960",
                    RecommendedRequirements = "Intel i7-6700K, 16GB RAM, GTX 1080",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.5m,
                    NumberOfRecentPurchases = 780,
                    TrailerPath =
                        "https://www.youtube.com/watch?v=ssrNcwxALS4", // Assassin's Creed Valhalla Cinematic World Premiere
                    GameplayPath =
                        "https://www.youtube.com/watch?v=gncB1_e9n8E", // Assassin's Creed Valhalla Gameplay Walkthrough
                    Discount = 0.10m,
                    PublisherUserId = usersSeed[2].UserId
                }
            };

            builder.Entity<Game>().HasData(testGameSeed);

            builder.Entity<Game>()
                .HasMany(g => g.Tags)
                .WithMany(t => t.Games)
                .UsingEntity<Dictionary<string, object>>("GameTag",
                    x => x.HasData(new { GamesGameId = testGameSeed[0].GameId, TagsTagId = testTagSeed[0].TagId },
                        new { GamesGameId = testGameSeed[0].GameId, TagsTagId = testTagSeed[1].TagId },
                        new { GamesGameId = testGameSeed[0].GameId, TagsTagId = testTagSeed[2].TagId },
                        new { GamesGameId = testGameSeed[1].GameId, TagsTagId = testTagSeed[3].TagId },
                        new { GamesGameId = testGameSeed[1].GameId, TagsTagId = testTagSeed[4].TagId },
                        new { GamesGameId = testGameSeed[1].GameId, TagsTagId = testTagSeed[5].TagId },
                        new { GamesGameId = testGameSeed[2].GameId, TagsTagId = testTagSeed[6].TagId },
                        new { GamesGameId = testGameSeed[2].GameId, TagsTagId = testTagSeed[7].TagId },
                        new { GamesGameId = testGameSeed[2].GameId, TagsTagId = testTagSeed[8].TagId }));
              
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
        }
    }
}