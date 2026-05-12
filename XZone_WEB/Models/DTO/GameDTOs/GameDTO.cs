using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace XZone_WEB.Models.DTO.GameDTOs
{
    public class GameDTO
    {

        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(300)]
        public string ImageURL { get; set; }
        [ForeignKey("Category")]

        [JsonIgnore]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

    }
}
