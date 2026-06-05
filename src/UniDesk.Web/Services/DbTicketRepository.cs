using Microsoft.EntityFrameworkCore;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
	public class DbTicketRepository : ITicketRepository
	{
		private readonly UniDeskDbContext _context;

		public DbTicketRepository(UniDeskDbContext context)
		{
			_context = context;
		}

		public Ticket? GetById(int id)
		{
			return _context.Tickets.FirstOrDefault(t => t.Id == id);
		}

		public void Add(Ticket ticket)
		{
			_context.Tickets.Add(ticket);
			_context.SaveChanges();
		}

		public void Update(Ticket ticket)
		{
			_context.Tickets.Update(ticket);
			_context.SaveChanges();
		}

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public IQueryable<Ticket> GetAll(TicketQueryParameters queryParams)
		{
			return _context.Tickets;
		}

		public List<Ticket> Search(string search)
		{
			return _context.Tickets.Where(t => t.Title.Contains(search)).ToList();
		}

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
        }
    }
}
