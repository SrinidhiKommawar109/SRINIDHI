using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs;
namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllCategoriesAsync();
        return Ok(result);   // ✅ returning DTO
    }
    [HttpPost("add-subcategory")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddSubCategory(CreateSubCategoryDto dto)
    {
        await _categoryService.AddSubCategoryAsync(dto);
        return Ok("SubCategory added successfully");
    }
    [HttpPost("add-plan")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddPlan(CreatePlanDto dto)
    {
        await _categoryService.AddPlanAsync(dto);
        return Ok("Plan added successfully");
    }
}