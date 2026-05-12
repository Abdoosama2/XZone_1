using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.Category
{
    public class CategoryUpdatedDTO
    {

        public int Id { get; set; }

        [Required(ErrorMessage ="this field can't be blank")]
        public string Name { get; set; }


    }
}
