using LibCalBooker.Models;
using Newtonsoft.Json.Linq;

namespace LibCalBooker.LibCal.Requests
{
    public struct BookRoomRequest
    {
        private Dictionary<string, string> parameters = new Dictionary<string, string>
		{


		};
        private Dictionary<string, string> headers = new Dictionary<string, string>
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
            { "Referer", "https://sheffield.libcal.com/r/new/availability?lid=2579&zone=0&gid=5160&capacity=2" },
            { "Accept-Encoding", "gzip, deflate, br" },
            { "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" },
            { "Priority", "u=1, i" }
        };

        public BookRoomRequest(int sessionId, JToken bookingData, ApplicationUser user)
        {

            this.parameters["\"session\""] = sessionId.ToString();
            this.parameters["\"fname\""] = user.FirstName;
            this.parameters["\"lname\""] = user.LastName;
            this.parameters["\"email\""] = user.Email;
            this.parameters["\"q1460\""] = user.SecondaryEmail;
            this.parameters["\"q1282\""] = user.RegistrationNumber.ToString();
            this.parameters["\"q1281\""] = user.UCardNumber.ToString();
            this.parameters["\"bookings\""] = $"[{bookingData.ToString()}]";;
            this.parameters["\"returnUrl\""] = $"/r/new?lid=2579&gid=5160&zone=0&capacity=2&date={bookingData["start"]}&start=&end=";
            this.parameters["\"pickupHolds\""] = "";
            this.parameters["\"method\""] = "11";

        }

    public HttpRequestMessage GetHttpRequest()
    {
        var content = new MultipartFormDataContent("----WebKitFormBoundaryJ7qgnb2wKJ7ZHGAA");

        foreach (var parameter in parameters)
        {
            var stringContent = new StringContent(parameter.Value);
            stringContent.Headers.Remove("Content-Type");
            content.Add(stringContent, parameter.Key);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "https://sheffield.libcal.com/ajax/space/book")
        {
            Content = content
        };

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return request;
        }



    }

    
}