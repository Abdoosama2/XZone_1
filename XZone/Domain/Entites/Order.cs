using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using XZone.Infrastructure.Identity;
using static XZone.Domain.Enums.DomainEnums;

namespace XZone.Domain.Entites
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Order(string userId, string customerName, string email)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required.");

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required.");

            if (customerName.Length > 50)
                throw new ArgumentException("Customer name cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            Id = Guid.NewGuid();
            OrderNumber = $"ORD-{DateTime.UtcNow.Year}-{Id.ToString("N")[..8]}";
            OrderDate = DateTime.UtcNow;
            UserId = userId;
            CustomerName = customerName;
            Email = email;
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
        }

        [Key]
        public Guid Id { get; private set; }
        public string OrderNumber { get; private set; }
        public string UserId { get; private set; }
        [ForeignKey("UserId")] 
        public ApplicationUser? User { get; private set; }
        public string CustomerName { get; private set; }
        public string Email { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public string? StripeSessionId { get; private set; }
        public string? StripePaymentIntentId { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // --- Mutations ---

        public OrderItem AddOrderItem(int gameId, string gameName, int quantity, decimal unitPrice)
        {
            var item = new OrderItem(gameId, gameName);
            item.SetQuantity(quantity);
            item.SetUnitPrice(unitPrice);
            item.SetOrder(this);

            _items.Add(item);
            RecalculateTotalAmount();
            return item;
        }

        public void RemoveOrderItem(Guid orderItemId)
        {
            var item = _items.FirstOrDefault(x => x.Id == orderItemId);
            if (item == null)
                throw new ArgumentException("Order item not found.");

            _items.Remove(item);
            RecalculateTotalAmount();
        }

        public void SetStripeSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Stripe session id is required.");

            StripeSessionId = sessionId;
        }

        public void MarkPaymentSucceeded(string stripeSessionId, string stripePaymentIntentId)
        {
            if (string.IsNullOrWhiteSpace(stripeSessionId))
                throw new ArgumentException("Stripe session id is required.");

            if (string.IsNullOrWhiteSpace(stripePaymentIntentId))
                throw new ArgumentException("Stripe payment intent id is required.");

            StripeSessionId = stripeSessionId;
            StripePaymentIntentId = stripePaymentIntentId;
            PaymentStatus = PaymentStatus.Paid;
            Status = OrderStatus.Confirmed;
        }

        public void MarkPaymentFailed()
        {
            PaymentStatus = PaymentStatus.Failed;
            Status = OrderStatus.Cancelled;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Shipped ||
         Status == OrderStatus.Delivered ||
        Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException($"Cannot cancel an order with status: {Status}");
            }
        

            Status = OrderStatus.Cancelled;
        }

        public void MarkAsRefunded()
        {
            if (PaymentStatus != PaymentStatus.Paid)
                throw new InvalidOperationException("Can only refund a paid order.");

            PaymentStatus = PaymentStatus.Refunded;
        }

        // --- Private ---

        private void RecalculateTotalAmount()
        {
            TotalAmount = _items.Sum(x => x.TotalPrice);


        }
    }
}