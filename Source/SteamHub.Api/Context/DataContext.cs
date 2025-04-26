using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
	public DbSet<TestGame> TestGames { get; set; }

    public DbSet<User> Users { get; set; }

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
		var testGamesSeed = new List<TestGame>
		{
			new TestGame { Id = 1, Name = "Roblox"},
			new TestGame { Id = 2, Name = "Minecraft"},
			new TestGame { Id = 3, Name = "Metin2"}
		};

		builder.Entity<TestGame>().HasData(testGamesSeed);

        var usersSeed = new List<User>
        {
            new User { UserId = 1, Email = "user1@gmail.com", PointsBalance = 100, UserName = "User1", UserRole  = Role.User, WalletBalance = 56 },
            new User { UserId = 2, Email = "user2@gmail.com", PointsBalance = 45, UserName = "User2", UserRole  = Role.User, WalletBalance = 78 },
            new User { UserId = 3, Email = "user3@gmail.com", PointsBalance = 234, UserName = "User3", UserRole  = Role.User, WalletBalance = 21 }
        };

        builder.Entity<User>().HasData(usersSeed);
    }
}