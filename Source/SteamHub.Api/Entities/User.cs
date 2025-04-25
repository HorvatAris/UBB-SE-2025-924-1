namespace SteamHub.Api.Entities;

public class User
{
    public enum Role
    {
        Developer = 1,
        User = 0,
    }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public Role UserRole { get; set; }
}