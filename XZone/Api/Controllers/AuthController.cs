using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using XZone.Api.Models;
using XZone.Application.DTO.Auth;
using XZone.Application.Services.IServices;
using XZone.Models.DTO.UserDto_s;
namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private ApiResponse<AuthResponse> _response;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
            this._response = new ApiResponse<AuthResponse>();
        }
        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value;
        }

        [HttpPost("Register")]

        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(UserRegistrationDTO userRegistrationDTO)
        {
            if (!ModelState.IsValid)
            {

                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                var errors = ModelState.Values
             .SelectMany(v => v.Errors)
             .Select(e => e.ErrorMessage)
             .ToList();
                _response.ErrorMessages = errors;
                return BadRequest(_response);


            }

            var Result = await authService.RegisterAsync(userRegistrationDTO);

            if (!Result.IsSuccess)
            {
                Result.StatusCode = HttpStatusCode.BadRequest;

                return BadRequest(Result);

            }
            Result.StatusCode = HttpStatusCode.Created;
           
            return Ok(Result);

        }

        [HttpPost("Login")]

        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(UserLoginDTO userLoginDTO)
        {
            if (!ModelState.IsValid)
            {

                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                var errors = ModelState.Values
             .SelectMany(v => v.Errors)
             .Select(e => e.ErrorMessage)
             .ToList();
                _response.ErrorMessages = errors;
                return BadRequest(_response);
            }

            _response = await authService.LoginAsync(userLoginDTO);
            if (!_response.IsSuccess)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);

            }
            _response.StatusCode = HttpStatusCode.OK;
       
            return Ok(_response);



        }

        [HttpPost("LogOut")]

        public async Task<ActionResult> SignOut()
        {
            await authService.LogOutAsync();
            return Ok();
        }

        [HttpPost("ResetPassword")]

        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            
            var result= new ApiResponse<bool>();    
            if (!ModelState.IsValid)
            {
                result.IsSuccess = false;
                result.StatusCode=HttpStatusCode.BadRequest;
                result.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage).ToList();
                return BadRequest(result);
            }
            var userId= GetUserId();
            result = await authService.ResetPasswordAsync(userId, resetPasswordDTO);
            if (!result.IsSuccess)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(result);
            }

            return Ok(result);

        }

        [HttpPost("AddRole")]

        public async Task<ActionResult<ApiResponse<bool>>> AddRole(AddRoleDTO addRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                var errors = ModelState.Values
             .SelectMany(v => v.Errors)
             .Select(e => e.ErrorMessage)
             .ToList();
                _response.ErrorMessages = errors;
                return BadRequest(_response);

            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await authService.AddRoleAsync(addRoleDTO);
            if (!String.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
           
            return Ok(addRoleDTO);

        }

        [HttpGet("GetRefreshToken")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RefreshToken()
        {
            var refreshtoken = Request.Cookies["RefreshToken"];
            var Result = await authService.RefreshTokenAsync(refreshtoken);
            if (!Result.IsSuccess)
            {
                return BadRequest(Result.ErrorMessages);
            }
            SetRefreshTokenInCookie(Result.Data.RefreshToken, Result.Data.RefreshTokenExpiration);
            return Ok(Result);
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] TokenRevokeDTO tokenRevokeDTO)
        {

            var RefreshToken = tokenRevokeDTO.Token ?? Request.Cookies["RefreshToken"];

            if (String.IsNullOrEmpty(RefreshToken))
            {
                return BadRequest("Invalid Token");
            }
            var Result = await authService.RevokeTokenAsync(RefreshToken);
            if (!Result)
            {
                return BadRequest("Token is Invalid");
            }
            return Ok();

        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime ExpiresOn)
        {
            var cookieoptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = ExpiresOn.ToLocalTime(),

            };

            Response.Cookies.Append("RefreshToken", refreshToken, cookieoptions);

        }

        
    }
}