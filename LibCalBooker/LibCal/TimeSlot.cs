namespace LibCalBooker.LibCal
{
	public class TimeSlot
	{
		public int roomId {get; set;} 
		public DateTime startTime {get; set;} 
		public DateTime endTime {get; set;} 
		public string checksum {get; set;} 
		public string className {get; set;} 
		public string diamondName {get; set;} 
		
		
		public TimeSlot()
        {

        }
		

		public TimeSlot(int roomId, DateTime startTime, DateTime endTime, string checksum, string className = "")
		{
			this.roomId = roomId;
			this.startTime = startTime;
			this.endTime = endTime;
			this.checksum = checksum;
			this.className = className;
		}
	}
}
