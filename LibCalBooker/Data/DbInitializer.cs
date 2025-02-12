using LibCalBooker.Models;
namespace LibCalBooker.Data
{
	public class DbInitializer
	{
		public static void Initialize(LibCalContext context)
		{
			context.Database.EnsureCreated();
			if (!context.Rooms.Any())
			{
				Seed(context);
			}
		}

		public static void Seed(LibCalContext context)
		{
			context.Rooms.AddRange(
				new Room { Id = 17630, Name = "Diamond Group Room 2.04" },
				new Room { Id = 17631, Name = "Diamond Group Room 3.12" },
				new Room { Id = 17632, Name = "Diamond Group Room 3.13" },
				new Room { Id = 17633, Name = "Diamond Group Room 3.14" },
				new Room { Id = 17634, Name = "Diamond Group Room 3.16" },
				new Room { Id = 17635, Name = "Diamond Group Room 3.17" },
				new Room { Id = 17636, Name = "Diamond Group Room 4.03" },
				new Room { Id = 17637, Name = "Diamond Group Room 4.04" },
				new Room { Id = 17638, Name = "Diamond Group Room 4.05" },
				new Room { Id = 17639, Name = "Diamond Group Room 4.06" },
				new Room { Id = 17640, Name = "Diamond Group Room 4.23" },
				new Room { Id = 17641, Name = "Diamond Group Room 4.24" }
			);
			context.SaveChanges();
		}
	}
}
