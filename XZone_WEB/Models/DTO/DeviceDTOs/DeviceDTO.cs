using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models.DTO.DeviceDTOs
{
    public class DeviceDTO
    {

        public int Id { get; set; } 
        public string Name { get; set; }


        [MaxLength(500)]
        public string Icon { get; set; }
    }
}
