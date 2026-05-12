using Microsoft.AspNetCore.Mvc.RazorPages;
using XZone.Api.Models;
using XZone.Application.DTO.GameDTOs;
using XZone.Application.DTO.QueryDTO;
using XZone.Domain.Entites;

namespace XZone.Application.Services.IServices
{
    public interface IGameService
    {

        public Task<ApiResponse<List<GameDTO>>> GetAllGames(string? IncludeProperties = null);

        public Task<ApiResponse<GameDTO>> GetGameById(int id);

        public Task<ApiResponse<bool> > CreateGame(GameCreateDTO game);

        public Task<ApiResponse<bool>> DeleteGame(int id);

        public Task<ApiResponse<bool>> UpdateGame(GameUpdateDTO game);

        public Task<ApiResponse<PagedResult<GameDTO>>> GetGamesAsync(GameQueryParameters gameQuery);

    }
}
