using AutoMapper;
using XZone.Api.Models;
using XZone.Application.DTO.DeviceDTOs;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Repository;

namespace XZone.Application.Services.IServices
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository deviceRepository;
        private readonly IMapper _mapper;

        public DeviceService(IDeviceRepository deviceRepository, IMapper mapper)
        {
            this.deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<bool>> CreateDevice(DeviceCreateDTO device)
        {
            var result = new ApiResponse<bool>();

            if (device == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device data is null");
                return result;
            }

            var existingDevice = await deviceRepository.GetAsync(x => x.Name == device.Name);
            if (existingDevice != null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device already exists");
                return result;
            }

            var newDevice = new Device
            {
                Name = device.Name,
                Icon = device.Icon
            };

            await deviceRepository.CreateAsync(newDevice);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

        public async Task<ApiResponse<bool>> DeleteDevice(int id)
        {
            var result = new ApiResponse<bool>();

            var device = await deviceRepository.GetAsync(x => x.Id == id);

            if (device == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device not found");
                return result;
            }

            await deviceRepository.DeleteAsync(device);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

      
        
          public async Task<ApiResponse<List<DeviceDTO>>> GetAllDevices()
          {
            var result = new ApiResponse<List<DeviceDTO>>();
            var devicesList = await deviceRepository.GetAllAsync();

            result.IsSuccess = true;
            result.Data = _mapper.Map<List<DeviceDTO>>(devicesList);
            return result;
           }
        

        public async Task<ApiResponse<DeviceDTO>> GetDeviceById(int id)
        {
            var result = new ApiResponse<DeviceDTO>();

            var device = await deviceRepository.GetAsync(x => x.Id == id);

            if (device == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device not found");
                return result;
            }

            result.IsSuccess = true;
            result.Data = _mapper.Map<DeviceDTO>(device);
            return result;
        }

        public async Task<ApiResponse<bool>> UpdateDevice(DeviceUpdatedDTO device)
        {
            var result = new ApiResponse<bool>();

            if (device == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device data is null");
                return result;
            }

            var existingDevice = await deviceRepository.GetAsync(x => x.Id == device.Id);

            if (existingDevice == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device not found");
                return result;
            }

            var duplicatedDevice = await deviceRepository.GetAsync(
                x => x.Name == device.Name && x.Id != device.Id);

            if (duplicatedDevice != null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Device name already exists");
                return result;
            }

            existingDevice.Name = device.Name;
            existingDevice.Icon = device.Icon;

            await deviceRepository.UpdateAsync(existingDevice);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }
    }
}
