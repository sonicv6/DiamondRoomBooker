using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibCalBooker.Models
{
	public class Booking
	{
		public int Id { get; set; }
		[DataType(DataType.Date)]
		public DateTime BookingDate { get; set; }
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
			return $"Booking {Id} for room {Room?.Name} at time {BookingTime} on date {BookingDate} Booked by {Booker.Email}";
		}
	}
}
