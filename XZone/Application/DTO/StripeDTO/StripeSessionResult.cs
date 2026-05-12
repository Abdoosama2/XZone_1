namespace XZone.Application.DTO.StripeDTO
{
    public class StripeSessionResult
    {

        public string SessionId { get; set; }
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
