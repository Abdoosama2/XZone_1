using System.ComponentModel.DataAnnotations;

namespace XZone_WEB.Models
{
    public class AddRoleDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
