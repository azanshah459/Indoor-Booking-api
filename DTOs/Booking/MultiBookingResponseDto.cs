namespace IndoorManagementAPI.DTOs.Booking
{
    public class MultiBookingResponseDto
    {
        public List<BookingResponseDto> Bookings { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public List<int> BookingIds { get; set; } = new();
    }
}
