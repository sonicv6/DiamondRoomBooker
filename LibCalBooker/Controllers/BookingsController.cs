using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibCalBooker.Data;
using LibCalBooker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LibCalBooker.LibCal;

namespace LibCalBooker.Controllers
{
    public class BookingsController : Controller
    {
        private readonly LibCalContext _context;
        
        private UserManager<ApplicationUser> _userManager;

        private LibCalSession LibCalSession;

        public BookingsController(LibCalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            LibCalSession = new LibCalSession("Balls");
        }

        // GET: Rooms
        [Authorize]
		public async Task<IActionResult> Rooms()
		{
			List<TimeSlot> times = await LibCalSession.GetAvailableRooms(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
			for (int i = 0; i < times.Count; i++)
			{
				var room = _context.Rooms.Find(times[i].roomId);
				if (room != null)
				{
					var timeSlot = times[i];
					timeSlot.diamondName = room.Name;
					times[i] = timeSlot;
				}
			}
            ViewData["Times"] = times;
			return View();
		}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookRoom(TimeSlot timeSlot)
        {
            var result = await LibCalSession.BookRoom(timeSlot);
            return RedirectToAction(nameof(Index));
        }

		// GET: Bookings
		[Authorize]
        public async Task<IActionResult> Index()
        {
            var libCalContext = _context.Bookings.Include(b => b.Room);
            return View(await libCalContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        [Authorize]
        public IActionResult Create()
        {
            TimeSpan start = new TimeSpan(7, 0, 0);
            List<String> times = new List<String>();

			while (start != TimeSpan.FromHours(21)+TimeSpan.FromMinutes(45))
            {
                times.Add(start.ToString());
				start = start.Add(new TimeSpan(0, 15, 0));
			}
            ViewData["Times"] = new SelectList(times);
			ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,BookingDate,BookingTime,RoomID")] Booking booking)
        {
   //         if (_context.Bookings.Where(b => booking.RoomId == b.RoomId && (booking.BookingTime - b.BookingTime).Hours < 4).Any())
			//{
   //             ModelState.AddModelError("BookingTime", "Room is already booked from: " + booking.BookingTime.ToString());
			//}
            
			if (ModelState.IsValid)
            {
                booking.BookerID = (await _userManager.GetUserAsync(User)).Id;
                _context.Add(booking);
                await _context.SaveChangesAsync();
                var test = _context.Bookings.Find(1);
                var test2 = test.Room.Name;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var balls = ModelState.ErrorCount;
            }
			TimeSpan start = new TimeSpan(7, 0, 0);
			List<String> times = new List<String>();

			while (start.Hours < 21 && start.Minutes < 45)
			{
				times.Add(start.ToString());
				start = start.Add(new TimeSpan(0, 15, 0));
			}
			ViewData["Times"] = new SelectList(times);
			ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Name", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,RegistrationNumber,UCardNumber,BookerEmail,SecondaryEmail,BookingTime,RoomId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
