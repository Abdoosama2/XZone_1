using XZone_WEB.Models.DTO.DeviceDTOs;
using XZone_WEB.Service.IService;
using XZoneUtility;

namespace XZone_WEB.Service
{
    public class DeviceService : BaseService, IDeviceService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private string _DeviceURL;
        public DeviceService(IHttpClientFactory httpClientFactory,IConfiguration config) : base(httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            _DeviceURL = config.GetValue<string>("ServiceUrls:XZoneAPI");

        }

        public Task<T> CreateAsync<T>(DeviceCreateDTO DeviceDto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Post,
                Data = DeviceDto,
                URL = _DeviceURL + "/api/Device/",
                Token = token,

            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Delete,

                URL = _DeviceURL + "/api/Device/" + id,
                Token = token,

            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Get,

                URL = _DeviceURL + "/api/Device/",
               // Token = token,

            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Get,

                URL = _DeviceURL + "/api/Device/" + id,
                Token = token,

            });
        }

        public Task<T> UpdateAsync<T>(DeviceUpdatedDTO DeviceDto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Put,
                Data = DeviceDto,
                URL = _DeviceURL + "/api/Device/" + DeviceDto.Id,
                Token = token,

            });
        }
    }
}
