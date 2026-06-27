using IndoorManagementAPI.DTOs.Slot;

namespace IndoorManagementAPI.Services
{
    public interface ISlotService
    {
        Task<List<SlotResponseDto>> GetAllSlotsAsync();
        Task<List<SlotResponseDto>> GetSlotsByGroundAsync(int groundId);
        Task<List<SlotResponseDto>> GetAvailableSlotsByGroundAsync(int groundId,DateTime? date = null);
        Task<SlotResponseDto?> GetSlotByIdAsync(int id);
        Task<SlotResponseDto> CreateSlotAsync(SlotRequestDto dto);
        Task<bool> DeleteSlotAsync(int id);
    }
}
