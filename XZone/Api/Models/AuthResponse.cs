namespace XZone.Api.Models
{
    public class AuthResponse
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public List<string>? Roles { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

        public string? Token { get; set; }
    }
}
