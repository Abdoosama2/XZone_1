using Microsoft.EntityFrameworkCore;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Data;

namespace XZone.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Order order)
        {
            _context.Orders.Add(order);
         
        }

        public async Task DeleteAsync(Order order)
        {
           _context.Orders.Remove(order);
         
        }

        public async Task<List<Order>> GetAllWithItemsAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
        {
           return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(x=>x.Id==orderId);
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
          return await _context.Orders.FirstOrDefaultAsync(x=>x.Id == orderId);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
           return await _context.Orders.Where(x=>x.UserId==userId).OrderByDescending(x=>x.OrderDate)
                .ToListAsync();
        }

        

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Update(order);
            
        }

       
    }
}
