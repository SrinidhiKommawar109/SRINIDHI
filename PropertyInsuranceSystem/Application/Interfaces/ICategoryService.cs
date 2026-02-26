using Application.DTOs;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
    Task AddSubCategoryAsync(CreateSubCategoryDto dto);
    Task AddPlanAsync(CreatePlanDto dto);
}