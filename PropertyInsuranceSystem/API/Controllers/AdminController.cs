using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("dashboard")]
    public IActionResult AdminDashboard()
    {
        return Ok("Welcome Admin");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("agents")]
    public async Task<IActionResult> GetAgents()
    {
        var agents = await _context.Users
            .Where(u => u.Role == UserRole.Agent && u.IsActive)
            .Select(u => new { u.Id, u.FullName, u.Email })
            .ToListAsync();
        return Ok(agents);
    }

    [Authorize(Roles = "Agent")]
    [HttpGet("agent-area")]
    public IActionResult AgentArea()
    {
        return Ok("Welcome Agent");
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("customer-area")]
    public async Task<IActionResult> CustomerArea()
    {
        var plans = await _context.PropertyPlans.ToListAsync();
        return Ok(plans);
    }
}