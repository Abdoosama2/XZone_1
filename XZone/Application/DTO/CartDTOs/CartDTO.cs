namespace XZone.Application.DTO.CartDTOs
{
    public class CartDTO
    {
        public int CartId { get; set; }

        public string UserId { get; set; }

        public List<CartItemDTO> Items { get; set; } = new();

        public decimal TotalAmount { get; set; }

    }
}
