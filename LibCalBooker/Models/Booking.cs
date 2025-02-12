namespace LibCalBooker.Models
{
	public class Booking
	{
		public int Id { get; set; }
		public int RoomId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int RegistrationNumber { get; set; }
		public int UCardNumber { get; set; }
		public string BookerEmail { get; set; }
		public string SecondaryEmail { get; set; }
		public DateTime BookingTime { get; set; }
	}
}
