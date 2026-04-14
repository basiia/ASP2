using UniDesk.Web.Models;
using UniDesk.Web.DTOs;

namespace UniDesk.Web.Services
{
	public class TicketMapper
	{
		// Метод для маппинга из Ticket в TicketReadDto
		public TicketReadDto MapTicketToDto(Ticket ticket)
		{
			return new TicketReadDto
			{
				Id = ticket.Id,
				Title = ticket.Title,
				Status = ticket.Status.ToString()  // Преобразуем TicketStatus в строку
			};
		}
	}
}