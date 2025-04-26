using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
	public DbSet<Tag> Tags { get; set; }

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
    builder.Entity<Role>()
        .Property(e => e.Id).ValueGeneratedNever();

    builder.Entity<Role>().HasData(
        Enum.GetValues(typeof(RoleEnum))
            .Cast<RoleEnum>()
            .Select(
                e => new Role
                {
                    Id = e,
                    Name = e.ToString()
                }));


    var testGamesSeed = new List<TestGame>

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
        new User { UserId = 1, Email = "user1@gmail.com", PointsBalance = 100, UserName = "User1", RoleId  = RoleEnum.User, WalletBalance = 56 },
        new User { UserId = 2, Email = "user2@gmail.com", PointsBalance = 45, UserName = "User2", RoleId  = RoleEnum.User, WalletBalance = 78 },
        new User { UserId = 3, Email = "user3@gmail.com", PointsBalance = 234, UserName = "User3", RoleId  = RoleEnum.Developer, WalletBalance = 21 },
        new User { UserId = 4, Email = "user4@gmail.com", PointsBalance = 34, UserName = "User4", RoleId  = RoleEnum.Developer, WalletBalance = 455 },
    };

    builder.Entity<TestGame>().HasData(testGamesSeed);
		builder.Entity<Tag>().HasData(testTagSeed);
    builder.Entity<User>().HasData(usersSeed);
	}
}