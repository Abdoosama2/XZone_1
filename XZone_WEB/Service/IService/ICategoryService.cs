using XZone_WEB.Models.DTO.CategoryDTo;

namespace XZone_WEB.Service.IService
{
    public interface ICategoryService
    {

        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id, string token);

        Task<T> CreateAsync<T>(CategoryCreateDto categorydto, string token);

        Task<T> UpdateAsync<T>(CategoryUpdatedDTO categorydto, string token);

        Task<T> DeleteAsync<T>(int id, string token);
    }
}
