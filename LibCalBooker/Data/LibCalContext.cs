using Microsoft.EntityFrameworkCore;
using LibCalBooker.Models;
namespace LibCalBooker.Data
{
	public class LibCalContext : DbContext
	{
		public LibCalContext(DbContextOptions<LibCalContext> options) : base(options)
		{
		}

		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Room> Rooms { get; set; }
	}
}
