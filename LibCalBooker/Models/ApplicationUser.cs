using Microsoft.AspNetCore.Identity;

namespace LibCalBooker.Models;

public class ApplicationUser : IdentityUser
{

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SecondaryEmail { get; set; }
    public int UCardNumber { get; set; }
    public int RegistrationNumber { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; }

    
}