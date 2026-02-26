using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyPlansController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PropertyPlansController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ POST: api/PropertyPlans
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePropertyPlanDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subCategory = await _context.PropertySubCategories
            .FirstOrDefaultAsync(s => s.Id == dto.SubCategoryId);

        if (subCategory == null)
            return NotFound("SubCategory not found");

        var plan = new PropertyPlan
        {
            PlanName = dto.PlanName,
            BaseCoverageAmount = dto.BaseCoverageAmount,
            CoverageRate = dto.CoverageRate,
            SubCategoryId = dto.SubCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.PropertyPlans.Add(plan);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            plan.Id,
            plan.PlanName,
            plan.BaseCoverageAmount,
            plan.CoverageRate,
            plan.SubCategoryId
        });
    }
    // ✅ GET: api/PropertyPlans/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlanById(int id)
    {
        var plan = await _context.PropertyPlans.FindAsync(id);

        if (plan == null)
            return NotFound();

        return Ok(plan);
    }
}