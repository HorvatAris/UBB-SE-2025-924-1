using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SteamHub.Api.Entities;
using System.Reflection.Emit;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
	public DbSet<TestGame> TestGames { get; set; }
	
	public DbSet<User> Users { get; set; }

	public DbSet<PointShopItem> PointShopItems { get; set; }

    public DbSet<UserInventoryItem> UserInventoryItems { get; set; }



    private readonly IConfiguration _configuration;
	public DataContext(DbContextOptions options, IConfiguration configuration) : base(options)
	{
		_configuration = configuration;
	}

	public DataContext()
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Default"));
		}
	}
	protected override void OnModelCreating(ModelBuilder builder)
	{
        base.OnModelCreating(builder);

        builder.Entity<UserInventoryItem>()
            .HasKey(uii => new { uii.UserId, uii.ItemIdentifier });

        builder.Entity<UserInventoryItem>()
            .HasOne(uii => uii.User)
            .WithMany(u => u.UserInventoryItems)
            .HasForeignKey(uii => uii.UserId);

        builder.Entity<UserInventoryItem>()
            .HasOne(uii => uii.PointShopItem)
            .WithMany(psi => psi.UserInventoryItems)
            .HasForeignKey(uii => uii.ItemIdentifier);

        var now = Convert.ToDateTime("2025-04-25 00:00:00.0000000");

        var testGamesSeed = new List<TestGame>
		{
			new TestGame { Id = 1, Name = "Roblox"},
			new TestGame { Id = 2, Name = "Minecraft"},
			new TestGame { Id = 3, Name = "Metin2"}
		};

        builder.Entity<TestGame>().HasData(testGamesSeed);


        var usersSeed = new List<User>
		{
			new User { UserId = 1, Email = "user1@gmail.com", PointsBalance = 100, UserName = "User1", UserRole  = User.Role.User, WalletBalance = 56 },
			new User { UserId = 2, Email = "user2@gmail.com", PointsBalance = 45, UserName = "User2", UserRole  = User.Role.User, WalletBalance = 78 },
			new User { UserId = 3, Email = "user3@gmail.com", PointsBalance = 234, UserName = "User3", UserRole  = User.Role.User, WalletBalance = 21 }
		};

        builder.Entity<User>().HasData(usersSeed);

		var pointShopItemsSeed = new List<PointShopItem>
		{ 
			new PointShopItem { ItemIdentifier = 1, Name = "Item1", Description = "Description1", ImagePath = "https://www.teeshood.com/cdn/shop/products/IMG_2627_1800x.png?v=1627292451", PointPrice = 10.0, ItemType = "Sticker", IsActive = true },
            new PointShopItem { ItemIdentifier = 2, Name = "Item2", Description = "Description2", ImagePath = "https://store.playstation.com/store/api/chihiro/00_09_000/container/US/en/19/UP1415-CUSA03724_00-AV00000000000160/image?w=320&h=320&bg_color=000000&opacity=100&_version=00_09_000", PointPrice = 20.0, ItemType = "Avatar", IsActive = false },
            new PointShopItem { ItemIdentifier = 3, Name = "Item3", Description = "Description3", ImagePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRPOga6RK9EhRyqpH7wO2HyZcjxyiryNk6vIw&s", PointPrice = 30.0, ItemType = "Emoji", IsActive = true },
            new PointShopItem { ItemIdentifier = 4, Name = "Item4", Description = "Description4", ImagePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRIaA9WSGUAWBQ2oVRwsPWoC7CrnhuYX5mMXA&s", PointPrice = 40.0, ItemType = "Sticker", IsActive = true }
        };

        builder.Entity<PointShopItem>().HasData(pointShopItemsSeed);

		var userInventoryItemsSeed = new List<UserInventoryItem>
		{
            new UserInventoryItem { UserId = 1, ItemIdentifier = 1, PurchaseDate = now, isActive = true },
            new UserInventoryItem { UserId = 2, ItemIdentifier = 2, PurchaseDate = now, isActive = false },
            new UserInventoryItem { UserId = 3, ItemIdentifier = 3, PurchaseDate = now, isActive = true },
			new UserInventoryItem { UserId = 1, ItemIdentifier = 4, PurchaseDate = now, isActive = true }
        };

        builder.Entity<UserInventoryItem>().HasData(userInventoryItemsSeed);
    }
}