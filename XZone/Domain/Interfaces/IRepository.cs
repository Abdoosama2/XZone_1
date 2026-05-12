using System.Linq.Expressions;

namespace XZone.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {

        public Task<T> CreateAsync(T entity);

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? IncludeProperties = null);
        
        public Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? IncludeProperties = null);

        public Task DeleteAsync(T entity);

        public Task UpdateAsync(T entity);

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null,
         string? includeProperties = null);




    }
}
