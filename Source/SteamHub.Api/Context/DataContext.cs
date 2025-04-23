using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
	public DbSet<TestGame> TestGames { get; set; }

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
	}
}