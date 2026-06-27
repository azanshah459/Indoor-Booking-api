using IndoorManagementAPI.DTOs.Slot;
using IndoorManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace IndoorManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var slots = await _slotService.GetAllSlotsAsync();
            return Ok(slots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var slot = await _slotService.GetSlotByIdAsync(id);

            if (slot == null)
                return NotFound($"Slot with ID {id} not found.");

            return Ok(slot);
        }

        [HttpGet("ground/{groundId}")]
        public async Task<IActionResult> GetByGround(int groundId)
        {
            var slots = await _slotService.GetSlotsByGroundAsync(groundId);
            return Ok(slots);
        }

        [HttpGet("ground/{groundId}/available")]
        public async Task<IActionResult> GetAvailableByGround(int groundId,[FromQuery] DateTime? date = null)
        {
            var slots = await _slotService.GetAvailableSlotsByGroundAsync(groundId,date);
            return Ok(slots);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SlotRequestDto dto)
        {
            try
            {
                var slot = await _slotService.CreateSlotAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = slot.SlotId }, slot);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _slotService.DeleteSlotAsync(id);

            if (!result)
                return NotFound($"Slot with ID {id} not found.");

            return NoContent();
        }
    }
}