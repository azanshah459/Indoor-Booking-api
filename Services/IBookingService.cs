using IndoorManagementAPI.DTOs.Booking;

namespace IndoorManagementAPI.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(int userId, BookingRequestDto dto);
        Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId);
        Task<List<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId);
        Task<bool> CancelBookingAsync(int bookingId, int userId);
    }
}
