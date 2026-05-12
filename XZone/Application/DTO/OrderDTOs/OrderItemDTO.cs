namespace XZone.Application.DTO.OrderDTOs
{
    public class OrderItemDTO
    {
        public Guid Id { get; set; }

        public int GameId { get; set; }

        public string GameName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

    }
}
