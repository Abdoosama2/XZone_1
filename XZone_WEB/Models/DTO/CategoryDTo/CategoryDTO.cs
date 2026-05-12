using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models.DTO.CategoryDTo
{
    public class CategoryDTO
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

       
    }
}
