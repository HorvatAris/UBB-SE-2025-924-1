using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class PaypalPaymentViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public decimal AmountToPay { get; set; }

        public string? Message { get; set; }
        public bool IsSuccess { get; set; } // new
        public int PointsEarned { get; set; } // new
    }
}
