namespace XZone.Application.DTO.OrderDTOs
{
    public class OrderSummrayDTO
    {

        public Guid Id { get; set; }

        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }
    }
}
