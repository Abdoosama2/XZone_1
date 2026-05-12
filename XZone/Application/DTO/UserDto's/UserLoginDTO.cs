using System.ComponentModel.DataAnnotations;

namespace XZone.Models.DTO.UserDto_s
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Can't be blank")]
        [DataType(DataType.EmailAddress)]
        public string  Email { get; set; }

        [Required(ErrorMessage = "Can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
    }
}
