namespace XZone.Application.DTO.CartDTOs
{
    public class CartItemDTO
    {
        public int GameId { get; set; }

        public string GameName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}