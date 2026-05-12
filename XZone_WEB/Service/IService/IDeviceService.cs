using XZone_WEB.Models.DTO.DeviceDTOs;

namespace XZone_WEB.Service.IService
{
    public interface IDeviceService
    {

        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id, string token);

        Task<T> CreateAsync<T>(DeviceCreateDTO DeviceDto, string token);

        Task<T> UpdateAsync<T>(DeviceUpdatedDTO DeviceDto, string token);

        Task<T> DeleteAsync<T>(int id, string token);
    }
}
