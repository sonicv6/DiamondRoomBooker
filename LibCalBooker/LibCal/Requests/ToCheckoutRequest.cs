namespace LibCalBooker.LibCal.Requests
{
    public struct ToCheckoutRequest
    {
        private int _id;
        private int _eid;
        private int _seat_id;
        private int _gid;
        private int _lid;
        private DateTime _start;
        private DateTime _end;
        private string _checksum;


        private Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{ "patron", "" },
			{ "patronHash", "" },

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

        public ToCheckoutRequest(int id, int eid, int seat_id, int gid, int lid, DateTime start, DateTime end, string checksum)
        {
            this._id = id;
            this._eid = eid;
            this._seat_id = seat_id;
            this._gid = gid;
            this._lid = lid;
            this._start = start;
            this._end = end;
            this._checksum = checksum;

            this.parameters["bookings[0][id]"] = id.ToString();
            this.parameters["bookings[0][eid]"] = eid.ToString();
            this.parameters["bookings[0][seat_id]"] = seat_id.ToString();
            this.parameters["bookings[0][gid]"] = gid.ToString();
            this.parameters["bookings[0][lid]"] = lid.ToString();
            this.parameters["bookings[0][start]"] = start.ToString();
            this.parameters["bookings[0][end]"] = end.ToString();
            this.parameters["bookings[0][checksum]"] = checksum;

        }
        public HttpRequestMessage GetHttpRequest()
		{
			var content = new FormUrlEncodedContent(parameters);
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded")
			{
				CharSet = "UTF-8"
			};
			var request = new HttpRequestMessage(HttpMethod.Post, "https://sheffield.libcal.com/ajax/space/times")
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
