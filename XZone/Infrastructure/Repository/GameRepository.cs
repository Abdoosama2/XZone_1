using XZone.Infrastructure.Data;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;

namespace XZone.Infrastructure.Repository
{
    public class GameRepository : Repository<Game>,IGameRepository
    {
        public GameRepository(AppDbContext context) : base(context)
        {
        }


    }
}
