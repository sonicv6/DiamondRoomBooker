using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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
		public int BookerID { get; set; }
		public virtual ApplicationUser? Booker { get; set; }
	}
}
