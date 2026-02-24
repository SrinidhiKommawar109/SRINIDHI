using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Public registration (Customer only)
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result.Contains("exists"))
                return BadRequest(result);

            return Ok(result);
        }

        // Login (All roles)
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result.Token == null)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }

        // Admin creates Claims Officer
        [Authorize(Roles = "Admin")]
        [HttpPost("create-claims-officer")]
        public async Task<IActionResult> CreateClaimsOfficer(CreateClaimsOfficerDTO dto)
        {
            var result = await _authService.CreateClaimsOfficerAsync(dto);

            if (result.Contains("exists"))
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("create-agent")]
        public async Task<IActionResult> CreateAgent(CreateAgentDTO dto)
        {
            var result = await _authService.CreateAgentAsync(dto);

            if (result.Contains("exists"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}