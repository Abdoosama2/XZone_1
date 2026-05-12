using System.ComponentModel.DataAnnotations;

namespace XZone.Models.DTO.UserDto_s
{
    public class UserRegistrationDTO
    {
        [Required(ErrorMessage = "Can't be blank")]
        [MaxLength(50)]
        public string  FirstName { get; set; }

        [Required(ErrorMessage = "Can't be blank")]
        [MaxLength(50)]
        public string  LastName { get; set; }

        [Required(ErrorMessage = "Can't be blank")]
        [MaxLength(50)]
        public string  UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]

        public string  Email { get; set; }

        [Required(ErrorMessage = "Can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Can't be blank")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number can only be digits")]
        public string PhoneNumber { get; set; }

    }
}
