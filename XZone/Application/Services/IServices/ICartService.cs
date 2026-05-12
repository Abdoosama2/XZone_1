using XZone.Api.Models;
using XZone.Application.DTO.CartDTOs;

namespace XZone.Application.Services.IServices
{
    public interface ICartService
    {
        Task<ApiResponse<CartDTO>> GetMyCartAsync(string userId);

        Task<ApiResponse<bool>> AddToCartAsync(string userId, AddToCartDTO dto);

        Task<ApiResponse<bool>> UpdateCartItemQuantityAsync(string userId, UpdateCartItemQuantityDTO dto);

        Task<ApiResponse<bool>> RemoveFromCartAsync(string userId, int gameId);

        Task<ApiResponse<bool>> ClearCartAsync(string userId);
    }
}
