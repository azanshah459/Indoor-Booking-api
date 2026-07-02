namespace IndoorManagementAPI.DTOs.Slot
{
    public class SlotResponseDto
    {
        public int SlotId { get; set; }
        public int GroundId { get; set; }
        public string GroundName { get; set; } = string.Empty;
        public string GroundType { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
