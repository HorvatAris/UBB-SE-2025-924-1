namespace SteamHub.ApiContract.Models.User;

public interface IUserDetails
{
    int UserId { get; }
    float PointsBalance { get; set; }
    UserRole UserRole { get; }
    string UserName { get; }
    string Email { get; }
    float WalletBalance { get; }
}