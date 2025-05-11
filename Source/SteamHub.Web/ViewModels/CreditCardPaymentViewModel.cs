using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class CreditCardPaymentViewModel
    {
        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiration Date")]
        public string ExpirationDate { get; set; }

        [Required]
        [Display(Name = "CVV")]
        public string CVV { get; set; }

        [Required]
        [Display(Name = "Card Owner Name")]
        public string OwnerName { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Message { get; set; }        
        public bool IsSuccess { get; set; }         
        public int PointsEarned { get; set; }       
    }
}
