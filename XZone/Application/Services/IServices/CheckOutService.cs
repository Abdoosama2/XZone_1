using Stripe;
using Stripe.Climate;
using XZone.Api.Models;
using XZone.Application.DTO.CheckOutDTO;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using static XZone.Domain.Enums.DomainEnums;
using Order = XZone.Domain.Entites.Order;
using CheckoutSessionService = Stripe.Checkout.SessionService;
namespace XZone.Application.Services
{
    public class CheckOutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IUnitofWork _unitOfWork;
        private readonly IStripeService _stripeService; 
        private readonly IEmailService _emailService;

        public CheckOutService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IGameRepository gameRepository,
            IUnitofWork unitOfWork,
            IStripeService stripeService,
            IEmailService emailService)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _stripeService = stripeService;
            _emailService = emailService;
        }

        public async Task<ApiResponse<CheckOutResponseDTOcs>> CheckoutAsync(string userId, CheckoutRequestDTO dto)
        {
            var response = new ApiResponse<CheckOutResponseDTOcs>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid user id.");
                return response;
            }

            if (dto == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Checkout data is null.");
                return response;
            }

            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Cart not found.");
                return response;
            }

            if (cart.Items == null || !cart.Items.Any())
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Cart is empty.");
                return response;
            }

            var gameIds = cart.Items.Select(i => i.GameId).ToList();
            var games = await _gameRepository.GetAllAsync(g => gameIds.Contains(g.Id));
            var gameDict = games.ToDictionary(g => g.Id);

            foreach (var item in cart.Items)
            {
                if (!gameDict.TryGetValue(item.GameId, out var game))
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add($"Game with id {item.GameId} was not found.");
                    return response;
                }

                if (game.StockQuantity < item.Quantity)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add($"Not enough stock for game: {item.GameName}.");
                    return response;
                }
            }

            var order = new Order(userId, dto.CustomerName, dto.Email);

            foreach (var item in cart.Items)
            {
                order.AddOrderItem(item.GameId, item.GameName, item.Quantity, item.UnitPrice);
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _orderRepository.CreateAsync(order);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                response.IsSuccess = false;
                response.ErrorMessages.Add("Checkout failed. Please try again.");
                return response;
            }

            var stripeResponse = await _stripeService.CreateCheckoutSessionAsync(order);

            if (!stripeResponse.IsSuccess)
            {
                order.MarkPaymentFailed();
               await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                response.IsSuccess = false;
                response.ErrorMessages.AddRange(stripeResponse.ErrorMessages);
                return response;
            }

            order.SetStripeSession(stripeResponse.Data.SessionId);
           await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendMessage(order.Email, $"{order.OrderNumber}",
                $"<h1>Your order {order.OrderNumber} is confirmed!</h1>");

            response.IsSuccess = true;
            response.Data = new CheckOutResponseDTOcs
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                StripeSessionId = stripeResponse.Data.SessionId,
                CheckoutUrl = stripeResponse.Data.CheckoutUrl
            };

            return response;
        }

       
        private async Task RollbackOrderAsync(Order order, Dictionary<int, Game> gameDict)
        {
            // Restore stock 
            foreach (var item in order.Items)
            {
                var game = gameDict[item.GameId];
                game.StockQuantity += item.Quantity; 
                await _gameRepository.UpdateAsync(game);
            }

            // Mark order as failed 
            order.MarkPaymentFailed();
            await _orderRepository.UpdateAsync(order);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ApiResponse<bool>> ConfirmStripePaymentAsync(string stripeSessionId)
        {
            var response = new ApiResponse<bool>();

            if (string.IsNullOrWhiteSpace(stripeSessionId))
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid Stripe session id.");
                return response;
            }

            try
            {
                var sessionService = new CheckoutSessionService();
                var session = await sessionService.GetAsync(stripeSessionId);

                if (session == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Stripe session not found.");
                    return response;
                }

                if (!session.Metadata.TryGetValue("OrderId", out var metaOrderId)
                    || !Guid.TryParse(metaOrderId, out var orderId))
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Session does not match order.");
                    return response;
                }

                var order = await _orderRepository.GetOrderWithItemsAsync(orderId);

                if (order == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Order not found.");
                    return response;
                }

                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    return response;
                }

                if (session.PaymentStatus != "paid")
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Payment not completed.");
                    return response;
                }

                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var gameIds = order.Items.Select(i => i.GameId).ToList();
                    var games = await _gameRepository.GetAllAsync(g => gameIds.Contains(g.Id));
                    var gameDict = games.ToDictionary(g => g.Id);

                    foreach (var item in order.Items)
                    {
                        if (gameDict.TryGetValue(item.GameId, out var game))
                        {
                            game.StockQuantity -= item.Quantity;
                            await _gameRepository.UpdateAsync(game);
                        }
                    }

                    var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(order.UserId);
                    if (cart != null)
                    {
                        await _cartRepository.DeleteAsync(cart);
                    }

                    var paymentIntentId = session.PaymentIntentId;
                    order.MarkPaymentSucceeded(session.Id, paymentIntentId);

                 await   _orderRepository.UpdateAsync(order);

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Failed to confirm payment. Please contact support.");
                    return response;
                }
                await _emailService.SendMessage(
                                         order.Email,
                 $"Payment Received — {order.OrderNumber}",
                 $"<h1>Payment of ${order.TotalAmount} received!</h1>"
                 );
                response.IsSuccess = true;
                response.Data = true;
            }
            catch (StripeException ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Stripe verification failed: {ex.StripeError.Message}");
            }
           

            return response;
        }
    }
}