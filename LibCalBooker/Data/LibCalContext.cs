using Microsoft.EntityFrameworkCore;
using LibCalBooker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace LibCalBooker.Data
{
	public class LibCalContext : IdentityDbContext<IdentityUser>
	{
		public LibCalContext(DbContextOptions<LibCalContext> options) : base(options)
		{
		}

		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Room> Rooms { get; set; }

	}
}
