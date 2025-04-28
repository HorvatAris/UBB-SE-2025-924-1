namespace SteamHub.Api.Models.Game;

public class UpdateGameRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public decimal? Price { get; set; }

    public string? MinimumRequirements { get; set; }

    public string? RecommendedRequirements { get; set; }

    public decimal? Rating { get; set; }

    public GameStatusEnum? Status { get; set; }

    public string? RejectMessage { get; set; }

    public int? NumberOfRecentPurchases { get; set; }

    public string? TrailerPath { get; set; }

    public string? GameplayPath { get; set; }

    public decimal? Discount { get; set; }
}