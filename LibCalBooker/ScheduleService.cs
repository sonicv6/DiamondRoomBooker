using LibCalBooker.Data;
using LibCalBooker.LibCal;

namespace LibCalBooker
{
	public class ScheduleService
	{
		private LibCalContext ctx;

		public ScheduleService(LibCalContext c)
		{
			ctx = c;
		}

		public async Task CreateScheduledBookings()
		{
			int attempts = 0;
			while (attempts++ < 8)
			{
				await LibCalSession.BookScheduledRooms(ctx);
				await Task.Delay(30 * 1000);
			}
		}
	}
}
