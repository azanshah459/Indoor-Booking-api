using IndoorManagementAPI.DTOs.Ground;

namespace IndoorManagementAPI.Services
{
    public interface IGroundService
    {
        Task<List<GroundResponseDto>> GetAllGroundsAsync();
        Task<GroundResponseDto?> GetGroundByIdAsync(int id);
        Task<GroundResponseDto> CreateGroundAsync(GroundRequestDto dto);
        Task<GroundResponseDto?> UpdateGroundAsync(int id, GroundRequestDto dto);
        Task<bool> DeleteGroundAsync(int id);
    }
}