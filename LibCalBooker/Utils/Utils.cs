using System;
using System.Net.Http;
using System.IO.Compression;
using Newtonsoft.Json.Linq;

namespace LibCalBooker.Utils
{
    public class RoomUtils
    {
        /*
            Given a Date period (StartDate and EndDate)
            this method sends a POST request to the Sheffield LibCal API
            which returns the availability of rooms in the library.
            This function currently filters out unavilable rooms and prints only the bookable slots.

            Example of calling the GetRooms method

            DateTime today = DateTime.UtcNow;
            DateTime tomorrow = today.AddDays(1);

            await GetRooms(today, tomorrow);

        
        */
        public static async Task<JArray> GetRooms(DateTime StartDate, DateTime EndDate)
        {
            
            // Date format for the API
            string startDate = StartDate.ToString("yyyy-MM-dd");
            string endDate = EndDate.ToString("yyyy-MM-dd");

            using HttpClient client = new();

            Console.WriteLine("Sending request to get rooms availability...");
            Console.WriteLine($"Current date: {StartDate}");
            Console.WriteLine($"End date: {EndDate}");
            
            // Request Body
            var parameters = new Dictionary<string, string>
            {
                { "lid", "2579" },
                { "gid", "5160" },
                { "eid", "-1" },
                { "seat", "0" },
                { "seatId", "0" },
                { "zone", "0" },
                { "start", startDate },
                { "end", endDate },
                { "pageIndex", "0" },
                { "pageSize", "18" }
            };

            var content = new FormUrlEncodedContent(parameters);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "UTF-8"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://sheffield.libcal.com/spaces/availability/grid")
            {
                Content = content
            };

            // Post Request Headers
            var headers = new Dictionary<string, string>
            {
                { "Host", "sheffield.libcal.com" },
                { "Sec-Ch-Ua", "\"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"132\"" },
                { "Accept", "application/json, text/javascript, */*; q=0.01" },
                { "X-Requested-With", "XMLHttpRequest" },
                { "Sec-Ch-Ua-Mobile", "?0" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36" },
                { "Sec-Ch-Ua-Platform", "\"Windows\"" },
                { "Origin", "https://sheffield.libcal.com" },
                { "Sec-Fetch-Site", "same-origin" },
                { "Sec-Fetch-Mode", "cors" },
                { "Sec-Fetch-Dest", "empty" },
                { "Referer", "https://sheffield.libcal.com/spaces?lid=2579&gid=5160" },
                { "Accept-Encoding", "gzip, deflate, br" },
                { "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" },
                { "Priority", "u=1, i" }
            };

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            //Console.WriteLine($"Sending request: {request}");

            HttpResponseMessage response = await client.SendAsync(request);
            string responseBody;


            // Decompress the response if it is compressed

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

            //Console.WriteLine($"Response: {response}");
                
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
                        return JArray.FromObject(filteredSlots);

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

            // Return an empty array if no bookable slots are found.
            return new JArray();
           
        }

    }
}