namespace SteamHub.ApiContract.Models.User;

public class UserResponse
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public RoleEnum Role { get; set; }
}
