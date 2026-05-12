using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XZone.Domain.Entites
{
    public class OrderItem
    {
        private OrderItem() { }

        internal OrderItem(int gameId, string gameName)
        {
            if (gameId <= 0)
                throw new ArgumentException("Invalid game id.");

            if (string.IsNullOrWhiteSpace(gameName))
                throw new ArgumentException("Game name cannot be empty.");

            if (gameName.Length > 100)
                throw new ArgumentException("Max game name length is 100.");

            Id = Guid.NewGuid();
            GameId = gameId;
            GameName = gameName;
        }

        [Key]
        public Guid Id { get; private set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; private set; }

        public Order Order { get; private set; }

        public int GameId { get; private set; }

        public string GameName { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice { get; private set; }

        internal void SetOrder(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            OrderId = order.Id;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be a positive number.");

            Quantity = quantity;
            CalculateTotalPrice();
        }

        public void SetUnitPrice(decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new InvalidOperationException("Unit price must be a positive number.");

            UnitPrice = unitPrice;
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Quantity * UnitPrice;
        }
    }
}