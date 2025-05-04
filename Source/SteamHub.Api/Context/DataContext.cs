namespace SteamHub.Api.Context
{
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using SteamHub.ApiContract.Models.Game;
    using System.Reflection.Emit;


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
                    UserId = 1, Email = "johndoe@gmail.com", PointsBalance = 100, UserName = "John Doe",
                    RoleId = RoleEnum.User,
                    WalletBalance = 56
                },
                new User
                {
                    UserId = 2, Email = "michaeljohn@gmail.com", PointsBalance = 45, UserName = "Michael John",
                    RoleId = RoleEnum.User,
                    WalletBalance = 78
                },
                new User
                {
                    UserId = 3, Email = "janedoe@gmail.com", PointsBalance = 234, UserName = "Jane Doe",
                    RoleId = RoleEnum.Developer, WalletBalance = 21
                },
                new User
                {
                    UserId = 4, Email = "mariaelena@gmail.com", PointsBalance = 34, UserName = "Maria Elena",
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
                },
                new Game
                {
                    GameId=4,
                    Name = "The Legend of Zelda",
                    Description = "An action-adventure game set in the fantasy land of Hyrule, where players control Link to rescue Princess Zelda.",
                    ImagePath = "https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg",
                    Price = 59.99m,
                    MinimumRequirements = "Intel Core i5, 8GB RAM, GTX 960",
                    RecommendedRequirements = "Intel Core i7, 16GB RAM, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 1500,
                    TrailerPath = "https://www.youtube.com/watch?v=0u8g1c2v4xE", // The Legend of Zelda: Breath of the Wild Trailer
                    GameplayPath = "https://www.youtube.com/watch?v=0u8g1c2v4xE", // The Legend of Zelda: Breath of the Wild Gameplay
                    Discount = 0.20m,
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
            
            builder.Entity<Item>()
                .HasOne(i => i.Game)
                .WithMany(g => g.Items)
                .HasForeignKey(i => i.CorrespondingGameId);
            
            var itemsSeed =  new List<Item>
            {
                // Items for Game 1: Legends of Etheria
                new Item
                {
                    ItemId = 1,
                    ItemName = "Ethereal Blade",
                    CorrespondingGameId = 1,
                    Price = 29.99f,
                    Description = "A mystical blade imbued with ancient magic from Legends of Etheria.",
                    IsListed = true,
                    ImagePath = "https://cdn.example.com/etheria/ethereal-blade.jpg"
                },
                new Item
                {
                    ItemId = 7,
                    ItemName = "pilfered ethereal blade",
                    CorrespondingGameId = 1,
                    Price = 29.99f,
                    Description = "A mystical blade imbued with ancient magic from Legends of Etheria.",
                    IsListed = false,
                    ImagePath = "https://cdn.example.com/etheria/ethereal-blade.jpg"
                },
                new Item
                {
                    ItemId = 2,
                    ItemName = "Mystic Armour",
                    CorrespondingGameId = 1,
                    Price = 39.99f,
                    Description = "An enchanted armour that protects the bearer in Legends of Etheria.",
                    IsListed = true,
                    ImagePath = "https://cdn.example.com/etheria/mystic-armour.jpg"
                },

                // Items for Game 2: Cyberstrike 2077
                new Item
                {
                    ItemId = 3,
                    ItemName = "Cybernetic Gauntlet",
                    CorrespondingGameId = 2,
                    Price = 34.99f,
                    Description = "A high-tech gauntlet to hack and crush foes in Cyberstrike 2077.",
                    IsListed = true,
                    ImagePath = "https://cdn.example.com/cyberstrike/gauntlet.jpg"
                },
                new Item
                {
                    ItemId = 4,
                    ItemName = "Neon Visor",
                    CorrespondingGameId = 2,
                    Price = 24.99f,
                    Description = "A visor that enhances your vision in the neon-lit battles of Cyberstrike 2077.",
                    IsListed = true,
                    ImagePath = "https://cdn.example.com/cyberstrike/neon-visor.jpg"
                },

                // Items for Game 3: Shadow of Valhalla
                new Item
                {
                    ItemId = 5,
                    ItemName = "Viking Axe",
                    CorrespondingGameId = 3,
                    Price = 44.99f,
                    Description = "A mighty axe for the warriors of Shadow of Valhalla.",
                    IsListed = true,
                    ImagePath = "https://cdn.example.com/valhalla/viking-axe.jpg"
                },
                new Item
                {
                    ItemId = 6,
                    ItemName = "Valhalla Shield",
                    CorrespondingGameId = 3,
                    Price = 34.99f,
                    Description = "A robust shield forged for the bravest of fighters in Shadow of Valhalla.",
                    IsListed = true,
                    ImagePath = "https://cdn.example.com/valhalla/shield.jpg"
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
                    UserId = 1,
                    PointShopItemId = 1,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 1,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 1,
                    PointShopItemId = 5,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 2,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 2,
                    PointShopItemId = 6,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 3,
                    PointShopItemId = 3,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 3,
                    PointShopItemId = 4,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                }
            };

            builder.Entity<UserPointShopItemInventory>().HasData(userInventorySeed);

            builder.Entity<UsersGames>()
                .HasOne(ug => ug.User)
                .WithMany()
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsersGames>()
               .HasOne(ug => ug.Game)
               .WithMany()
               .HasForeignKey(ug => ug.GameId)
               .OnDelete(DeleteBehavior.Cascade);

           var usersGamesSeed = new List<UsersGames>
            {
                new UsersGames
                {
                    UserId = 1,
                    GameId = 1,
                    IsInWishlist = true,
                    IsPurchased = false,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 1,
                    GameId = 2,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 1,
                    GameId = 3,
                    IsInWishlist = false,
                    IsPurchased = false,
                    IsInCart = true
                },
                new UsersGames
                {
                    UserId = 2,
                    GameId = 1,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 2,
                    GameId = 3,
                    IsInWishlist = false,
                    IsPurchased = false,
                    IsInCart = true
                },
            };

            builder.Entity<UsersGames>().HasData(usersGamesSeed);

            builder.Entity<StoreTransaction>()
                .HasOne(st => st.User)
                .WithMany(u => u.StoreTransactions)
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StoreTransaction>()
                .HasOne(st => st.Game)
                .WithMany(g => g.StoreTransactions)
                .HasForeignKey(st => st.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            var storeTransactionsSeed = new List<StoreTransaction>
            {
                new StoreTransaction
                {
                    StoreTransactionId = 1,
                    UserId = 1,
                    GameId = 1,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)49.99,
                    WithMoney = true
                },
                new StoreTransaction
                {
                    StoreTransactionId = 2,
                    UserId = 2,
                    GameId = 2,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)59.99,
                    WithMoney = false
                }
            };

            builder.Entity<StoreTransaction>().HasData(storeTransactionsSeed);

            builder.Entity<ItemTrade>()
                .HasOne(it => it.SourceUser)
                .WithMany()
                .HasForeignKey(it => it.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemTrade>()
                .HasOne(it => it.DestinationUser)
                .WithMany()
                .HasForeignKey(it => it.DestinationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemTrade>()
                .HasOne(it => it.GameOfTrade)
                .WithMany()
                .HasForeignKey(it => it.GameOfTradeId)
                .OnDelete(DeleteBehavior.Cascade);

            var itemTradesSeed = new List<ItemTrade>
            {
                new ItemTrade
                {
                    TradeId = 1,
                    SourceUserId = 1,
                    DestinationUserId = 2,
                    GameOfTradeId = 1,
                    TradeDescription = "Trade 1: John Doe offers Game1 to Michael John",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = false,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 2,
                    SourceUserId = 3,
                    DestinationUserId = 4,
                    GameOfTradeId = 2,
                    TradeDescription = "Trade 2: Jane Doe offers Game2 to Maria Elena",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 3,
                    SourceUserId = 1,
                    DestinationUserId = 2,
                    GameOfTradeId = 1,
                    TradeDescription = "Trade 1: John Doe offers Game1 to Michael John",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Declined,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = true
                },

            };

            builder.Entity<ItemTrade>().HasData(itemTradesSeed);

            var userInventoryTableSeed = new List<UserInventory>
            {
                new UserInventory
                {
                    UserId = 1,
                    ItemId = 1,
                    GameId = 1,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 1,
                    ItemId = 2,
                    GameId = 1,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 1,
                    ItemId = 3,
                    GameId = 2,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 1,
                    ItemId = 4,
                    GameId = 2,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 1,
                    ItemId = 6,
                    GameId = 3,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 1,
                    ItemId = 7,
                    GameId = 1,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 2,
                    ItemId = 6,
                    GameId = 3,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 2,
                    ItemId = 5,
                    GameId = 3,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
                new UserInventory
                {
                    UserId = 2,
                    ItemId = 7,
                    GameId = 1,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0)
                },
            };

            // have the delete cascaded only for games
            builder.Entity<UserInventory>()
                .HasOne(ui => ui.Item)
                .WithMany()
                .HasForeignKey(ui => ui.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserInventory>()
                .HasOne(ui => ui.User)
                .WithMany()
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserInventory>().HasData(userInventoryTableSeed);

            builder.Entity<ItemTradeDetail>()
            .HasKey(itd => new { itd.TradeId, itd.ItemId });

            builder.Entity<ItemTradeDetail>()
                .HasOne(itd => itd.ItemTrade)
                .WithMany(it => it.ItemTradeDetails)
                .HasForeignKey(itd => itd.TradeId)
                .OnDelete(DeleteBehavior.Restrict);

            var itemTradeDetailsSeed = new List<ItemTradeDetail>
            { 
                new ItemTradeDetail { TradeId = 1, ItemId = 1, IsSourceUserItem = true },
               // new ItemTradeDetail { TradeId = 1, ItemId = 1, IsSourceUserItem = false },
                new ItemTradeDetail { TradeId = 2, ItemId = 2, IsSourceUserItem = false },
               // new ItemTradeDetail { TradeId = 2, ItemId = 2, IsSourceUserItem = true }
            };

            builder.Entity<ItemTradeDetail>().HasData(itemTradeDetailsSeed);
        }
    }
}
