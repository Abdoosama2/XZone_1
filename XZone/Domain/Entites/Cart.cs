using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XZone.Infrastructure.Identity;

namespace XZone.Domain.Entites
{
    public class Cart
    {

        private Cart() { }

        private readonly List<CartItem> _items = new();

        public Cart(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required.");

            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public string UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        [NotMapped]
        public decimal TotalAmount => _items.Sum(x => x.TotalPrice);

        public CartItem AddItem(int gameId, string gameName, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var existingItem = _items.FirstOrDefault(x => x.GameId == gameId);

            if (existingItem != null)
            {
                existingItem.SetQuantity(existingItem.Quantity + quantity);
                Touch();
                return existingItem;
            }

            var item = new CartItem(gameId, gameName, quantity, unitPrice);
            item.SetCart(this);

            _items.Add(item);
            Touch();
            return item;
        }

        public void RemoveItem(int gameId)
        {
            var item = _items.FirstOrDefault(x => x.GameId == gameId);

            if (item == null)
                throw new ArgumentException("Game not found in cart.");

            _items.Remove(item);
            Touch();
        }

        public void UpdateItemQuantity(int gameId, int quantity)
        {
            var item = _items.FirstOrDefault(x => x.GameId == gameId);

            if (item == null)
                throw new ArgumentException("Game not found in cart.");

            if (quantity <= 0)
            {
                _items.Remove(item);
            }
            else
            {
                item.SetQuantity(quantity);
            }

            Touch();
        }

        public void Clear()
        {
            _items.Clear();
            Touch();
        }

        private void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
