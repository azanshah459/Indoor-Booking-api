namespace IndoorManagementAPI.DTOs.Slot
{
    public class SlotRequestDto
    {
        public int GroundId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
