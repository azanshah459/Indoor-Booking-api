using IndoorManagementAPI.Data;
using IndoorManagementAPI.DTOs.Slot;
using IndoorManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IndoorManagementAPI.Services
{
    public class SlotService : ISlotService
    {
        private readonly AppDbContext _context;
        private readonly ISlotGeneratorService _slotGenerator;

        public SlotService(AppDbContext context, ISlotGeneratorService slotGenerator)
        {
            _context = context;
            _slotGenerator = slotGenerator;
        }

        public async Task<List<SlotResponseDto>> GetAllSlotsAsync()
        {
            var slots = await _context.Slots
                .Include(s => s.Ground)
                .ToListAsync();

            return slots.Select(s => MapToDto(s)).ToList();
        }

        public async Task<List<SlotResponseDto>> GetSlotsByGroundAsync(int groundId)
        {
            var slots = await _context.Slots
                .Include(s => s.Ground)
                .Where(s => s.GroundId == groundId)
                .ToListAsync();

            return slots.Select(s => MapToDto(s)).ToList();
        }

        public async Task<List<SlotResponseDto>> GetAvailableSlotsByGroundAsync(int groundId, DateTime? date = null)
        {
            // Auto generate slots for this date if they don't exist
            if (date.HasValue)
            {
                await _slotGenerator.GenerateSlotsForDateAsync(groundId, date.Value);
            }

            var query = _context.Slots
                .Include(s => s.Ground)
                .Where(s => s.GroundId == groundId && s.IsAvailable == true);

            if (date.HasValue)
            {
                query = query.Where(s => s.Date.Date == date.Value.Date);
            }

            var slots = await query
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return slots.Select(s => MapToDto(s)).ToList();
        }

        public async Task<SlotResponseDto?> GetSlotByIdAsync(int id)
        {
            var slot = await _context.Slots
                .Include(s => s.Ground)
                .FirstOrDefaultAsync(s => s.SlotId == id);

            if (slot == null) return null;

            return MapToDto(slot);
        }

        public async Task<SlotResponseDto> CreateSlotAsync(SlotRequestDto dto)
        {
            var ground = await _context.Grounds.FindAsync(dto.GroundId);

            if (ground == null)
                throw new Exception($"Ground with ID {dto.GroundId} not found.");

            var slot = new Slot
            {
                GroundId = dto.GroundId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsAvailable = true
            };

            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();

            await _context.Entry(slot).Reference(s => s.Ground).LoadAsync();

            return MapToDto(slot);
        }

        public async Task<bool> DeleteSlotAsync(int id)
        {
            var slot = await _context.Slots.FindAsync(id);

            if (slot == null) return false;

            _context.Slots.Remove(slot);
            await _context.SaveChangesAsync();

            return true;
        }

        private SlotResponseDto MapToDto(Slot slot)
        {
            return new SlotResponseDto
            {
                SlotId = slot.SlotId,
                GroundId = slot.GroundId,
                GroundName = slot.Ground?.Name ?? string.Empty,
                GroundType = slot.Ground?.Type ?? string.Empty,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                IsAvailable = slot.IsAvailable
            };
        }
    }
}