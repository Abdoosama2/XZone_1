using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models.DTO.CategoryDTo
{
    public class CategoryCreateDto
    {

        [MaxLength(100)]
        public string Name { get; set; }
    }
}
