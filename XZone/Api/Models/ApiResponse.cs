using System.Net;

namespace XZone.Api.Models
{
    public class ApiResponse<T>
    {

        public ApiResponse() { ErrorMessages = new List<string>(); }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
        //public string? RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiration { get; set; }


    }
}
