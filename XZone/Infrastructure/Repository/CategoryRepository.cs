using XZone.Infrastructure.Data;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;


namespace XZone.Infrastructure.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
