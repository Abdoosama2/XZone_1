using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.Auth
{
    public class ResetPasswordDTO
    {

        [Required(ErrorMessage ="Please enter your current password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }


        [Required(ErrorMessage = "Please enter your New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
