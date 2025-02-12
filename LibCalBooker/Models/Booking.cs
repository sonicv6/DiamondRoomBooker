using System.ComponentModel.DataAnnotations;

namespace LibCalBooker.Models
{
	public class Booking
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int RegistrationNumber { get; set; }
		public int UCardNumber { get; set; }
		public string BookerEmail { get; set; }
		public string SecondaryEmail { get; set; }
		[DataType(DataType.Date)]
		public DateTime BookingDate { get; set; }
		[DataType(DataType.Time)]
		public DateTime BookingTime { get; set; }

		public int RoomID { get; set; }
		public Room Room { get; set; } = null!;
	}
}
