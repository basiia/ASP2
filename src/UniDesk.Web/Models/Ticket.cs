using System.ComponentModel.DataAnnotations;

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
		public int Id { get; set; }

		[Required(ErrorMessage = "Tytuł jest wymagany")]
		[StringLength(100)]
		public required string Title { get; set; }

		[Required(ErrorMessage = "Opis jest wymagany")]
		[StringLength(500)]
		public required string Description { get; set; }

		public TicketStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}