using IndoorManagementAPI.Data;
using IndoorManagementAPI.DTOs.Ground;
using IndoorManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IndoorManagementAPI.Services
{
    public class GroundService : IGroundService
    {
        private readonly AppDbContext _context;

        public GroundService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroundResponseDto>> GetAllGroundsAsync()
        {
            var grounds = await _context.Grounds.ToListAsync();

            return grounds.Select(g => new GroundResponseDto
            {
                GroundId = g.GroundId,
                Name = g.Name,
                Type = g.Type,
                Description = g.Description,
                HourlyRate = g.HourlyRate
            }).ToList();
        }

        public async Task<GroundResponseDto?> GetGroundByIdAsync(int id)
        {
            var ground = await _context.Grounds.FindAsync(id);

            if (ground == null) return null;

            return new GroundResponseDto
            {
                GroundId = ground.GroundId,
                Name = ground.Name,
                Type = ground.Type,
                Description = ground.Description,
                HourlyRate = ground.HourlyRate
            };
        }

        public async Task<GroundResponseDto> CreateGroundAsync(GroundRequestDto dto)
        {
            var ground = new Ground
            {
                Name = dto.Name,
                Type = dto.Type,
                Description = dto.Description,
                HourlyRate = dto.HourlyRate
            };

            _context.Grounds.Add(ground);
            await _context.SaveChangesAsync();

            return new GroundResponseDto
            {
                GroundId = ground.GroundId,
                Name = ground.Name,
                Type = ground.Type,
                Description = ground.Description,
                HourlyRate = ground.HourlyRate
            };
        }

        public async Task<GroundResponseDto?> UpdateGroundAsync(int id, GroundRequestDto dto)
        {
            var ground = await _context.Grounds.FindAsync(id);

            if (ground == null) return null;

            ground.Name = dto.Name;
            ground.Type = dto.Type;
            ground.Description = dto.Description;
            ground.HourlyRate = dto.HourlyRate;

            await _context.SaveChangesAsync();

            return new GroundResponseDto
            {
                GroundId = ground.GroundId,
                Name = ground.Name,
                Type = ground.Type,
                Description = ground.Description,
                HourlyRate = ground.HourlyRate
            };
        }

        public async Task<bool> DeleteGroundAsync(int id)
        {
            var ground = await _context.Grounds.FindAsync(id);

            if (ground == null) return false;

            _context.Grounds.Remove(ground);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}