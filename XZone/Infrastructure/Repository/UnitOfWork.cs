using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Data;

namespace XZone.Infrastructure.Repository
{
    public class UnitOfWork : IUnitofWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
           return await _dbContext.Database.BeginTransactionAsync();
        }
       
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
