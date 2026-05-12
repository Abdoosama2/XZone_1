using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models.DTO.DeviceDTOs
{
    public class DeviceUpdatedDTO
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }


        [MaxLength(500)]
        public string Icon { get; set; }
    }
}
