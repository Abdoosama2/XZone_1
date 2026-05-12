using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.DeviceDTOs
{
    public class DeviceUpdatedDTO
    {


        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }


        [MaxLength(500)]
        [Required]
        public string Icon { get; set; }
    }
}
