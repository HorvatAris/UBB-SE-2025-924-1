using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SteamHub.Api.Entities;

public class PointShopItem
{
    public PointShopItem()
    {
    }

    [Key]
    public int ItemIdentifier { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public double PointPrice { get; set; }

    [Required]
    public string ItemType { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public ICollection<UserInventoryItem> UserInventoryItems { get; set; }
}