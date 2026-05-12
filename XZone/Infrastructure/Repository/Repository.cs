using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using XZone.Infrastructure.Data;
using XZone.Domain.Interfaces;


namespace XZone.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> dbset;
        public Repository(AppDbContext context)
        {
            this._context = context;
            this.dbset = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? IncludeProperties = null)
        {
            IQueryable<T> query = dbset;

            if (!tracked)
            {
                query=query.AsNoTracking();
            }
            if (filter != null)
            {
                query=query.Where(filter);
            }
            if (IncludeProperties != null)
            {
                foreach (var property in IncludeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries) )
                {
                        
                    query=query.Include(property);
                }

               
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? IncludeProperties = null)
        {
            IQueryable<T> query = dbset;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (IncludeProperties != null)
            {
                foreach (var property in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {

                    query = query.Include(property);
                }


            }
            return await query.FirstOrDefaultAsync();


        }
        public async Task<T> CreateAsync(T entity)
        {
            
          
             await dbset.AddAsync(entity);
           await _context.SaveChangesAsync();
            return entity;

        }

        public async Task DeleteAsync(T entity)
        {
            dbset.Remove(entity);
            await _context.SaveChangesAsync();

        }


        public async Task UpdateAsync(T entity)
        {
          dbset.Update(entity);

            await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbset.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(property);
            }
               
            return query; 
        }
    }
}
