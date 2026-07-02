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
            //slot.IsAvailable = false;

            var booking = new Booking
            {
                UserId = userId,
                SlotId = dto.SlotId,
                BookingDate = DateTime.UtcNow,
                Status = "PendingPayment",
                PaymentStatus = "Unpaid"
            };

            _context.Bookings.Add(booking);

            // Step 5 - Save everything in one transaction
            await _context.SaveChangesAsync();

            return MapToDto(booking, user, slot);
        }

        public async Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId)
        {
            await ExpireOldPendingBookingsAsync(userId);
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return bookings.Select(b => MapToDto(b, b.User!, b.Slot!)).ToList();
        }

        public async Task ExpireOldPendingBookingsAsync(int userId)
        {
            var now = DateTime.UtcNow;

            var expiredBookings = await _context.Bookings
                .Include(b => b.Slot)
                .Where(b => b.UserId == userId
                         && b.Status == "PendingPayment"
                         && b.Slot!.HeldUntil != null
                         && b.Slot.HeldUntil < now)
                .ToListAsync();

            foreach (var booking in expiredBookings)
            {
                booking.Status = "Expired";
                booking.Slot!.HeldUntil = null;
            }

            if (expiredBookings.Any())
                await _context.SaveChangesAsync();
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

        public async Task<MultiBookingResponseDto> CreateMultipleBookingsAsync(int userId, MultiBookingRequestDto dto)
        {
            if (dto.SlotIds == null || !dto.SlotIds.Any())
                throw new Exception("No slots selected.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var now = DateTime.UtcNow;
            // Load all requested slots at once
            var slots = await _context.Slots
                .Include(s => s.Ground)
                .Where(s => dto.SlotIds.Contains(s.SlotId))
                .ToListAsync();

            // Verify all slots exist
            if (slots.Count != dto.SlotIds.Count)
                throw new Exception("One or more slots not found.");

            // Verify all slots are available
            var unavailable = slots.Where(s => !s.IsAvailable || (s.HeldUntil != null && s.HeldUntil > now)).ToList();
            if (unavailable.Any())
                throw new Exception($"Some slots are no longer available. Please refresh and try again.");

            // Create a booking for each slot
            var bookings = new List<Booking>();
            var holdExpiry = now.AddMinutes(3);
            foreach (var slot in slots)
            {
                slot.HeldUntil = holdExpiry;
                var booking = new Booking
                {
                    UserId = userId,
                    SlotId = slot.SlotId,
                    BookingDate = DateTime.UtcNow,
                    Status = "PendingPayment",
                    PaymentStatus = "Unpaid",
                    AmountPaid = slot.Ground!.HourlyRate
                };
                bookings.Add(booking);
            }

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            var totalAmount = slots.Sum(s => s.Ground!.HourlyRate);
            var bookingDtos = bookings.Select((b, i) => MapToDto(b, user, slots[i])).ToList();

            return new MultiBookingResponseDto
            {
                Bookings = bookingDtos,
                TotalAmount = totalAmount,
                BookingIds = bookings.Select(b => b.BookingId).ToList()
            };
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
                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus,
                AmountPaid = booking.AmountPaid,
                HourlyRate = slot.Ground?.HourlyRate ?? 0
            };
        }
    }
}
