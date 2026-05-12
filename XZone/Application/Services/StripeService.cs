using Microsoft.Extensions.Configuration;
using Stripe;
using XZone.Api.Models;
using XZone.Application.DTO.CheckOutDTO;
using XZone.Application.DTO.StripeDTO;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using static XZone.Domain.Enums.DomainEnums;
using CheckoutSessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using CheckoutSessionLineItemOptions = Stripe.Checkout.SessionLineItemOptions;
using CheckoutSessionLineItemPriceDataOptions = Stripe.Checkout.SessionLineItemPriceDataOptions;
using CheckoutSessionLineItemPriceDataProductDataOptions = Stripe.Checkout.SessionLineItemPriceDataProductDataOptions;
using CheckoutSessionService = Stripe.Checkout.SessionService;

namespace XZone.Application.Services
{
    public class StripeService : IStripeService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitofWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public StripeService(
            IOrderRepository orderRepository,
            IUnitofWork unitOfWork,
            IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ApiResponse<StripeSessionResult>> CreateCheckoutSessionAsync(Order order)
        {
            var response = new ApiResponse<StripeSessionResult>();

            try
            {
                var lineItems = order.Items.Select(item => new CheckoutSessionLineItemOptions
                {
                    PriceData = new CheckoutSessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmountDecimal = item.UnitPrice * 100,
                        ProductData = new CheckoutSessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.GameName
                        }
                    },
                    Quantity = item.Quantity
                }).ToList();

                var successUrl = _configuration["Stripe:SuccessUrl"];
                var cancelUrl = _configuration["Stripe:CancelUrl"];

                var sessionOptions = new CheckoutSessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    CustomerEmail = order.Email,
                    SuccessUrl = $"{successUrl}?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{cancelUrl}?orderId={order.Id}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", order.Id.ToString() },
                        { "UserId", order.UserId }
                    }
                };

                var sessionService = new CheckoutSessionService();
                var session = await sessionService.CreateAsync(sessionOptions);

                response.IsSuccess = true;
                response.Data = new StripeSessionResult
                {
                    SessionId = session.Id,
                    CheckoutUrl = session.Url
                };
            }
            catch (StripeException ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Stripe error: {ex.StripeError.Message}");
            }

            return response;
        }

        public async Task<ApiResponse<bool>> ConfirmStripePaymentAsync(
            Guid orderId,
            string stripeSessionId,
            string stripePaymentIntentId)
        {
            var response = new ApiResponse<bool>();

            if (orderId == Guid.Empty)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid order id.");
                return response;
            }

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add("Order not found.");
                return response;
            }

            // ✅ Idempotent — safe to call multiple times
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                response.IsSuccess = true;
                response.Data = true;
                return response;
            }

            try
            {
                var sessionService = new CheckoutSessionService();
                var session = await sessionService.GetAsync(stripeSessionId);

                if (session.PaymentStatus != "paid")
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Payment not completed.");
                    return response;
                }

                // ✅ Verify session belongs to this order
                if (!session.Metadata.TryGetValue("OrderId", out var metaOrderId)
                    || metaOrderId != orderId.ToString())
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Session does not match order.");
                    return response;
                }

                order.MarkPaymentSucceeded(stripeSessionId, stripePaymentIntentId);

                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

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

        public async Task<ApiResponse<bool>> RefundPaymentAsync(string paymentIntendId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntendId,
                   
                };

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions);

                if (refund.Status == "succeeded" || refund.Status == "pending")
                {
                    response.IsSuccess = true;
                    response.Data = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.ErrorMessages.Add($"Refund failed with status: {refund.Status}");
                }
            }
            catch (StripeException ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages.Add($"Refund failed: {ex.StripeError.Message}");
            }

            return response;
        }
    }
}