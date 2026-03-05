using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
	public class InMemoryTicketService : ITicketService
	{
		private readonly List<Ticket> _tickets = new();

		public List<Ticket> GetAll()
		{
			return _tickets;
		}

		public void Add(Ticket ticket)
		{
			ticket.Id = _tickets.Count + 1;
			ticket.CreatedAt = DateTime.Now;
			_tickets.Add(ticket);
		}
	}
}