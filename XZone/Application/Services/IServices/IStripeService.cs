using XZone.Api.Models;
using XZone.Application.DTO.StripeDTO;
using XZone.Domain.Entites;

namespace XZone.Application.Services.IServices
{
    public interface IStripeService
    {
        Task<ApiResponse<StripeSessionResult>> CreateCheckoutSessionAsync(Order order);
        Task<ApiResponse<bool>> ConfirmStripePaymentAsync(Guid orderId, string stripeSessionId, string stripePaymentIntentId);
        public Task<ApiResponse<bool>> RefundPaymentAsync(string paymentIntendId);

    }
}
