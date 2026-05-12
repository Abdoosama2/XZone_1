namespace XZone.Application.DTO.OrderDTOs
{
    public class OrderAdminDTO
    {

        public Guid Id { get; set; }

        public string OrderNumber { get; set; }

        public string UserId { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string? StripeSessionId { get; set; }

        public string? StripePaymentIntentId { get; set; }

        public List<OrderItemDTO> Items { get; set; } = new();
    }
}
