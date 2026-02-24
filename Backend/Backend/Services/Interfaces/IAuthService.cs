using Backend.DTOs;

namespace Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO dto);
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
        Task<string> CreateClaimsOfficerAsync(CreateClaimsOfficerDTO dto);
        Task<string> CreateAgentAsync(CreateAgentDTO dto);
    }
}