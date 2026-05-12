using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.CartDTOs
{
    public class AddToCartDTO
    {

        [Required]
        public int GameId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }
    }
}
