using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibCalBooker.Models;
using LibCalBooker.Utils;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;


namespace LibCalBooker.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	public async Task<IActionResult> Rooms()
	{
    	JArray rooms = await RoomUtils.GetRooms(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
		return View(rooms);
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}