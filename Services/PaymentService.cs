using IndoorManagementAPI.Data;
using IndoorManagementAPI.DTOs.Payment;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace IndoorManagementAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int bookingId, int userId)
        {
            // Load booking with related data
            var booking = await _context.Bookings
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new Exception("Booking not found.");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only pay for your own bookings.");

            if (booking.Status != "PendingPayment")
                throw new Exception("This booking has already been paid or cancelled.");

            // Get amount from ground hourly rate
            var amount = booking.Slot!.Ground!.HourlyRate;

            // Configure Stripe secret key
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            // Create PaymentIntent with Stripe
            var options = new PaymentIntentCreateOptions
            {
                // Stripe amounts are in smallest currency unit
                // PKR has no sub-units so multiply by 1
                Amount = (long)(amount * 100),
                Currency = "pkr",
                Metadata = new Dictionary<string, string>
                {
                    { "BookingId", bookingId.ToString() },
                    { "UserId", userId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Store PaymentIntentId on the booking
            booking.PaymentIntentId = paymentIntent.Id;
            booking.AmountPaid = amount;
            await _context.SaveChangesAsync();

            return new PaymentIntentResponseDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PublishableKey = _configuration["StripeSettings:PublishableKey"]!,
                Amount = amount,
                Currency = "pkr"
            };
        }

        public async Task<PaymentIntentResponseDto> CreateMultiPaymentIntentAsync(List<int> bookingIds, int userId)
        {
            // Load all bookings
            var bookings = await _context.Bookings
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .Where(b => bookingIds.Contains(b.BookingId))
                .ToListAsync();

            if (bookings.Count != bookingIds.Count)
                throw new Exception("One or more bookings not found.");

            if (bookings.Any(b => b.UserId != userId))
                throw new UnauthorizedAccessException("Unauthorized.");

            if (bookings.Any(b => b.Status != "PendingPayment"))
                throw new Exception("One or more bookings are not pending payment.");

            var totalAmount = bookings.Sum(b => b.Slot!.Ground!.HourlyRate);

            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalAmount * 100),
                Currency = "pkr",
                Metadata = new Dictionary<string, string>
                {
                    { "BookingIds", string.Join(",", bookingIds) },
                    { "UserId", userId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Store PaymentIntentId on all bookings
            foreach (var booking in bookings)
            {
                booking.PaymentIntentId = paymentIntent.Id;
            }
            await _context.SaveChangesAsync();

            return new PaymentIntentResponseDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PublishableKey = _configuration["StripeSettings:PublishableKey"]!,
                Amount = totalAmount,
                Currency = "pkr"
            };
        }
        public async Task<bool> ConfirmPaymentAsync(ConfirmPaymentDto dto, int userId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(dto.PaymentIntentId);

            if (paymentIntent.Status != "succeeded")
                throw new Exception("Payment has not been completed.");

            var bookingIdsString = paymentIntent.Metadata["BookingIds"];
            var bookingIds = bookingIdsString.Split(',').Select(int.Parse).ToList();

            var bookings = await _context.Bookings
                .Include(b => b.Slot)
                .Where(b => bookingIds.Contains(b.BookingId))
                .ToListAsync();

            if (bookings.Any(b => b.UserId != userId))
                throw new UnauthorizedAccessException("Unauthorized.");

            foreach (var booking in bookings)
            {
                booking.Status = "Confirmed";
                booking.PaymentStatus = "Paid";
                booking.Slot!.IsAvailable = false;
                booking.Slot!.HeldUntil = null; // Clear the hold, it's permanently booked now
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
