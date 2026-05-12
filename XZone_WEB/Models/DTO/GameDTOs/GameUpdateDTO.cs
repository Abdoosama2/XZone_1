using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models.DTO.GameDTOs
{
    public class GameUpdateDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(300)]
        public string ImageURL { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

    }
}
