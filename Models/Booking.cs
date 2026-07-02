namespace IndoorManagementAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int SlotId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "PendingPayment";

        // Payment fields
        public string? PaymentIntentId { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Slot? Slot { get; set; }
    }
}