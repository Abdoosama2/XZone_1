using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using XZone.Api.Models;
using XZone.Application.DTO.Category;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;

namespace XZone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;



        public CategoryController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
           
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDTO>>>> GetAll()
        {
            var response = await _categoryService.GetAllCategories();

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<CategoryDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Invalid id" }
                });
            }

            var response = await _categoryService.GetcategoryById(id);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<bool>>> CreateCategory([FromBody] CategoryCreateDto categoryCreateDto)
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

            var response = await _categoryService.Createcategory(categoryCreateDto);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = HttpStatusCode.Created;
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
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

            var response = await _categoryService.Deletecategory(id);

            if (!response.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCategory(int id, [FromBody] CategoryUpdatedDTO categoryUpdated)
        {
            if (id <= 0 || id != categoryUpdated.Id)
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

            var response = await _categoryService.Updatecategory(categoryUpdated);

            if (!response.IsSuccess)
            {
                if (response.ErrorMessages.Any(x =>
                        x.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                        x.Contains("invalid id", StringComparison.OrdinalIgnoreCase)))
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