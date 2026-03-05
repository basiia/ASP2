using UniDesk.Web.Models;
using System.Linq;

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

		public Ticket? GetById(int id)
		{
			return _tickets.FirstOrDefault(t => t.Id == id);
		}

		public List<Ticket> Search(string search)
		{
			return _tickets
				.Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
				.ToList();
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