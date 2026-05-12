using XZone_WEB.Models;
using XZone_WEB.Models.DTO.UserDto_s;
using XZone_WEB.Service.IService;
using XZoneUtility;

namespace XZone_WEB.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private  string _userURL;
        public UserService(IHttpClientFactory httpClientFactory,IConfiguration config) : base(httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this._userURL = config.GetValue<string>("ServiceUrls:XZoneAPI");
        }

        public Task<T> AddRoleAsync<T>(AddRoleDTO addRoleDTO)
        {
            return SendAsync<T>(new ApiRequest
            {
                Data = addRoleDTO,
                ApiType = SD.ApiType.Post,
                URL = _userURL + "/api/Auth/AddRole"

            });
        }

        public Task<T> LoginAsync<T>(UserLoginDTO userLogin)
        {
            return SendAsync<T>(new ApiRequest
            {
                Data = userLogin,
                ApiType = SD.ApiType.Post,
                URL = _userURL + "/api/Auth/Login"

            });
        }

        public Task<T> RegisterAsync<T>(UserRegistrationDTO userRegister)
        {
            return SendAsync<T>(new ApiRequest
            {
                Data=userRegister,
                ApiType=SD.ApiType.Post,
                URL=_userURL+"/api/Auth/Register"

            });
        }
    }
}
