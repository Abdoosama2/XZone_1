using XZone.Api.Models;
using XZone.Application.DTO.DeviceDTOs;

namespace XZone.Application.Services.IServices
{
    public interface IDeviceService
    {
        public Task<ApiResponse<List<DeviceDTO>>> GetAllDevices();

        public Task<ApiResponse<DeviceDTO>> GetDeviceById(int id);

        public Task<ApiResponse<bool>> CreateDevice(DeviceCreateDTO device);

        public Task<ApiResponse<bool>> DeleteDevice(int id);

        public Task<ApiResponse<bool>> UpdateDevice(DeviceUpdatedDTO device);

    }
}
