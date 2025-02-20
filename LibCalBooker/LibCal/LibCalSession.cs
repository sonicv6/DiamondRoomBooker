using LibCalBooker.Data;
using LibCalBooker.LibCal.Requests;
using LibCalBooker.Models;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;


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
						var filteredSlots = slots.Where(slot => slot["className"] == null);

						//Console.WriteLine($"Filtered Response: {JArray.FromObject(filteredSlots)}");
						foreach (var slot in filteredSlots)
						{
							rooms.Add(new TimeSlot(int.Parse(slot["itemId"].ToString()), DateTime.Parse(slot["start"].ToString()), DateTime.Parse(slot["end"].ToString()), slot["checksum"].ToString()));
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


		// Booking a Room. 
		// TODO : implement the final booking request by integrating the user information.
		// Refactor.
		public async Task<bool> BookRoom(TimeSlot timeSlot, ApplicationUser user = null)
		{

			using HttpClient client = new();
			
			// First POST
			var formData = new Dictionary<string, string>
			{
				{ "add[eid]", timeSlot.roomId.ToString() },
				{ "add[gid]", "5160" },
				{ "add[lid]", "2579" },
				{ "add[start]", timeSlot.startTime.ToString("yyyy-MM-dd HH:mm:ss") },
				{ "add[checksum]", timeSlot.checksum },
				{ "lid", "2579" },
				{ "gid", "0" },
				{ "start", timeSlot.startTime.ToString("yyyy-MM-dd") },
				{ "end", timeSlot.endTime.ToString("yyyy-MM-dd") }
			};

			var content = new FormUrlEncodedContent(formData);
			client.DefaultRequestHeaders.Add("Host", "sheffield.libcal.com");
			client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
			client.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"132\"");
			client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
			client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
			client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.6367.118 Safari/537.36");
			client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "Windows");
			client.DefaultRequestHeaders.Add("Origin", "https://sheffield.libcal.com");
			client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
			client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
			client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
			client.DefaultRequestHeaders.Add("Referer", "https://sheffield.libcal.com/r/new/availability?lid=2579&zone=0&gid=5160&capacity=2");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
			client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
			client.DefaultRequestHeaders.Add("Priority", "u=1, i");
			var request = new SelectedTimeSlotRequest(timeSlot.roomId, timeSlot.startTime, timeSlot.endTime, timeSlot.checksum).GetHttpRequest();
			Console.WriteLine($"Request: {request.Content}");
			HttpResponseMessage response = await client.SendAsync(request);
			string responseBody = await ProcessResponse(response);

			if (response.IsSuccessStatusCode)
			{
				if (response.Content.Headers.ContentType != null)
				{
					if (response.Content.Headers.ContentType.MediaType == "application/json")
					{
						var jsonResponse = JObject.Parse(responseBody);
						var bookings = jsonResponse["bookings"] as JArray;

						// Procceeding with a booking.
						if (bookings != null && bookings.Count > 0)
						{
							var bookingData = bookings[0];
							
							// Send To "Checkout/Baseket"
							var bookingRequest = new ToCheckoutRequest(int.Parse(bookingData["id"].ToString()), int.Parse(bookingData["eid"].ToString()), int.Parse(bookingData["seat_id"].ToString()), int.Parse(bookingData["gid"].ToString()), int.Parse(bookingData["lid"].ToString()), DateTime.Parse(bookingData["start"].ToString()), DateTime.Parse(bookingData["end"].ToString()), bookingData["checksum"].ToString()).GetHttpRequest();
							response = await client.SendAsync(request);
							responseBody = await ProcessResponse(response);


							if (response.IsSuccessStatusCode)
							{
								var sessionId = GetSessionId(responseBody);
								
								if (sessionId != -1)
								{

									Console.WriteLine($"Successfully Found Session ID: {sessionId}.");
										

									throw new NotImplementedException();
									
									// Final Post Request to Checkout and finialise the booking process.
									

								} else { return false; }

							} else 
							{
								Console.WriteLine("Second booking request failed.");
								return false;
							}

						} 
						else 
						{
							Console.WriteLine("No bookings found.");
							return false;
						}

					}
					return false;

				}
				
				return false;

			}
			else
			{
				Console.WriteLine($"Error: {response.StatusCode}");
				return false;
			}

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