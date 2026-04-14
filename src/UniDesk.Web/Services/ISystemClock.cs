namespace UniDesk.Web.Services
{
	public interface ISystemClock
	{
		DateTime UtcNow { get; }
	}
}
