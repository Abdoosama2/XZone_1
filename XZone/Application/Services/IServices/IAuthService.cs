using System.IdentityModel.Tokens.Jwt;
using XZone.Api.Models;
using XZone.Application.DTO.Auth;
using XZone.Infrastructure.Identity;
using XZone.Models.DTO.UserDto_s;

namespace XZone.Application.Services.IServices
{
    public interface IAuthService
    {
         public Task<ApiResponse<AuthResponse>> RegisterAsync(UserRegistrationDTO userRegistrationDTO);

          Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser appUser);
          Task<ApiResponse<AuthResponse>> LoginAsync(UserLoginDTO userLoginDTO );

           Task LogOutAsync();
          public Task<string> AddRoleAsync(AddRoleDTO addRoleDTO);
          Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string token);

          Task<bool> RevokeTokenAsync(string token);

          Task<ApiResponse<bool>> ResetPasswordAsync(string userId, ResetPasswordDTO resetdto);




    }
}
