namespace IndoorManagementAPI.Models
{
    public class Slot
    {
        public int SlotId { get; set; }
        public int GroundId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? HeldUntil { get; set; }

        // Navigation Property
        public Ground? Ground { get; set; }
    }
}
