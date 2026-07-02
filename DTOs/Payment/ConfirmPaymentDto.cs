namespace IndoorManagementAPI.DTOs.Payment
{
    public class ConfirmPaymentDto
    {
        public List<int> BookingIds { get; set; } = new();
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
