using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.DeviceDTOs
{
    public class DeviceDTO
    {
        public string Name { get; set; }


        [MaxLength(500)]
        public string Icon { get; set; }
    }
}
