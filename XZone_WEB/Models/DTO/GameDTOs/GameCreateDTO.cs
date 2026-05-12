using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XZone_WEB.Models.DTO.GameDTOs
{
    public class GameCreateDTO
    {

        public string Name { get; set; }
        public IFormFile ImageFile { get; set; }   // For file upload (from MVC form)
        [ValidateNever]
        [BindNever]
        public string ImageURL { get; set; }       // Final URL to send to API (populated in service)
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public List<int> SelectedDevices { get; set; } = new List<int>();
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> devices { get; set; } = Enumerable.Empty<SelectListItem>();
        [MaxLength(500)]
        public string Description { get; set; }


    }
}
