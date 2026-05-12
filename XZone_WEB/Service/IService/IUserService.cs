using XZone_WEB.Models;
using XZone_WEB.Models.DTO.UserDto_s;

namespace XZone_WEB.Service.IService
{
    public interface IUserService
    {

        Task<T> LoginAsync<T>(UserLoginDTO userLogin);

        Task<T> RegisterAsync<T>(UserRegistrationDTO userRegister);

        Task<T> AddRoleAsync<T>(AddRoleDTO addRoleDTO);


    }
}
