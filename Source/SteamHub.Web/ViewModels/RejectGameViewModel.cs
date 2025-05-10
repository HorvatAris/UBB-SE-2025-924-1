using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class RejectGameViewModel
    {
        [Required]
        public int GameId { get; set; }

        [Display(Name = "Rejection Message")]
        [DataType(DataType.MultilineText)]
        public string? RejectionMessage { get; set; }
    }
}
