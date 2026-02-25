using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        
        // Category endpoints
      

        // Admin creates category
        [HttpPost("category")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromQuery] string name)
        {
            await _policyService.CreateCategoryAsync(name);
            return Ok(new
            {
                Message = "Category Created Successfully"
            });
        }

        // All logged-in users can view categories
        [HttpGet("categories")]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _policyService.GetAllCategoriesAsync();

            return Ok(categories);
        }

        
    }
}