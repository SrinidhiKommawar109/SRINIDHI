using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =========================
        // CUSTOMER REGISTER
        // =========================
        public async Task<string> RegisterAsync(RegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == dto.UserName))
                return "Username already exists";

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return "Email already exists";

            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }

        // =========================
        // ADMIN CREATES CLAIMS OFFICER
        // =========================
        public async Task<string> CreateClaimsOfficerAsync(CreateClaimsOfficerDTO dto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == dto.UserName))
                return "Username already exists";

            var officer = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.ClaimsOfficer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(officer);
            await _context.SaveChangesAsync();

            return "Claims Officer created successfully";
        }
        // =========================
        // ADMIN CREATES AGENT
        // =========================
        public async Task<string> CreateAgentAsync(CreateAgentDTO dto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == dto.UserName))
                return "Username already exists";

            var agent = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Agent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(agent);
            await _context.SaveChangesAsync();

            return "Agent created successfully";
        }

        // =========================
        // LOGIN
        // =========================
        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == dto.UserName);

            if (user == null)
                return new LoginResponseDTO();

            if (!user.IsActive)
                return new LoginResponseDTO();

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new LoginResponseDTO();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new LoginResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = user.Role.ToString()
            };
        }
    }
}