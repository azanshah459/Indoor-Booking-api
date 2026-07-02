using IndoorManagementAPI.DTOs.Ground;
using IndoorManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndoorManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class GroundsController : ControllerBase
    {
        private readonly IGroundService _groundService;

        public GroundsController(IGroundService groundService)
        {
            _groundService = groundService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var grounds = await _groundService.GetAllGroundsAsync();
            return Ok(grounds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ground = await _groundService.GetGroundByIdAsync(id);

            if (ground == null)
                return NotFound($"Ground with ID {id} not found.");

            return Ok(ground);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] GroundRequestDto dto)
        {
            var ground = await _groundService.CreateGroundAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = ground.GroundId }, ground);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] GroundRequestDto dto)
        {
            var ground = await _groundService.UpdateGroundAsync(id, dto);

            if (ground == null)
                return NotFound($"Ground with ID {id} not found.");

            return Ok(ground);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _groundService.DeleteGroundAsync(id);

            if (!result)
                return NotFound($"Ground with ID {id} not found.");

            return NoContent();
        }
    }
}

