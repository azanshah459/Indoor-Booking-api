namespace IndoorManagementAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int SlotId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Navigation Properties
        public User? User { get; set; }
        public Slot? Slot { get; set; }
    }
}
