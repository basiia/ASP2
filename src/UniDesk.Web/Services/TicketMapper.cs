using UniDesk.Web.Models;
using UniDesk.Web.DTOs;

namespace UniDesk.Web.Services
{
	public class TicketMapper
	{
	
		public TicketReadDto MapTicketToDto(Ticket ticket)
		{
			return new TicketReadDto
			{
				Id = ticket.Id,
				Title = ticket.Title,
				Status = ticket.Status.ToString()  
			};
		}
	}
}