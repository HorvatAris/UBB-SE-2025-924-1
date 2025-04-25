using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class DataContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> User { get; set; }

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



        builder.Entity<Game>()
            .HasMany(g => g.Tags)
            .WithMany();

        builder.Entity<Game>()
            .Ignore(g => g.Rating);

        builder.Entity<Game>()
            .Ignore(g => g.TrendingScore);
        
        builder.Entity<Game>()
            .Property(g => g.Status)
            .HasConversion<string>();

        builder.Entity<Game>()
            .Ignore(g => g.NumberOfRecentPurchases);

        builder.Entity<Tag>()
            .Ignore(t => t.NumberOfUserGamesWithTag);

        builder.Entity<Game>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(g => g.PublisherIdentifier)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Game>()
            .HasKey(g => g.Identifier);
        
        SetUpTestData(builder);

    }

    private static void SetUpTestData(ModelBuilder builder)
    {
        var developerRole = Entities.User.Role.Developer;
        var userRole = Entities.User.Role.User;
        var testUserSeed = new List<User>
        {
            new User
            {
                UserId = 1, UserName = "Roblox", WalletBalance = 11.00f, PointsBalance = 22.00f,
                UserRole = developerRole, Email = "roblox@gmail.com"
            },
            new User
            {
                UserId = 2, UserName = "john_doe", WalletBalance = 100.00f, PointsBalance = 500.00f,
                UserRole = userRole, Email = "john_doe@example.com"
            },
            new User
            {
                UserId = 3, UserName = "jane_smith", WalletBalance = 150.00f, PointsBalance = 300.00f,
                UserRole =developerRole, Email = "jane_smith@example.com"
            },
            new User
            {
                UserId = 4, UserName = "alex_brown", WalletBalance = 50.00f, PointsBalance = 150.00f,
                UserRole = userRole, Email = "alex_brown@example.com"
            },
            new User
            {
                UserId = 5, UserName = "Behaviour Interactive", WalletBalance = 200.00f, PointsBalance = 1000.00f,
                UserRole =developerRole, Email = "behaviour@example.com"
            },
            new User
            {
                UserId = 6, UserName = "Valve Corporation", WalletBalance = 150.00f, PointsBalance = 300.00f,
                UserRole =developerRole, Email = "valve@example.com"
            },
            new User
            {
                UserId = 7, UserName = "Nintendo", WalletBalance = 250.00f, PointsBalance = 800.00f,
                UserRole =developerRole, Email = "nintendo@example.com"
            },
            new User
            {
                UserId = 8, UserName = "Hempuli Oy", WalletBalance = 100.00f, PointsBalance = 500.00f,
                UserRole =developerRole, Email = "hempuli@example.com"
            },
            new User
            {
                UserId = 9, UserName = "Mobius Digital", WalletBalance = 120.00f, PointsBalance = 600.00f,
                UserRole =developerRole, Email = "mobius@example.com"
            },
            new User
            {
                UserId = 10, UserName = "Mojang Studios", WalletBalance = 300.00f, PointsBalance = 900.00f,
                UserRole =developerRole, Email = "mojang@example.com"
            },
            new User
            {
                UserId = 11, UserName = "Unknown Worlds Entertainment", WalletBalance = 180.00f,
                PointsBalance = 700.00f, UserRole =developerRole, Email = "unknownworlds@example.com"
            },
            new User
            {
                UserId = 12, UserName = "mary_jones", WalletBalance = 200.00f, PointsBalance = 1000.00f,
                UserRole =developerRole, Email = "mary_jones@example.com"
            }
        };
        
        builder.Entity<User>().HasData(testUserSeed);

        var testTagSeeds = new List<Tag>
        {
            new Tag { TagId = 1, Tag_name = "Tag1" },
            new Tag { TagId = 2, Tag_name = "Rogue-Like" },
            new Tag { TagId = 3, Tag_name = "Third-Person Shooter" },
            new Tag { TagId = 4, Tag_name = "Multiplayer" },
            new Tag { TagId = 5, Tag_name = "Horror" },
            new Tag { TagId = 6, Tag_name = "First-Person Shooter" },
            new Tag { TagId = 7, Tag_name = "Action" },
            new Tag { TagId = 8, Tag_name = "Platformer" },
            new Tag { TagId = 9, Tag_name = "Adventure" },
            new Tag { TagId = 10, Tag_name = "Puzzle" },
            new Tag { TagId = 11, Tag_name = "Exploration" },
            new Tag { TagId = 12, Tag_name = "Sandbox" },
            new Tag { TagId = 13, Tag_name = "Survival" },
            new Tag { TagId = 14, Tag_name = "Arcade" },
            new Tag { TagId = 15, Tag_name = "RPG" },
            new Tag { TagId = 16, Tag_name = "Racing" }
        };
        builder.Entity<Tag>().HasData(testTagSeeds);
    }
}