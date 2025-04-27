using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
	public DbSet<PointShopItem> PointShopItems { get; set; }

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