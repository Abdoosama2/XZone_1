using System.ComponentModel.DataAnnotations;

namespace XZone.Application.DTO.CheckOutDTO
{
    public class ConfirmPaymentRequestDTO
    {
        public string StripeSessionId { get; set; }
    }
}
