using SteamHub.ApiContract.Models.Tag;

namespace SteamHub.Web.ViewModels
{
    public class CreateGameViewModel
    {
        public string GameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string GameplayUrl { get; set; }
        public string MinimumRequirement { get; set; }
        public string RecommendedRequirement { get; set; }
        public string Discount { get; set; }

        public List<Tag> AllTags { get; set; } = new();
        public List<Tag> SelectedTags { get; set; } = new();
    }

}
