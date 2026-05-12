using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.CheckOutDTO
{
    public class CheckoutRequestDTO
    {

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
