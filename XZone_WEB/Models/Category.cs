using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models
{
    public class Category
    {

        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public List<Game> Games { get; set; }
    }
}
