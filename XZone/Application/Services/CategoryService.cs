using AutoMapper;
using XZone.Api.Models;
using XZone.Application.DTO.Category;
using XZone.Application.Services.IServices;
using XZone.Domain.Entites;
using XZone.Domain.Interfaces;
using XZone.Infrastructure.Repository;

namespace XZone.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this._categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<CategoryDTO>>> GetAllCategories()
        {
            var result = new ApiResponse<List<CategoryDTO>>();
            var categoriesList = await _categoryRepository.GetAllAsync();

            result.IsSuccess = true;
            result.Data = _mapper.Map<List<CategoryDTO>>(categoriesList);
            return result;
        }


        public async Task<ApiResponse<CategoryDTO>> GetcategoryById(int id)
        {
            var result = new ApiResponse<CategoryDTO>();

            var category = await _categoryRepository.GetAsync(x => x.Id == id);

            if (category == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid id");
                return result;
            }

            result.IsSuccess = true;
            result.Data = _mapper.Map<CategoryDTO>(category);
            return result;
        }

        public async Task<ApiResponse<bool>> Createcategory(CategoryCreateDto category)
        {
            var result = new ApiResponse<bool>();

            if (category == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Category data is null");
                return result;
            }

            var existingCategory = await _categoryRepository.GetAsync(x => x.Name == category.Name);
            if (existingCategory != null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Category already exists");
                return result;
            }

            var newCategory = new Category
            {
                Name = category.Name
            };

            await _categoryRepository.CreateAsync(newCategory);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

        public async Task<ApiResponse<bool>> Deletecategory(int id)
        {
            var result = new ApiResponse<bool>();

            var category = await _categoryRepository.GetAsync(x => x.Id == id);

            if (category == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid id");
                return result;
            }

            await _categoryRepository.DeleteAsync(category);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }

       

       
        public async Task<ApiResponse<bool>> Updatecategory(CategoryUpdatedDTO category)
        {
            var result = new ApiResponse<bool>();

            var existingCategory = await  _categoryRepository.GetAsync(x => x.Id == category.Id);

            if (existingCategory == null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Invalid id");
                return result;
            }

            var duplicatedCategory = await  _categoryRepository.GetAsync(x => x.Name == category.Name && x.Id != category.Id);
            if (duplicatedCategory != null)
            {
                result.IsSuccess = false;
                result.ErrorMessages.Add("Category name already exists");
                return result;
            }

            existingCategory.Name = category.Name;

            await _categoryRepository.UpdateAsync(existingCategory);

            result.IsSuccess = true;
            result.Data = true;
            return result;
        }
    }
}
