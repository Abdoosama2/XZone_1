using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.GameDTOs
{
    public class GameUpdateDTO
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "the Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "the stock quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid category")]
        public int StockQuantity { get; set; }
        public string? ImageURL { get; set; }


        public List<int> SelectedDevices { get; set; } = new();

    }
}
