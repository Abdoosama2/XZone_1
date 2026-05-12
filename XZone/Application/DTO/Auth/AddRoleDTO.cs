using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.Auth
{
    public class AddRoleDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
