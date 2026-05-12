using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XZone.Domain.Interfaces;
using XZone.Application.DTO.GameDTOs;
using XZone.Api.Models;
using XZone.Application.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using XZone.Application.DTO.QueryDTO;

namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
    
        public GameController( IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]

        public async Task<ActionResult<ApiResponse<PagedResult<GameDTO>>>> GetGames([FromQuery] GameQueryParameters gameQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }
            var response= await _gameService.GetGamesAsync(gameQuery);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }



        [HttpGet("All")]

        public async Task<ActionResult<ApiResponse<List<GameDTO>>>> GetAll()
        {
            var response = await _gameService.GetAllGames(IncludeProperties: "Category");

            if (response == null || !response.IsSuccess)
            {
                response ??= new ApiResponse<List<GameDTO>>();
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GameDTO>>> GetGameById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<GameDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Invalid id" }
                });
            }

            var response = await _gameService.GetGameById(id);

            if (!response.IsSuccess)
            {
                if (response.ErrorMessages.Any(x => x.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateGame([FromBody] GameCreateDTO gameCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _gameService.CreateGame(gameCreateDto);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.Created;
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteGame(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Invalid id" }
                });
            }

            var response = await _gameService.DeleteGame(id);

            if (!response.IsSuccess)
            {
                if (response.ErrorMessages.Any(x => x.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateGame(int id, [FromBody] GameUpdateDTO gameUpdated)
        {
            if (id <= 0 || id != gameUpdated.Id)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Invalid id or mismatched route/body id" }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _gameService.UpdateGame(gameUpdated);

            if (!response.IsSuccess)
            {
                if (response.ErrorMessages.Any(x => x.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

    }
}
