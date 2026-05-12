using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XZone.Api.Models;
using XZone.Application.DTO.OrderDTOs;
using XZone.Application.Services;
using XZone.Application.Services.IServices;

namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<OrderSummrayDTO>>>> GetMyOrders()
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<List<OrderSummrayDTO>>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _orderService.GetMyOrdersAsync(userId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<ApiResponse<OrderDetailedDTO>>> GetOrderById(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<OrderDetailedDTO>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Invalid order id." }
                });
            }

            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<OrderDetailedDTO>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "User is not authenticated." }
                });
            }

            var response = await _orderService.GetOrderByIdAsync(orderId, userId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<OrderAdminDTO>>>> GetAllOrdersForAdmin()
        {
            var response = await _orderService.GetAllOrdersForAdminAsync();

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("admin/{orderId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderAdminDTO>>> GetOrderByIdForAdmin(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<OrderAdminDTO>
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Invalid order id." }
                });
            }

            var response = await _orderService.GetOrderByIdForAdminAsync(orderId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{id}/Cancel")]

        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.CancelOrderAsync(id, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
