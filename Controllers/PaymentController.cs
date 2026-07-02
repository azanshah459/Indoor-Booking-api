using IndoorManagementAPI.DTOs.Payment;
using IndoorManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndoorManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent(
            [FromBody] CreatePaymentIntentDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService
                    .CreatePaymentIntentAsync(dto.BookingId, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("create-multi-intent")]
        public async Task<IActionResult> CreateMultiPaymentIntent(
            [FromBody] List<int> bookingIds)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService
                    .CreateMultiPaymentIntentAsync(bookingIds, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment(
            [FromBody] ConfirmPaymentDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _paymentService.ConfirmPaymentAsync(dto, userId);
                return Ok("Payment confirmed. Booking is now active.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

