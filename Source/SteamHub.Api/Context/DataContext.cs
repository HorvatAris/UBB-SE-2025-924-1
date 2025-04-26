using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
	public DbSet<Tag> Tags { get; set; }

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

		builder.Entity<Tag>().HasData(testTagSeed);
	}
}