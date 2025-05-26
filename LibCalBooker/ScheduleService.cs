using LibCalBooker.Data;
using LibCalBooker.LibCal;
using System.Reflection.Metadata.Ecma335;

namespace LibCalBooker
{
	public class ScheduleService
	{
		private LibCalContext ctx;

		public ScheduleService(LibCalContext c)
		{
			ctx = c;
		}

		public async Task CreateScheduledBookings(int numTries, int delay = 30)
		{
			Console.WriteLine("Attempting to book scheduled rooms");
			int attempts = 0;
			while (attempts++ < numTries)
			{
				if (!ctx.Bookings.Any()) continue;
				await LibCalSession.BookScheduledRooms(ctx);
				await Task.Delay(delay * 1000);
			}
		}
	}
}
