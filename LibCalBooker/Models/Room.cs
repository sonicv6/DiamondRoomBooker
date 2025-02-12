using System.ComponentModel.DataAnnotations.Schema;

namespace LibCalBooker.Models
{
	public class Room
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Booking> Bookings { get; set; }
	}
}
