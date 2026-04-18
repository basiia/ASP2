namespace UniDesk.Web.Services
{
	public class SystemClock : ISystemClock
	{
		public DateTime UtcNow => DateTime.UtcNow;
	}
}