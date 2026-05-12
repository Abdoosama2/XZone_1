using XZone.Domain.Entites;

namespace XZone.Domain.Interfaces
{
    public interface ICartRepository
    {

        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart?> GetCartWithItemsByUserIdAsync(string userId);
        Task<Cart?> GetCartByIdAsync(int cartId);
        Task CreateAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Cart cart);

        Task SaveChangesAsync();
    }
}
