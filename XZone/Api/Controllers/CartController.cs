using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using XZone.Api.Models;
using XZone.Application.DTO.CartDTOs;
using XZone.Application.Services;
using XZone.Application.Services.IServices;
using XZone.Infrastructure.Repository;

namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {


        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
           
            _cartService = cartService;
        }
        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartDTO>>> GetMyCart()
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<CartDTO>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _cartService.GetMyCartAsync(userId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<bool>>> AddToCart([FromBody] AddToCartDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _cartService.AddToCartAsync(userId, dto);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("update-quantity")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateQuantity([FromBody] UpdateCartItemQuantityDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _cartService.UpdateCartItemQuantityAsync(userId, dto);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("remove/{gameId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFromCart(int gameId)
        {
            if (gameId <= 0)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Invalid game id." }
                });
            }

            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _cartService.RemoveFromCartAsync(userId, gameId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _cartService.ClearCartAsync(userId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }



    }
}
