using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XZone.Api.Models;
using XZone.Application.DTO.GameDTOs;
using XZone.Application.DTO.QueryDTO;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;

namespace XZone.Application.Services
{
    public class GameService : IGameService
    {

        private readonly IGameRepository gameRepository;
        private IMapper _mapper;

        public GameService(IGameRepository gameRepository, IMapper mapper)
        {
            this.gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<GameDTO>>> GetAllGames(string? IncludeProperties = null)
        {
            var result = new ApiResponse<List<GameDTO>>();
            var gamesList = await gameRepository.GetAllAsync(IncludeProperties: IncludeProperties);

            result.IsSuccess = true;
            result.Data = _mapper.Map<List<GameDTO>>(gamesList);
            return result;
        }

        public async Task<ApiResponse<GameDTO>> GetGameById(int id)
        {
            var result = new ApiResponse<GameDTO>();

            var game = await gameRepository.GetAsync(x => x.Id == id);
            if (game == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid id");
                return result;
            }

            result.IsSuccess = true;
            result.Data = _mapper.Map<GameDTO>(game);
            return result;
        }

        public async Task<ApiResponse<bool>> CreateGame(GameCreateDTO game)
        {
            var result = new ApiResponse<bool>();

            if (game.SelectedDevices == null || !game.SelectedDevices.Any())
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("At least one device must be selected");
                return result;
            }

            var gameDevices = game.SelectedDevices
                .Select(deviceId => new GameDevice
                {
                    DeviceId = deviceId
                }).ToList();

            Game newGame = new Game
            {
                Name = game.Name,
                Description = game.Description,
                CategoryId = game.CategoryId,
                ImageURL = game.ImageURL,
                StockQuantity= game.StockQuantity,
                Price= game.Price,
                Devices = gameDevices
            };

            await gameRepository.CreateAsync(newGame);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

        public async Task<ApiResponse<bool>> DeleteGame(int id)
        {
            var result = new ApiResponse<bool>();
            var game = await gameRepository.GetAsync(x => x.Id == id);

            if (game == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid id");
                return result;
            }

            await gameRepository.DeleteAsync(game);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

        public async Task<ApiResponse<bool>> UpdateGame(GameUpdateDTO game)
        {
            var result = new ApiResponse<bool>();

            if (game.SelectedDevices == null || !game.SelectedDevices.Any())
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("At least one device must be selected");
                return result;
            }

            var existGame = await gameRepository.GetAsync(x => x.Id == game.Id /* , includeProperties: "Devices" */);

            if (existGame == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid id");
                return result;
            }

            existGame.Name = game.Name;
            existGame.ImageURL = game.ImageURL;
            existGame.CategoryId = game.CategoryId;
            existGame.Description = game.Description;
           existGame.Price= game.Price;
            existGame.StockQuantity = game.StockQuantity;
            existGame.Devices ??= new List<GameDevice>();
            existGame.Devices.Clear();

            existGame.Devices = game.SelectedDevices
                .Select(deviceId => new GameDevice
                {
                    GameId = game.Id,
                    DeviceId = deviceId
                }).ToList();

            await gameRepository.UpdateAsync(existGame);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

        public async Task<ApiResponse<PagedResult<GameDTO>>> GetGamesAsync(GameQueryParameters gameQuery)
        {

            var response = new ApiResponse<PagedResult<GameDTO>>();

            var query =  gameRepository.GetQueryable(includeProperties:"Category");

            //Filtering
            if (!string.IsNullOrWhiteSpace(gameQuery.Search))
            {
                query= query.Where(x=>x.Name.Contains( gameQuery.Search));
            }

            if (!string.IsNullOrEmpty(gameQuery.Category))
            {
                query=query.Where(x=>x.Category.Name==gameQuery.Category);
            }

            if (gameQuery.MaxPrice.HasValue)
            {
                query=query.Where(x=>x.Price<=gameQuery.MaxPrice.Value);
            }

            if (gameQuery.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= gameQuery.MinPrice.Value);
            }

            //Ordring
            var sortBy = string.IsNullOrWhiteSpace(gameQuery.SortBy) ? "name" : gameQuery.SortBy.ToLower();


            var isDesc= gameQuery.SortOrder?.ToLower() == "desc";
            query = sortBy switch
            {
                "price" => isDesc ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
                 "name" => isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "category" => isDesc ? query.OrderByDescending(x => x.Category.Name) : query.OrderBy(x => x.Category.Name),
                _ => query.OrderBy(g => g.Name)

            };

            // pagination 
            var totalCount= await query.CountAsync();

            var games = await query.Skip((gameQuery.Page - 1) * gameQuery.PageSize)
                .Take(gameQuery.PageSize).ToListAsync();

            response.IsSuccess = true;

            response.Data = new PagedResult<GameDTO>
            {
                Data = games.Select(x => new GameDTO  
             {
                    Name= x.Name,
                    ImageURL= x.ImageURL,
                    CategoryName=x.Category.Name,
                    price=x.Price,
                    StockQuantity=x.StockQuantity,
                    Description=x.Description,


                }).ToList(),
                PageSize= gameQuery.PageSize,
                CurrentPage=gameQuery.Page,
                TotalCount= totalCount,

            };
            return response;
        }
    }
}
