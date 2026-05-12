using Microsoft.EntityFrameworkCore;
using XZone.Domain.Entites;

namespace XZone.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<Order?> GetOrderWithItemsAsync(Guid orderId);
        public  Task<List<Order>> GetAllWithItemsAsync();
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
     

        Task SaveChangesAsync();

    }
}
