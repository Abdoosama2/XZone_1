using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.DeviceDTOs
{
    public class DeviceCreateDTO
    {
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }


        [MaxLength(500)]
        [Required]
        public string Icon { get; set; }
    }
}
