using XZone.Api.Models;
using XZone.Application.DTO.Category;
using XZone.Application.DTO.GameDTOs;

namespace XZone.Application.Services.IServices
{
    public interface ICategoryService
    {
        public Task<ApiResponse<List<CategoryDTO>>> GetAllCategories();

        public Task<ApiResponse<CategoryDTO>> GetcategoryById(int id);

        public Task<ApiResponse<bool>> Createcategory(CategoryCreateDto category);

        public Task<ApiResponse<bool>> Deletecategory(int id);

        public Task<ApiResponse<bool>> Updatecategory(CategoryUpdatedDTO category);
    }
}
