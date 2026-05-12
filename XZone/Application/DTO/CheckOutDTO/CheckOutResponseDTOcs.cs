namespace XZone.Application.DTO.CheckOutDTO
{
    public class CheckOutResponseDTOcs
    {

        public Guid OrderId { get; set; }

        public string OrderNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string? StripeSessionId { get; set; }

        public string? CheckoutUrl { get; set; }
    }
}
