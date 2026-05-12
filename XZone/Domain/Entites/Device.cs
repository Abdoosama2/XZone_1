using System.ComponentModel.DataAnnotations;

namespace XZone.Domain.Entites
{
    public class Device
    {

        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }


        [MaxLength(500)]
        public string Icon { get; set; }

        public List<GameDevice>? Devices { get; set; }
    }
}
