using IndoorManagementAPI.DTOs.Payment;

namespace IndoorManagementAPI.Services
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int bookingId, int userId);
        Task<PaymentIntentResponseDto> CreateMultiPaymentIntentAsync(List<int> bookingIds, int userId);
        Task<bool> ConfirmPaymentAsync(ConfirmPaymentDto dto, int userId);
    }
}