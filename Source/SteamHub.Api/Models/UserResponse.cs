namespace SteamHub.Api.Models;

public class UserResponse
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public Role UserRole { get; set; }
}
