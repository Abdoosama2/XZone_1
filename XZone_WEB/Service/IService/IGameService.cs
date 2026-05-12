using XZone_WEB.Models.DTO.GameDTOs;

namespace XZone_WEB.Service.IService
{
    public interface IGameService
    {

        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id, string token);

        Task<T> CreateAsync<T>(GameCreateDTO GameDto);

        Task<T> UpdateAsync<T>(GameUpdateDTO GameDto, string token);

        Task<T> DeleteAsync<T>(int id, string token);
    }
}
