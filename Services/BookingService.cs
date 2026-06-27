using IndoorManagementAPI.Data;
using IndoorManagementAPI.DTOs.Booking;
using IndoorManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IndoorManagementAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(int userId, BookingRequestDto dto)
        {
            // Step 1 - Find the slot
            var slot = await _context.Slots
                .Include(s => s.Ground)
                .FirstOrDefaultAsync(s => s.SlotId == dto.SlotId);

            if (slot == null)
                throw new Exception("Slot not found.");

            // Step 2 - Check availability
            if (!slot.IsAvailable)
                throw new Exception("This slot is already booked.");

            // Step 3 - Find the user
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            // Step 4 - Mark slot as unavailable and create booking
            slot.IsAvailable = false;

            var booking = new Booking
            {
                UserId = userId,
                SlotId = dto.SlotId,
                BookingDate = DateTime.Now,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);

            // Step 5 - Save everything in one transaction
            await _context.SaveChangesAsync();

            return MapToDto(booking, user, slot);
        }

        public async Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return bookings.Select(b => MapToDto(b, b.User!, b.Slot!)).ToList();
        }

        public async Task<List<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .ToListAsync();

            return bookings.Select(b => MapToDto(b, b.User!, b.Slot!)).ToList();
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return null;

            return MapToDto(booking, booking.User!, booking.Slot!);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Slot)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new Exception("Booking not found.");

            // Make sure the user owns this booking
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own bookings.");

            if (booking.Status == "Cancelled")
                throw new Exception("Booking is already cancelled.");

            // Mark slot as available again
            booking.Status = "Cancelled";
            booking.Slot!.IsAvailable = true;

            await _context.SaveChangesAsync();

            return true;
        }

        private BookingResponseDto MapToDto(Booking booking, User user, Slot slot)
        {
            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                CustomerName = user.Name,
                SlotId = booking.SlotId,
                GroundName = slot.Ground?.Name ?? string.Empty,
                GroundType = slot.Ground?.Type ?? string.Empty,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                BookingDate = booking.BookingDate,
                Status = booking.Status
            };
        }
    }
}
