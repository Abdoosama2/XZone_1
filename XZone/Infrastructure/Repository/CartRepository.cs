
using Microsoft.EntityFrameworkCore;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Data;
namespace XZone.Infrastructure.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;


        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts.Include(x=>x.Items)
                  .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Cart cart)
        {
           await _context.Carts.AddAsync(cart);
           
        }

        public async Task DeleteAsync(Cart cart)
        {
                 _context.Carts.Remove(cart);
        
        }

        public async Task<Cart?> GetCartByIdAsync(int cartId)
        {
          return  await _context.Carts.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.Id == cartId);
        }

       

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
           
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
