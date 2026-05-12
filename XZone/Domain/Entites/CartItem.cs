using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XZone.Domain.Entites
{
    public class CartItem
    {
        private CartItem() { }

        internal CartItem(int gameId, string gameName, int quantity, decimal unitPrice)
        {
            if (gameId <= 0)
                throw new ArgumentException("Invalid game id.");

            if (string.IsNullOrWhiteSpace(gameName))
                throw new ArgumentException("Game name is required.");

            if (gameName.Length > 100)
                throw new ArgumentException("Game name cannot exceed 100 characters.");

            GameId = gameId;
            GameName = gameName;
            SetQuantity(quantity);
            SetUnitPrice(unitPrice);
        }

        [Key]
        public int Id { get; private set; }

        public int CartId { get; private set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; private set; }

        public int GameId { get; private set; }

        public string GameName { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;

        internal void SetCart(Cart cart)
        {
            Cart = cart ?? throw new ArgumentNullException(nameof(cart));
            CartId = cart.Id;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            Quantity = quantity;
        }

        public void SetUnitPrice(decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new InvalidOperationException("Unit price must be greater than zero.");

            UnitPrice = unitPrice;
        }
    }
}