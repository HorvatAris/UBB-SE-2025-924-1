using SteamHub.ApiContract.Models.Tag;
using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class EditGameViewModel
    {
        [Required]
        public string GameId { get; set; }

        [Required]
        [Display(Name = "Game Title")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Price")]
        public string Price { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "Trailer URL")]
        public string TrailerUrl { get; set; }

        [Display(Name = "Gameplay URL")]
        public string GameplayUrl { get; set; }

        [Display(Name = "Minimum Requirements")]
        public string MinimumRequirement { get; set; }

        [Display(Name = "Recommended Requirements")]
        public string RecommendedRequirement { get; set; }

        [Display(Name = "Discount (%)")]
        public string Discount { get; set; }

        // Tags from the system
        public List<Tag> AllTags { get; set; } = new();

        // Tags selected for this game
        public List<int> SelectedTags { get; set; } = new List<int>();

    }
}
