using LibCalBooker.Data;
using LibCalBooker.LibCal.Requests;
using LibCalBooker.Models;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Hangfire;
using Azure;


namespace LibCalBooker.LibCal;

public static class LibCalSession
{

	public static async Task<List<Booking>> BookScheduledRooms(LibCalContext db)
	{
		var availableRooms = await GetAvailableRooms(DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
		List<Booking> completedBookings = new();
		var bookings = db.Bookings.ToList();

		foreach (var room in availableRooms)
		{
			if (bookings.Count() > 0)
			{
				Booking bookingMatch = null;
				foreach (var booking in bookings)
				{
					if (booking.BookingDate + booking.BookingTime.TimeOfDay == room.startTime && booking.RoomID == room.roomId)
					{
						Console.WriteLine("INFO: Attempted to book " + booking);
						bookingMatch = booking;
						if (await BookRoom(room, booking.Booker))
						{
							db.Bookings.Remove(booking);
							await db.SaveChangesAsync();
							completedBookings.Add(booking);
							Console.WriteLine("Booking Successful");
							break;
						}
						Console.WriteLine("Booking Failed");
					}
				}

				if (bookingMatch != null) bookings.Remove(bookingMatch);
			}
		}
		return completedBookings;
	}

	public static async Task<List<TimeSlot>> GetAvailableRooms(DateTime startDate, DateTime endDate)
	{

		// Date format for the API
		using HttpClient client = new();
		Console.WriteLine("Sending request to get rooms availability...");
		Console.WriteLine($"Current date: {startDate}");
		Console.WriteLine($"End date: {endDate}");

		// Request Body
		var request = new GetRoomsRequest(startDate, endDate).GetHttpRequest();
		HttpResponseMessage response = await client.SendAsync(request);
		string responseBody = await DecodeResponseBody(response);
		List<TimeSlot> rooms = new();
		if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType == "application/json")
		{
			//Console.WriteLine($"JSON Response: {responseBody}");
			// Parse the JSON response and return slots that are bookable.
			var jsonResponse = JObject.Parse(responseBody);
			var slots = jsonResponse["slots"] as JArray;
			if (slots != null)
			{
				var filteredSlots = slots.Where(slot => slot["className"] == null);

				//Console.WriteLine($"Filtered Response: {JArray.FromObject(filteredSlots)}");
				foreach (var slot in filteredSlots)
				{
					rooms.Add(new TimeSlot(int.Parse(slot["itemId"].ToString()), DateTime.Parse(slot["start"].ToString()), DateTime.Parse(slot["end"].ToString()), slot["checksum"].ToString()));
				}
			}
		}
		return rooms;

	}


	// Booking a Room. 
	// TODO : implement the final booking request by integrating the user information.
	// Refactor.
	public static async Task<bool> BookRoom(TimeSlot timeSlot, ApplicationUser user)
	{
		using HttpClient client = new();
		JToken bookingData = await GetBookingData(client, timeSlot);
		if (bookingData == null) return false;
		// Procceeding with a booking.
		HttpResponseMessage basketResponse = await GoToBasketRequest(client, bookingData);

		if (basketResponse.IsSuccessStatusCode)
		{
			var sessionId = GetSessionId(await DecodeResponseBody(basketResponse));

			Console.WriteLine("Failed to obtain Session ID");
			if (sessionId == -1) return false;

			Console.WriteLine($"Successfully Found Session ID: {sessionId}.");

			// Final Post Request to Checkout and finialise the booking process.

			var bookRoomRequest = new BookRoomRequest(sessionId, bookingData, user).GetHttpRequest();
			HttpResponseMessage bookingResponse = await client.SendAsync(bookRoomRequest);

			if (bookingResponse.IsSuccessStatusCode)
			{
				Console.WriteLine("Booking request successful.");
				return true;
			}
			Console.WriteLine(bookingResponse.Content.ReadAsStringAsync().Result);
			if ((await bookingResponse.Content.ReadAsStringAsync()).Contains(
				    "Sorry, this exceeds the 240 minute per day limit in this category."))
			{
				int atPosition = user.Email.IndexOf("@");
				user.Email = user.Email.Insert(atPosition, "+");
				await BookRoom(timeSlot, user);
			}
			Console.WriteLine("booking request failed.");

		}
		else
		{
			Console.WriteLine("Basket attempt failed");
		}
		return false;
	}





	private static async Task<JToken> GetBookingData(HttpClient client, TimeSlot timeSlot)
	{
		HttpResponseMessage selectRoomResponse = await SelectRoom(timeSlot, client);
		if (selectRoomResponse.IsSuccessStatusCode && selectRoomResponse.Content.Headers.ContentType != null && selectRoomResponse.Content.Headers.ContentType.MediaType == "application/json")
		{
			var jsonResponse = JObject.Parse(await DecodeResponseBody(selectRoomResponse));
			var bookings = jsonResponse["bookings"] as JArray;

			var bookingData = bookings[0];
			var bookingDataParse = bookingData.DeepClone();

			// Remove unnecessary properties
			bookingDataParse["options"]?.Parent.Remove();
			bookingDataParse["optionSelected"]?.Parent.Remove();
			bookingDataParse["optionChecksums"]?.Parent.Remove();
			bookingDataParse["cost"]?.Parent.Remove();
			return bookingDataParse;
		}
		Console.WriteLine($"Error: {selectRoomResponse.StatusCode}");
		return null;
	}

	// Helper function to go to the "checkout/basket" stage.
	private static async Task<HttpResponseMessage> GoToBasketRequest(HttpClient client, JToken bookingData)
	{
		// Send To "Checkout/Baseket"
		var bookingRequest = new ToCheckoutRequest(
			int.Parse(bookingData["id"].ToString()),
			int.Parse(bookingData["eid"].ToString()),
			int.Parse(bookingData["seat_id"].ToString()),
			int.Parse(bookingData["gid"].ToString()),
			int.Parse(bookingData["lid"].ToString()),
			DateTime.Parse(bookingData["start"].ToString()),
			DateTime.Parse(bookingData["end"].ToString()),
			bookingData["checksum"].ToString()
		).GetHttpRequest();

		HttpResponseMessage response = await client.SendAsync(bookingRequest);
		return response;
	}

	// Helper Function For Selecting a Room Request.
	private static async Task<HttpResponseMessage> SelectRoom(TimeSlot timeSlot, HttpClient client)
	{
		var request = new SelectedTimeSlotRequest(timeSlot.roomId, timeSlot.startTime, timeSlot.endTime, timeSlot.checksum).GetHttpRequest();
		Console.WriteLine($"Request: {request.Content}");
		HttpResponseMessage response = await client.SendAsync(request);

		return response;
	}

	private static int GetSessionId(string htmlString)
	{
		Console.WriteLine("Getting session ID...");
		//Console.WriteLine(htmlString);

		Regex regex = new Regex(@"springySession\.setId\((\d+)\)");

		Match match = regex.Match(htmlString);
		if (match.Success)
		{
			int sessionId = int.Parse(match.Groups[1].Value);
			Console.WriteLine($"Session ID: {sessionId}");
			return sessionId;
		}
		else
		{
			return -1;
		}

	}

	private static async Task<string> DecodeResponseBody(HttpResponseMessage response)
	{
		string responseBody;
		if (response.Content.Headers.ContentEncoding.Contains("gzip"))
		{
			using var responseStream = await response.Content.ReadAsStreamAsync();
			using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);
			using var reader = new StreamReader(decompressedStream);
			responseBody = await reader.ReadToEndAsync();
		}
		else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
		{
			using var responseStream = await response.Content.ReadAsStreamAsync();
			using var decompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress);
			using var reader = new StreamReader(decompressedStream);
			responseBody = await reader.ReadToEndAsync();
		}
		else
		{
			responseBody = await response.Content.ReadAsStringAsync();
		}

		return responseBody;
	}
}