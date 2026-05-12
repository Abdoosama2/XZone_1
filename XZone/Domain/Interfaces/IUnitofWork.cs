using Microsoft.EntityFrameworkCore.Storage;

namespace XZone.Domain.Interfaces
{
    public interface IUnitofWork
    {

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}
