namespace LibCalBooker.LibCal
{
	public struct TimeSlot
	{
		public int roomId;
		public DateTime startTime;
		public DateTime endTime;
		public string checksum;
		public string roomName;
		public string diamondName;

		public TimeSlot(int roomId, DateTime startTime, DateTime endTime, string checksum, string roomName)
		{
			this.roomId = roomId;
			this.startTime = startTime;
			this.endTime = endTime;
			this.checksum = checksum;
			this.roomName = roomName;
		}
	}
}
