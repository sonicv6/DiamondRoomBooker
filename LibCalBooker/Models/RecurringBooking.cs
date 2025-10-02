using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibCalBooker.Models
{
	public class RecurringBooking
	{
		public int Id { get; set; }

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

        public string Interval { get; set; }

		[DataType(DataType.Time)]
		public DateTime BookingTime { get; set; }

		[ForeignKey("RoomID")]
		public int RoomID { get; set; }
		public virtual Room? Room { get; set; }
		
		[ForeignKey("BookerID")]
		public string BookerID { get; set; }
		public virtual ApplicationUser? Booker { get; set; }

		public override string ToString()
		{
			return $"Recurring {Interval} Booking {Id} for room {Room?.Name} at time {BookingTime} from date {StartDate} to {EndDate} Booked by {Booker.Email}";
		}
	}
}
