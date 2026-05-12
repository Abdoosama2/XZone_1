using XZone.Api.Models;
using XZone.Application.DTO.OrderDTOs;

namespace XZone.Application.Services.IServices
{
    public interface IOrderService
    {

        Task<ApiResponse<List<OrderSummrayDTO>>> GetMyOrdersAsync(string userId);

        Task<ApiResponse<OrderDetailedDTO>> GetOrderByIdAsync(Guid orderId, string userId);

        Task<ApiResponse<List<OrderAdminDTO>>> GetAllOrdersForAdminAsync();

        Task<ApiResponse<OrderAdminDTO>> GetOrderByIdForAdminAsync(Guid orderId);

        public Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId, string userId);
    }
}
