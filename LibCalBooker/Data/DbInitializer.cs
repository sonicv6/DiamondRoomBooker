namespace LibCalBooker.Data
{
	public class DbInitializer
	{
		public static void Initialize(LibCalContext context)
		{
			context.Database.EnsureCreated();
		}
	}
}
