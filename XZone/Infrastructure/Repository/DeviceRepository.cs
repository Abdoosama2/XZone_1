using XZone.Infrastructure.Data;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;

namespace XZone.Infrastructure.Repository
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(AppDbContext context) : base(context)
        {
        }

    }
}
