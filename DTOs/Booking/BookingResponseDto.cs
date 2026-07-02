namespace IndoorManagementAPI.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int SlotId { get; set; }
        public string GroundName { get; set; } = string.Empty;
        public string GroundType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal HourlyRate { get; set; }
    }
}
