using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniDesk.Web.Services;  // Для ISystemClock


namespace UniDesk.UnitTests.Fakes
{
	public class FakeClock : ISystemClock
	{
		public DateTime UtcNow { get; } = new DateTime(2026, 04, 08, 12, 00, 00, DateTimeKind.Utc);
	}
}
