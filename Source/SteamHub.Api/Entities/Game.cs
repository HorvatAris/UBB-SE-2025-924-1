namespace SteamHub.Api.Entities;

using Models;
using SteamHub.Api.Models.Game;

public class Game
{
    public int GameId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public decimal Price { get; set; }

    public string? MinimumRequirements { get; set; }

    public string? RecommendedRequirements { get; set; }

    public GameStatusEnum StatusId { get; set; }

    public virtual GameStatus Status { get; set; }

    public string? RejectMessage { get; set; }

    public ISet<Tag> Tags { get; set; } = new HashSet<Tag>();

    public decimal Rating { get; set; }

    public int NumberOfRecentPurchases { get; set; }

    public string? TrailerPath { get; set; }

    public string? GameplayPath { get; set; }

    public decimal Discount { get; set; }

    public virtual User Publisher { get; set; }

    public int PublisherUserId { get; set; }
}