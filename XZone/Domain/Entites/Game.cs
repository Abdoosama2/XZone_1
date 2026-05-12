using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace XZone.Domain.Entites
{
    public class Game
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(300)]
        public string ImageURL { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<GameDevice>? Devices { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public List<CartItem> CartItems { get; set; } = new();
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
