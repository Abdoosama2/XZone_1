using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace XZone.Application.DTO.GameDTOs
{
    public class GameCreateDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100)]
        public string Description { get; set; }

        
        public string? ImageURL { get; set; }

        [Required(ErrorMessage ="the Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="the stock quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid category")]
        public int StockQuantity { get; set; }

        public List<int> SelectedDevices { get; set; } = new();

    }
}
