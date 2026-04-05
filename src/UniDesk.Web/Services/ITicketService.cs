using UniDesk.Web.Models;
using UniDesk.Web.DTOs;
using System.Collections.Generic;

namespace UniDesk.Web.Services
{
	public interface ITicketService
	{
		List<TicketListDto> GetAll(string? status, int page, int pageSize, bool desc);
		void Add(Ticket ticket);
		Ticket? GetById(int id);
		List<Ticket> Search(string search);
		void Update(Ticket ticket);
	}
}