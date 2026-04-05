using UniDesk.Web.Models;
using UniDesk.Web.DTOs;
using System.Linq;

namespace UniDesk.Web.Services
{
	public class DbTicketService : ITicketService
	{
		private readonly UniDeskDbContext _context;

		public DbTicketService(UniDeskDbContext context)
		{
			_context = context;
		}

		public PagedResult<TicketListDto> GetAll(TicketQueryParameters queryParams)
		{
			IQueryable<Ticket> query = _context.Tickets.AsQueryable();

			// FILTER
			if (!string.IsNullOrEmpty(queryParams.Status))
			{
				if (Enum.TryParse<TicketStatus>(queryParams.Status, out var parsedStatus))
				{
					query = query.Where(t => t.Status == parsedStatus);
				}
			}

			// TOTAL COUNT (ВАЖНО ДО PAGING)
			int totalCount = query.Count();

			// SORT
			query = queryParams.Desc
				? query.OrderByDescending(t => t.CreatedAt)
				: query.OrderBy(t => t.CreatedAt);

			// PAGING
			query = query
				.Skip((queryParams.Page - 1) * queryParams.PageSize)
				.Take(queryParams.PageSize);

			// DTO
			var items = query
				.Select(t => new TicketListDto
				{
					Id = t.Id,
					Title = t.Title,
					Status = t.Status.ToString()
				})
				.ToList();

			return new PagedResult<TicketListDto>
			{
				Items = items,
				TotalCount = totalCount
			};
		}

		public Ticket? GetById(int id)
		{
			return _context.Tickets.FirstOrDefault(t => t.Id == id);
		}

		public List<Ticket> Search(string search)
		{
			return _context.Tickets
				.Where(t => t.Title.Contains(search))
				.ToList();
		}

		public void Add(Ticket ticket)
		{
			ticket.CreatedAt = DateTime.UtcNow;

			if (ticket.Status == 0)
			{
				ticket.Status = TicketStatus.Open;
			}

			_context.Tickets.Add(ticket);
			_context.SaveChanges();
		}

		public void Update(Ticket ticket)
		{
			var existing = _context.Tickets.Find(ticket.Id);

			if (existing != null)
			{
				existing.Title = ticket.Title;
				existing.Description = ticket.Description;
				existing.Status = ticket.Status;
				existing.UpdatedAt = DateTime.UtcNow;

				_context.SaveChanges();
			}
		}
	}
}