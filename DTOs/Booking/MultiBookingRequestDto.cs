namespace IndoorManagementAPI.DTOs.Booking
{
    public class MultiBookingRequestDto
    {
        public List<int> SlotIds { get; set; } = new();
    }
}
