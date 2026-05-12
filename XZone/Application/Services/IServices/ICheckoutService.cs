using XZone.Api.Models;
using XZone.Application.DTO.CheckOutDTO;

namespace XZone.Application.Services.IServices
{
    public interface ICheckoutService
    {
        Task<ApiResponse<CheckOutResponseDTOcs>> CheckoutAsync(string userId, CheckoutRequestDTO dto);

        Task<ApiResponse<bool>> ConfirmStripePaymentAsync(string stripeSessionId);
    }
}
