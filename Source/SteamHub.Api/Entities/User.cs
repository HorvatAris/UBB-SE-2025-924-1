using System.ComponentModel.DataAnnotations;

namespace SteamHub.Api.Entities;

public class User
{

    public User()
    {
    }

    public enum Role
    {
        Developer = 1,
        User = 0,
    }

    public int UserId { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    [Required]
    public Role UserRole { get; set; }

    public ICollection<UserInventoryItem> UserInventoryItems { get; set; }
}