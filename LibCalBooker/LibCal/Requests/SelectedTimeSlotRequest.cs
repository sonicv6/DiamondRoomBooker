namespace LibCalBooker.LibCal.Requests
{
    public struct SelectedTimeSlotRequest
    {
        private int _eid;
        private DateTime start;
        private DateTime end;
        private string checksum;


        private Dictionary<string, string> parameters = new Dictionary<string, string>
        {
				{ "add[gid]", "5160" },
				{ "add[lid]", "2579" },

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

        public SelectedTimeSlotRequest(int eid, DateTime start, DateTime end, string checksum)
        {
            this._eid = eid;
            this.start = start;
            this.end = end;
            this.checksum = checksum;

            this.parameters["add[eid]"] = eid.ToString();
            this.parameters["add[gid]"] = "5160";
            this.parameters["add[lid]"] = "2579";
            this.parameters["add[start"] = start.ToString("yyyy-MM-dd HH:mm:ss");
            this.parameters["add[checksum]"] = checksum;
            this.parameters["lid"] = "2579";
            this.parameters["gid"] = "0";
            this.parameters["start"] = start.ToString("yyyy-MM-dd");
            this.parameters["end"] = end.ToString("yyyy-MM-dd");


        }
        
        public HttpRequestMessage GetHttpRequest()
		{

            
			var content = new FormUrlEncodedContent(parameters);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded")
			{
				CharSet = "UTF-8"
			};
			var request = new HttpRequestMessage(HttpMethod.Post, "https://sheffield.libcal.com/spaces/availability/booking/add");
            request.Content = content;

			// foreach (var header in headers)
			// {
			// 	request.Headers.Add(header.Key, header.Value);
			// }

			return request;
		}






    }
    
    
    
    
}