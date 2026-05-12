using Azure;
using XZone.Api.Models;
using XZone.Application.DTO.CartDTOs;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Repository;

namespace XZone.Application.Services
{
    public class CartServicecs : ICartService
    {
        private readonly ICartRepository _cartRepsoitory;
      
        private readonly IGameRepository _gameRepository;

        public CartServicecs(ICartRepository cartrepsoitory, IGameRepository gameRepository)
        {
            _cartRepsoitory = cartrepsoitory;
          
            _gameRepository = gameRepository;
        }

        public async Task<ApiResponse<bool>> AddToCartAsync(string userId, AddToCartDTO dto)
        {
            var result = new ApiResponse<bool>();

        

            var game = await _gameRepository.GetAsync(x => x.Id == dto.GameId);
            if (game == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid Game Id");
                return result;
            }

            if (game.StockQuantity < dto.Quantity)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Not enough stock available");
                return result;
            }

            var cart = await _cartRepsoitory.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart(userId);
                await _cartRepsoitory.CreateAsync(cart);
                
            }

         
            var existItem = cart.Items?.FirstOrDefault(i => i.GameId == dto.GameId);
            var newRequestedQuantity = dto.Quantity;

            if (existItem != null)
            {
                newRequestedQuantity += existItem.Quantity;
            }

            if (game.StockQuantity < newRequestedQuantity)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Requested quantity exceeds available stock");
                return result;
            }

            cart.AddItem(game.Id, game.Name, dto.Quantity, game.Price);

          
            await _cartRepsoitory.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

        public async Task<ApiResponse<bool>> ClearCartAsync(string userId)
        {
            var result= new ApiResponse<bool>();    
            if (string.IsNullOrWhiteSpace(userId))
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid user id.");
                return result;
            }
            var cart =await _cartRepsoitory.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add(" Cart not found");
                return result;

            }
            if (!cart.Items.Any())
            {
                result.IsSuccess = true;
                result.Data = true;
                return result;
            }
            cart.Clear();

            await _cartRepsoitory.DeleteAsync(cart);

            await _cartRepsoitory.SaveChangesAsync();
            result.IsSuccess= true;
            result.Data= true;
            return result;
        }

        public async Task<ApiResponse<CartDTO>> GetMyCartAsync(string userId)
        {
            var result = new ApiResponse<CartDTO>();
            if (string.IsNullOrWhiteSpace(userId))
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid user id.");
                return result;
            }
            var cart = await _cartRepsoitory.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                result.IsSuccess = true;
                result.Data = new CartDTO
                {
                    UserId = userId,
                    Items = new List<CartItemDTO>(),
                    TotalAmount = 0
                };
                return  result;

            }

            result.IsSuccess = true;
            result.Data = new CartDTO
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDTO
                {
                    GameId = i.GameId,
                    GameName = i.GameName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList(),
                TotalAmount = cart.TotalAmount
            };

            return result;

        }

        public async Task<ApiResponse<bool>> RemoveFromCartAsync(string userId, int gameId)
        {
            var result= new ApiResponse<bool>();
            if (string.IsNullOrEmpty(userId))
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid User id");
                return result;
            }
            var cart = await _cartRepsoitory.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Cart Not found ");
                return result;
            }
            var existingItem = cart.Items.FirstOrDefault(i => i.GameId == gameId);
            if (existingItem == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Game not found in cart.");
                return result;
            }

            cart.RemoveItem(gameId);
          
            await _cartRepsoitory.SaveChangesAsync();
            result.IsSuccess = true;
            result.Data = true;
            return result;

        }

        public async Task<ApiResponse<bool>> UpdateCartItemQuantityAsync(string userId, UpdateCartItemQuantityDTO dto)
        {
            var response = new ApiResponse<bool>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid user id.");
                return response;
            }

            if (dto == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Cart item data is null.");
                return response;
            }

            if (dto.GameId <= 0)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid game id.");
                return response;
            }

            if (dto.Quantity <= 0)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Quantity must be greater than zero.");
                return response;
            }

            var cart = await _cartRepsoitory.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Cart not found.");
                return response;
            }

            var game = await _gameRepository.GetAsync(g => g.Id == dto.GameId);

            if (game == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Game not found.");
                return response;
            }

            if (game.StockQuantity < dto.Quantity)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Requested quantity exceeds available stock.");
                return response;
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.GameId == dto.GameId);

            if (existingItem == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Game not found in cart.");
                return response;
            }

            cart.UpdateItemQuantity(dto.GameId, dto.Quantity);

          

            await _cartRepsoitory.SaveChangesAsync();
            response.IsSuccess = true;
            response.Data = true;
            return response;
        }
    }
}
