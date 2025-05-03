namespace SteamHub.ApiContract.Models.User;

public class UpdateUserRequest
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public RoleEnum Role { get; set; }
}
