using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
	public interface ITicketService
	{
		List<Ticket> GetAll();
		void Add(Ticket ticket);
	}
}