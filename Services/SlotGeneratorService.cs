using IndoorManagementAPI.Data;
using IndoorManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IndoorManagementAPI.Services
{
    public class SlotGeneratorService : ISlotGeneratorService
    {
        private readonly AppDbContext _context;

        public SlotGeneratorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateSlotsForDateAsync(int groundId, DateTime date)
        {
            var existingSlots = await _context.Slots
                .Where(s => s.GroundId == groundId && s.Date.Date == date.Date)
                .ToListAsync();

            if (existingSlots.Any()) return;

            var slots = new List<Slot>();

            // 12 AM to 6 AM (6 slots) — comes first in display
            for (int hour = 0; hour < 6; hour++)
            {
                slots.Add(new Slot
                {
                    GroundId = groundId,
                    Date = date.Date,
                    StartTime = new TimeSpan(hour, 0, 0),
                    EndTime = new TimeSpan(hour + 1, 0, 0),
                    IsAvailable = true
                });
            }

            // 5 PM to 11 PM (7 slots)
            for (int hour = 17; hour < 24; hour++)
            {
                slots.Add(new Slot
                {
                    GroundId = groundId,
                    Date = date.Date,
                    StartTime = new TimeSpan(hour, 0, 0),
                    // For 11PM slot EndTime is next hour = 24:00:00
                    // SQL Server doesn't support TimeSpan > 23:59:59
                    // So we store it as 23:59:59 instead
                    EndTime = hour == 23
                        ? new TimeSpan(23, 59, 59)
                        : new TimeSpan(hour + 1, 0, 0),
                    IsAvailable = true
                });
            }

            _context.Slots.AddRange(slots);
            await _context.SaveChangesAsync();
        }
    }
}