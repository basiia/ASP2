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
			IQueryable<Ticket> query = _context.Tickets;  

			if (!string.IsNullOrEmpty(queryParams.Status))
			{
				if (Enum.TryParse<TicketStatus>(queryParams.Status, out var parsedStatus))
				{
					query = query.Where(t => t.Status == parsedStatus);
				}
			}

			var allowedSorts = new List<string> { "Title", "Status", "CreatedAt" };
			if (!string.IsNullOrEmpty(queryParams.SortBy) && allowedSorts.Contains(queryParams.SortBy))
			{
				query = queryParams.Desc
					? query.OrderByDescending(x => EF.Property<object>(x, queryParams.SortBy))
					: query.OrderBy(x => EF.Property<object>(x, queryParams.SortBy));
			}

			query = query.Skip((queryParams.Page - 1) * queryParams.PageSize)
						 .Take(queryParams.PageSize);

			return query; 
		}

		public List<Ticket> Search(string search)
		{
			return _context.Tickets.Where(t => t.Title.Contains(search)).ToList();
		}
	}
}