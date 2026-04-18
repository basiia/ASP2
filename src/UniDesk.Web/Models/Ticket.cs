using System.ComponentModel.DataAnnotations;
using UniDesk.Web.Services;

namespace UniDesk.Web.Models
{
	public enum TicketStatus
	{
		Open,
		InProgress,
		Closed
	}

	public class Ticket
	{
		private readonly ISystemClock? _systemClock;

		public Ticket()
		{
			CreatedAt = DateTime.UtcNow;
		}

		public Ticket(ISystemClock systemClock)
		{
			_systemClock = systemClock;
			Title = string.Empty;
			Description = string.Empty;
			CreatedAt = _systemClock?.UtcNow ?? DateTime.UtcNow;
		}

		public int Id { get; set; }

		[Required(ErrorMessage = "Tytuł jest wymagany")]
		[StringLength(100)]
		public string Title { get; set; } = string.Empty;

		[Required(ErrorMessage = "Opis jest wymagany")]
		[StringLength(500)]
		public string Description { get; set; } = string.Empty;

		public TicketStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}