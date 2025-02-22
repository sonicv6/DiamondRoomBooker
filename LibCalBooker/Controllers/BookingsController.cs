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


        public BookingsController(LibCalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private void CreateScheduledBookings()
        {

        }

        // GET: Rooms
        [Authorize]
        public async Task<IActionResult> Rooms()
        {
            List<TimeSlot> times = await LibCalSession.GetAvailableRooms(DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
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
            ApplicationUser user = await _userManager.GetUserAsync(User);
            await LibCalSession.BookRoom(timeSlot, user);
			return RedirectToAction(nameof(Index));
        }

		// GET: Bookings
		[Authorize]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
			{
				return RedirectToAction("Identity/Account/Logout");
			}
			var bookings = _context.Bookings.Where(b => b.BookerID == user.Id);
            return View(await bookings.ToListAsync());
        }


        // GET: Bookings/Create
        [Authorize]
        public IActionResult Create()
        {
            TimeSpan start = new TimeSpan(7, 0, 0);
            List<string> times = new List<string>();

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
			booking.BookerID = (await _userManager.GetUserAsync(User)).Id;
			if (ModelState.IsValid || ModelState.ErrorCount == 1)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var balls = ModelState.ErrorCount;
            }
			TimeSpan start = new TimeSpan(7, 0, 0);
			List<string> times = new();

			while (start.Hours < 21 && start.Minutes < 45)
			{
				times.Add(start.ToString());
				start = start.Add(new TimeSpan(0, 15, 0));
			}
			ViewData["Times"] = new SelectList(times);
			ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Name", booking.RoomID);
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
            if (booking.BookerID != (await _userManager.GetUserAsync(User)).Id)
			{
				return Unauthorized();
			}
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
