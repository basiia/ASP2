using UniDesk.Web.Models;

namespace UniDesk.Web.DTOs
{
	public class TicketReadDto
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public TicketStatus Status { get; set; }

		public TicketReadDto(string title, TicketStatus status)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title), "Title cannot be null");
			Status = status;
		}
	}
}