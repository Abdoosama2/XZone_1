using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using XZone.Api.Models;
using XZone.Application.DTO.Auth;
using XZone.Application.Services.IServices;
using XZone.Infrastructure.Identity;
using XZone.Models.DTO.UserDto_s;

namespace XZone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signinManager;

        public AuthService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration config,
            SignInManager<ApplicationUser> signinManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _signinManager = signinManager;
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(UserRegistrationDTO userRegistrationDTO)
        {
            var response = new ApiResponse<AuthResponse>();

            if (userRegistrationDTO == null)
                return Fail(response, "Registration data is null");

            var email = userRegistrationDTO.Email.Trim();
            var userName = userRegistrationDTO.UserName.Trim();

            var existingEmail = await _userManager.FindByEmailAsync(email);
            if (existingEmail is not null)
                return Fail(response, "Email is already registered");

            var existingUserName = await _userManager.FindByNameAsync(userName);
            if (existingUserName is not null)
                return Fail(response, "Username already exists");

            var user = new ApplicationUser
            {
                FirstName = userRegistrationDTO.FirstName.Trim(),
                LastName = userRegistrationDTO.LastName.Trim(),
                UserName = userName,
                Email = email,
                PhoneNumber = userRegistrationDTO.PhoneNumber.Trim()
            };

            var result = await _userManager.CreateAsync(user, userRegistrationDTO.Password);

            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                foreach (var error in result.Errors)
                {
                    response.ErrorMessages.Add(error.Description);
                }
                return response;
            }

            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            var jwtToken = await CreateTokenAsync(user);
            var refreshToken = CreateRefreshToken();

            user.RefreshToken.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            //geting the for the user
            var roles = await _userManager.GetRolesAsync(user);
             
            response.IsSuccess = true;

            response.Data = new AuthResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresON,
                Name = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };
            return response;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(UserLoginDTO userLoginDTO)
        {
            var response = new ApiResponse<AuthResponse>();

            if (userLoginDTO == null)
                return Fail(response, "Login data is null");

            var userInDb = await _userManager.FindByEmailAsync(userLoginDTO.Email.Trim());

            if (userInDb == null || !await _userManager.CheckPasswordAsync(userInDb, userLoginDTO.Password))
                return Fail(response, "The password or email is wrong");

            var jwtToken = await CreateTokenAsync(userInDb);
            var refreshToken = CreateRefreshToken();

            userInDb.RefreshToken.Add(refreshToken);
            await _userManager.UpdateAsync(userInDb);

            //getting roles for the user
            var roles = await _userManager.GetRolesAsync(userInDb);

            response.IsSuccess = true;
           response.Data = new AuthResponse()
           {
               Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
               RefreshToken = refreshToken.Token,
               RefreshTokenExpiration = refreshToken.ExpiresON,
               Name=userInDb.UserName,
               Email=userInDb.Email,
               Roles=roles.ToList()

           };
            

            return response;
        }

        public async Task<string> AddRoleAsync(AddRoleDTO addRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(addRoleDTO.UserId);

            if (user == null || !await _roleManager.RoleExistsAsync(addRoleDTO.Role))
                return "Invalid user or role";

            if (await _userManager.IsInRoleAsync(user, addRoleDTO.Role))
                return "User already has this role";

            var result = await _userManager.AddToRoleAsync(user, addRoleDTO.Role);

            if (!result.Succeeded)
                return "Failed to add role to user";

            return string.Empty;
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string token)
        {
            var response = new ApiResponse<AuthResponse>();

            if (string.IsNullOrWhiteSpace(token))
                return Fail(response, "Invalid token");

            var user = await _userManager.Users
                .Include(x => x.RefreshToken)
                .SingleOrDefaultAsync(x => x.RefreshToken.Any(rt => rt.Token == token));

            if (user == null)
                return Fail(response, "Invalid token");

            var refreshToken = user.RefreshToken.SingleOrDefault(x => x.Token == token);

            if (refreshToken == null || !refreshToken.IsActive)
                return Fail(response, "Inactive token");

            refreshToken.RevokeOn = DateTime.UtcNow;

            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken.Add(newRefreshToken);

            var newJwt = await CreateTokenAsync(user);
            await _userManager.UpdateAsync(user);

            response.IsSuccess = true;
            response.Data = new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(newJwt),
                 RefreshToken = newRefreshToken.Token,
               RefreshTokenExpiration = newRefreshToken.ExpiresON,
            };
          

            return response;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var user = await _userManager.Users
                .Include(x => x.RefreshToken)
                .SingleOrDefaultAsync(x => x.RefreshToken.Any(rt => rt.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshToken.SingleOrDefault(x => x.Token == token);

            if (refreshToken == null || !refreshToken.IsActive)
                return false;

            refreshToken.RevokeOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser appUser)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
                new Claim(ClaimTypes.Name, appUser.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, appUser.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(appUser);

            foreach (var role in roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyString = _config["JWT:Key"];
            if (string.IsNullOrWhiteSpace(keyString))
                throw new InvalidOperationException("JWT:Key is missing from configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials
            );
        }

        private static RefreshToken CreateRefreshToken()
        {
            var randomNumber = RandomNumberGenerator.GetBytes(32);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresON = DateTime.UtcNow.AddHours(2),
                CreatedOn = DateTime.UtcNow
            };
        }

        private static ApiResponse<AuthResponse> Fail(ApiResponse<AuthResponse> response, string error)
        {
            response.IsSuccess = false;
            response.ErrorMessages.Add(error);
            return response;
        }

        public async Task LogOutAsync()
        {
           await _signinManager.SignOutAsync();
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(string userId, ResetPasswordDTO resetdto)
        {
           var result= new ApiResponse<bool>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid User ID");
                return result;

            }
            if (string.IsNullOrWhiteSpace(resetdto.CurrentPassword))
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid Password");
                return result;


            }
            if (string.IsNullOrWhiteSpace(resetdto.NewPassword))
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid Password");
                return result;

            }

            var existUser= await _userManager.FindByIdAsync(userId);
            if (existUser == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("User doesn't Exist");
                return result;

            }
            var identityResult = await _userManager.ChangePasswordAsync(existUser,resetdto.CurrentPassword
                  ,resetdto.NewPassword);

            if (!identityResult.Succeeded)
            {
                result.IsSuccess = false;
                result.ErrorMessages = identityResult.Errors.Select(x => x.Description).ToList<string>();
                return result;
            }

            result.IsSuccess= true;
            result.Data = true;
            return result;


        }
    }
}