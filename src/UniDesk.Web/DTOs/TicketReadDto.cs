namespace UniDesk.Web.DTOs
{
	public class TicketReadDto
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string Status { get; set; }  // Статус теперь строка, не nullable

		// Конструктор по умолчанию
		public TicketReadDto()
		{
			Status = string.Empty;  // Инициализируем Status по умолчанию
		}

		// Конструктор с параметрами
		public TicketReadDto(string title, string status)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title), "Title cannot be null");
			Status = status ?? throw new ArgumentNullException(nameof(status), "Status cannot be null");
		}
	}
}