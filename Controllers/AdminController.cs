using IndoorManagementAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndoorManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var today = DateTime.UtcNow.Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var totalBookings = await _context.Bookings.CountAsync();

            var date = DateTime.UtcNow.Date;
            var startOfToday = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var endOfToday = startOfToday.AddDays(1);
            var todayBookings = await _context.Bookings
                .Where(b => b.BookingDate >= startOfToday && b.BookingDate < endOfToday)
                .CountAsync();

            var activeBookings = await _context.Bookings
                .Where(b => b.Status == "Confirmed")
                .CountAsync();

            var cancelledBookings = await _context.Bookings
                .Where(b => b.Status == "Cancelled")
                .CountAsync();

            var totalGrounds = await _context.Grounds.CountAsync();
            var totalUsers = await _context.Users
                .Where(u => u.Role == "Customer")
                .CountAsync();

            var monthlyRevenue = await _context.Bookings
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .Where(b => b.BookingDate >= firstDayOfMonth
                         && b.Status == "Confirmed")
                .SumAsync(b => b.Slot!.Ground!.HourlyRate);

            return Ok(new
            {
                TotalBookings = totalBookings,
                TodayBookings = todayBookings,
                ActiveBookings = activeBookings,
                CancelledBookings = cancelledBookings,
                TotalGrounds = totalGrounds,
                TotalCustomers = totalUsers,
                MonthlyRevenue = monthlyRevenue
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Where(u => u.Role == "Customer")
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("bookings/today")]
        public async Task<IActionResult> GetTodayBookings()
        {
            var today = DateTime.UtcNow.Date;
            var date = DateTime.UtcNow.Date;
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);

            var startOfToday = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var endOfToday = startOfToday.AddDays(1);
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .Where(b => b.BookingDate >= startOfToday && b.BookingDate < endOfToday)
                .Select(b => new
                {
                    b.BookingId,
                    CustomerName = b.User!.Name,
                    GroundName = b.Slot!.Ground!.Name,
                    GroundType = b.Slot.Ground.Type,
                    b.Slot.StartTime,
                    b.Slot.EndTime,
                    b.Status
                })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Ground)
                .Where(b => b.Status == "Confirmed")
                .ToListAsync();

            var monthlyRevenue = bookings
                .GroupBy(b => new
                {
                    b.BookingDate.Year,
                    b.BookingDate.Month
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(b => b.Slot!.Ground!.HourlyRate),
                    TotalBookings = g.Count()
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToList();

            return Ok(monthlyRevenue);
        }
    }
}