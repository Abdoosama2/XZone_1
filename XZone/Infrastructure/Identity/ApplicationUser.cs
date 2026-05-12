using Microsoft.AspNetCore.Identity;
using XZone.Application.DTO.Auth;
using XZone.Domain.Entites;

namespace XZone.Infrastructure.Identity
{
    public class ApplicationUser :IdentityUser
    {

        public string  FirstName { get; set; }
        public string  LastName { get; set; }

        public List<RefreshToken>? RefreshToken { get; set; }


        public List<Order> Orders { get; set; } = new();

        public Cart? Cart { get; set; }

    }
}
