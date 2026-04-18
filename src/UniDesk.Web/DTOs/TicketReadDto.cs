namespace UniDesk.Web.DTOs
{
	public class TicketReadDto
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string Status { get; set; } 

		
		public TicketReadDto()
		{
			Status = string.Empty;  
		}

		public TicketReadDto(string title, string status)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title), "Title cannot be null");
			Status = status ?? throw new ArgumentNullException(nameof(status), "Status cannot be null");
		}
	}
}