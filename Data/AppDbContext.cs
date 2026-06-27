using IndoorManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IndoorManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Each DbSet = one table in your database
        public DbSet<User> Users { get; set; }
        public DbSet<Ground> Grounds { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
