using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.Category
{
    public class CategoryDTO
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

       
    }
}
