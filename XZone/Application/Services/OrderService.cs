using XZone.Api.Models;
using XZone.Application.DTO.OrderDTOs;
using XZone.Application.Services.IServices;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Repository;
using static XZone.Domain.Enums.DomainEnums;

namespace XZone.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGameRepository gameRepository;
        private readonly IUnitofWork unitofWork;
        private readonly IStripeService stripeService;
        private readonly IEmailService _emailservice;
        public OrderService(IOrderRepository orderRepository, IGameRepository gameRepository, IUnitofWork unitofWork, IStripeService stripeService, IEmailService emailservice)
        {
            this._orderRepository = orderRepository;
            this.gameRepository = gameRepository;
            this.unitofWork = unitofWork;
            this.stripeService = stripeService;
            _emailservice = emailservice;
        }
        public async Task<ApiResponse<List<OrderSummrayDTO>>> GetMyOrdersAsync(string userId)
        {
            var response = new ApiResponse<List<OrderSummrayDTO>>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid user id.");
                return response;
            }

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            response.IsSuccess = true;
            response.Data = orders.Select(order => new OrderSummrayDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString()
            }).ToList();

            return response;
        }

        public async Task<ApiResponse<OrderDetailedDTO>> GetOrderByIdAsync(Guid orderId, string userId)
        {
            var response = new ApiResponse<OrderDetailedDTO>();

            if (orderId == Guid.Empty)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid order id.");
                return response;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid user id.");
                return response;
            }

            var order = await _orderRepository.GetOrderWithItemsAsync(orderId);

            if (order == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Order not found.");
                return response;
            }

            if (order.UserId != userId)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Unauthorized access to this order.");
                return response;
            }

            response.IsSuccess = true;
            response.Data = new OrderDetailedDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Items = order.Items.Select(item => new OrderItemDTO
                {
                    Id = item.Id,
                    GameId = item.GameId,
                    GameName = item.GameName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            return response;
        }

        public async Task<ApiResponse<List<OrderAdminDTO>>> GetAllOrdersForAdminAsync()
        {
            var response = new ApiResponse<List<OrderAdminDTO>>();

            var orders = await _orderRepository.GetAllWithItemsAsync();

            response.IsSuccess = true;
            response.Data = orders.Select(order => new OrderAdminDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StripeSessionId = order.StripeSessionId,
                StripePaymentIntentId = order.StripePaymentIntentId,
                Items = order.Items.Select(item => new OrderItemDTO
                {
                    Id = item.Id,
                    GameId = item.GameId,
                    GameName = item.GameName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            }).ToList();

            return response;
        }

        public async Task<ApiResponse<OrderAdminDTO>> GetOrderByIdForAdminAsync(Guid orderId)
        {
            var response = new ApiResponse<OrderAdminDTO>();

            if (orderId == Guid.Empty)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid order id.");
                return response;
            }

            var order = await _orderRepository.GetOrderWithItemsAsync(orderId);

            if (order == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Order not found.");
                return response;
            }

            response.IsSuccess = true;
            response.Data = new OrderAdminDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StripeSessionId = order.StripeSessionId,
                StripePaymentIntentId = order.StripePaymentIntentId,
                Items = order.Items.Select(item => new OrderItemDTO
                {
                    Id = item.Id,
                    GameId = item.GameId,
                    GameName = item.GameName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            return response;
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId, string userId)
        {
            var response= new ApiResponse<bool>();

            if (orderId == Guid.Empty)
            {
                response.IsSuccess=false;
                response.ErrorMessages.Add("Invalid order Id");
                return response;
            }

            if (string.IsNullOrEmpty(userId))
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid user Id");
                return response;
            }
            var order= await _orderRepository.GetOrderWithItemsAsync(orderId);

            if (order == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("No order with this id");
                return response;

            }

            if (order.UserId != userId)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("No Orders Associated with this customer");
                return response;
            }
            if (order.Status == OrderStatus.Shipped ||
              order.Status == OrderStatus.Delivered ||
             order.Status == OrderStatus.Cancelled)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Cannot cancel an order with status: {order.Status}");
                return response;
            }

            if (order.PaymentStatus == PaymentStatus.Pending)
            {
                order.Cancel();
                await _orderRepository.UpdateAsync(order);
                await unitofWork.SaveChangesAsync();

                response.IsSuccess = true;
                response.Data = true;
                return response;
               
            }
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                using var transaction = await unitofWork.BeginTransactionAsync();
                try
                {
                    // Restore stock
                    var gameIds = order.Items.Select(i => i.GameId).ToList();
                    var games = await gameRepository.GetAllAsync(g => gameIds.Contains(g.Id));
                    var gameDict = games.ToDictionary(g => g.Id);

                    foreach (var item in order.Items)
                    {
                        if (gameDict.TryGetValue(item.GameId, out var game))
                        {
                            game.StockQuantity += item.Quantity;
                            await gameRepository.UpdateAsync(game);
                        }
                    }

                    // Refund via Stripe
                    var refundResult = await stripeService.RefundPaymentAsync(order.StripePaymentIntentId);


                    if (!refundResult.IsSuccess)
                    {
                        await transaction.RollbackAsync(); 
                        response.IsSuccess = false;
                        response.ErrorMessages.AddRange(refundResult.ErrorMessages);
                        return response;
                    }

                    order.MarkAsRefunded();
                    order.Cancel();

                    await _orderRepository.UpdateAsync(order);
                    await unitofWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Failed to cancel order.");
                    return response;
                }
            }
            await _emailservice.SendMessage(
    order.Email,
    $"Order Cancelled — {order.OrderNumber}",
    $"<h1>Your order {order.OrderNumber} has been cancelled.</h1>"
);
            response.IsSuccess = true;
            response.Data = true;
            return response;

        }
    }
}
