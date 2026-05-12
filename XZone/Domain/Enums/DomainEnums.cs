namespace XZone.Domain.Enums
{
    public class DomainEnums
    {
        public enum OrderStatus
        {
            Pending,
            Confirmed,
            Processing,
            Shipped,
            Delivered,
            Cancelled
        }

        public enum PaymentStatus
        {
            Pending,
            Paid,
            Failed,
            Refunded
        }

    }
}
