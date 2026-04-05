using UniDesk.Web.Models;
using UniDesk.Web.DTOs;
using System.Collections.Generic;

namespace UniDesk.Web.Services
{
	public interface ITicketService
	{
		PagedResult<TicketListDto> GetAll(TicketQueryParameters query);
		void Add(Ticket ticket);
		Ticket? GetById(int id);
		List<Ticket> Search(string search);
		void Update(Ticket ticket);
	}
}