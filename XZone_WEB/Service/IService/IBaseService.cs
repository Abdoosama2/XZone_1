using XZone_WEB.Models;

namespace XZone_WEB.Service.IService
{
    public interface IBaseService
    {

        public ApiRequest _request { get; set; }
        public Task<T> SendAsync<T>(ApiRequest request);
    }
}
