using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.Category
{
    public class CategoryCreateDto
    {

        [MaxLength(100)]
        public string Name { get; set; }
    }
}
