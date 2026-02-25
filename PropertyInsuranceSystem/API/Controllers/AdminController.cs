using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("dashboard")]
    public IActionResult AdminDashboard()
    {
        return Ok("Welcome Admin");
    }

    [Authorize(Roles = "Agent")]
    [HttpGet("agent-area")]
    public IActionResult AgentArea()
    {
        return Ok("Welcome Agent");
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("customer-area")]
    public IActionResult CustomerArea()
    {
        return Ok("Welcome Customer");
    }
}