using XZone_WEB.Models.DTO.CategoryDTo;
using XZone_WEB.Service.IService;
using XZoneUtility;

namespace XZone_WEB.Service
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IHttpClientFactory _factory;
        private string _categoryURL;
        public CategoryService(IHttpClientFactory httpClientFactory ,IConfiguration config) : base(httpClientFactory)
        {
            _factory = httpClientFactory;
            _categoryURL = config.GetValue<string>("ServiceUrls:XZoneAPI");
        }

        public Task<T> CreateAsync<T>(CategoryCreateDto categorydto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Post,
                Data = categorydto,
                URL = _categoryURL + "/api/Category/",
                Token = token,

            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Delete,
               
                URL = _categoryURL + "/api/Category/"+id,
                Token = token,

            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Get,
               
                URL = _categoryURL + "/api/Category/",
                //Token = token,

            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Get,
              
                URL = _categoryURL + "/api/Category/"+id,
                Token = token,

            });
        }

        public Task<T> UpdateAsync<T>(CategoryUpdatedDTO categorydto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Put,
            
                URL = _categoryURL + "/api/Category/"+categorydto.Id,
                Token = token,

            });
        }
    }
}
