using Microsoft.AspNetCore.Identity;

namespace XZone_WEB.Models
{
    public class ApplicationUser 
    {

        public string  Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string  Password { get; set; }

    }
}
