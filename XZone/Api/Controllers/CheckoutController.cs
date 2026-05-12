using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XZone.Application.DTO.CheckOutDTO;
using XZone.Application.Services.IServices;

namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _checkoutService.CheckoutAsync(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequestDTO dto)
        {
            var result = await _checkoutService.ConfirmStripePaymentAsync(dto.StripeSessionId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<IActionResult> Success([FromQuery] string sessionId)
        {
            var result = await _checkoutService.ConfirmStripePaymentAsync(sessionId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("cancel")]
        [AllowAnonymous]
        public IActionResult Cancel([FromQuery] Guid orderId)
        {
            return Ok(new { message = "Payment cancelled.", orderId });
        }
    }
}
