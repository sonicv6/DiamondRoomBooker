using LibCalBooker.Data;
using LibCalBooker.LibCal.Requests;
using LibCalBooker.Models;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.IO.Compression;
namespace LibCalBooker.LibCal
{
	public class LibCalSession
	{
		private readonly string libCalUrl;
		public LibCalSession(string libCalUrl)
		{
			this.libCalUrl = libCalUrl;
		}

		public void BookRoom(int roomId, DateTime startTime)
		{

		}

		public async Task<List<TimeSlot>> GetAvailableRooms(DateTime startDate, DateTime endDate)
		{

			// Date format for the API
			using HttpClient client = new();
			Console.WriteLine("Sending request to get rooms availability...");
			Console.WriteLine($"Current date: {startDate}");
			Console.WriteLine($"End date: {endDate}");

			// Request Body
			var request = new GetRoomsRequest(startDate, endDate).GetHttpRequest();
			HttpResponseMessage response = await client.SendAsync(request);
			string responseBody = await ProcessResponse(response);
			List<TimeSlot> rooms = new();
			if (response.Content.Headers.ContentType != null)
			{
				if (response.Content.Headers.ContentType.MediaType == "application/json")
				{
					//Console.WriteLine($"JSON Response: {responseBody}");
					// Parse the JSON response and return slots that are bookable.
					var jsonResponse = JObject.Parse(responseBody);
					var slots = jsonResponse["slots"] as JArray;
					if (slots != null)
					{
						var filteredSlots = slots.Where(slot => slot["className"] != null && slot["className"].ToString() == "s-lc-eq-checkout");

						//Console.WriteLine($"Filtered Response: {JArray.FromObject(filteredSlots)}");
						foreach (var slot in filteredSlots)
						{
							rooms.Add(new TimeSlot(int.Parse(slot["itemId"].ToString()), DateTime.Parse(slot["start"].ToString()), DateTime.Parse(slot["end"].ToString()), slot["checksum"].ToString(), slot["className"].ToString()));
						}
					}
				}
				else if (response.Content.Headers.ContentType.MediaType == "text/html")
				{
					Console.WriteLine($"HTML Response: {responseBody}");
				}
				else
				{
					Console.WriteLine($"Other Response: {responseBody}");
				}
			}
			return rooms;

		}

		private static async Task<string> ProcessResponse(HttpResponseMessage response)
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

}