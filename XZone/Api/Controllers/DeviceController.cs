using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XZone.Api.Models;
using XZone.Application.DTO.DeviceDTOs;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Repository;

namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<DeviceDTO>>>> GetAll()
        {
            var response = await _deviceService.GetAllDevices();

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DeviceDTO>>> GetDeviceById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<DeviceDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Invalid id" }
                });
            }

            var response = await _deviceService.GetDeviceById(id);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<bool>>> CreateDevice([FromBody] DeviceCreateDTO deviceCreateDto)
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

            var response = await _deviceService.CreateDevice(deviceCreateDto);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.Created;
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateDevice(int id, [FromBody] DeviceUpdatedDTO deviceUpdatedDto)
        {
            if (id <= 0 || id != deviceUpdatedDto.Id)
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

            var response = await _deviceService.UpdateDevice(deviceUpdatedDto);

            if (!response.IsSuccess)
            {
                if (response.ErrorMessages.Any(x =>
                        x.Contains("not found", StringComparison.OrdinalIgnoreCase)))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDevice(int id)
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

            var response = await _deviceService.DeleteDevice(id);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }
    }
}
