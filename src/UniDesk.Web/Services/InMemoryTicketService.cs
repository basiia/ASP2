using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
	public class InMemoryTicketService : ITicketService
	{
		private static List<Ticket> _tickets = new List<Ticket>();
		private static int _nextId = 1;

		public List<Ticket> GetAll()
		{
			return _tickets;
		}

		public void Add(Ticket ticket)
		{
			ticket.Id = _nextId++;
			ticket.CreatedAt = DateTime.Now;

			if (ticket.Status == 0)
			{
				ticket.Status = TicketStatus.Open;
			}

			_tickets.Add(ticket);
		}
	}
}