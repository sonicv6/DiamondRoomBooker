namespace LibCalBooker.LibCal.Requests
{
	public struct GetRoomsRequest
	{
		private DateTime startDate;
		private DateTime endDate;
		private Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{ "lid", "2579" },
				{ "gid", "5160" },
				{ "eid", "-1" },
				{ "seat", "0" },
				{ "seatId", "0" },
				{ "zone", "0" },
				{ "pageIndex", "0" },
				{ "pageSize", "100" }
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
				{ "Referer", "https://sheffield.libcal.com/spaces?lid=2579&gid=5160" },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" },
				{ "Priority", "u=1, i" }
			};

		public GetRoomsRequest(DateTime startDate, DateTime endDate)
		{
			this.startDate = startDate;
			this.endDate = endDate;
			this.parameters["start"] = startDate.ToString("yyyy-MM-dd");
			this.parameters["end"] = endDate.ToString("yyyy-MM-dd");
		}

		public HttpRequestMessage GetHttpRequest()
		{
			var content = new FormUrlEncodedContent(parameters);
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded")
			{
				CharSet = "UTF-8"
			};
			var request = new HttpRequestMessage(HttpMethod.Post, "https://sheffield.libcal.com/spaces/availability/grid")
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
