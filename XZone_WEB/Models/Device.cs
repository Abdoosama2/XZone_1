using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models { 
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
