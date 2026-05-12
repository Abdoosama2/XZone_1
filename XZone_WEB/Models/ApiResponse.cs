using System.Net;

namespace XZone_WEB.Models
{
    public class ApiResponse
    {

        public ApiResponse() { ErrorMessages = new List<string>(); }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Token { get; set; }
        public object Result { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
