using Microsoft.EntityFrameworkCore;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
using UniDesk.Web.Exceptions;

namespace UniDesk.Web.Services
{
	public class DbTicketService : ITicketService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly ISystemClock _systemClock; 

		public DbTicketService(ITicketRepository ticketRepository, ISystemClock systemClock)
		{
			_ticketRepository = ticketRepository;
			_systemClock = systemClock;
		}

		public PagedResult<TicketListDto> GetAll(TicketQueryParameters queryParams)
		{
			IQueryable<Ticket> query = _ticketRepository.GetAll(queryParams);

			if (!string.IsNullOrEmpty(queryParams.Status))
			{
				if (Enum.TryParse<TicketStatus>(queryParams.Status, out var parsedStatus))
				{
					query = query.Where(t => t.Status == parsedStatus);
				}
			}

			int totalCount = query.Count();

			var allowedSorts = new List<string>
			{
				"Title",
				"Status",
				"CreatedAt"
			};

			if (!string.IsNullOrEmpty(queryParams.SortBy) && allowedSorts.Contains(queryParams.SortBy))
			{
				query = queryParams.Desc
					? query.OrderByDescending(x => EF.Property<object>(x, queryParams.SortBy))
					: query.OrderBy(x => EF.Property<object>(x, queryParams.SortBy));
			}
			else
			{
				query = query.OrderBy(x => x.Id);
			}

			query = query
				.Skip((queryParams.Page - 1) * queryParams.PageSize)
				.Take(queryParams.PageSize);

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
			return _ticketRepository.GetById(id);
		}

		public List<Ticket> Search(string search)
		{
			return _ticketRepository.Search(search);
		}

		public void Add(Ticket ticket)
		{
			ticket.CreatedAt = _systemClock.UtcNow;  

			if (ticket.Status == 0)
			{
				ticket.Status = TicketStatus.Open;
			}

			_ticketRepository.Add(ticket);
		}

		public void Update(Ticket ticket)
		{
			_ticketRepository.Update(ticket);
		}

		public void UpdateStatus(int ticketId, TicketStatus status)
		{
			var ticket = _ticketRepository.GetById(ticketId);

			if (ticket == null)
			{
				throw new InvalidOperationException("Nie znaleziono biletu.");
			}

			if (ticket.Status == TicketStatus.Closed)
			{
				throw new InvalidOperationException("Nie można zmienić statusu zamkniętego zgłoszenia.");
			}

			ticket.Status = status;
			_ticketRepository.Update(ticket);
		}

        public TicketReadDto Create(CreateTicketRequest request)
        {
            var ticket = new Ticket(_systemClock)
            {
                Title = request.Title,
                Description = request.Description,
                Status = TicketStatus.Open
            };

            Add(ticket);

            return new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };
        }

        public bool Delete(int id)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Nie znaleziono zgłoszenia o id {id}.");
            }

            _ticketRepository.Delete(ticket);
            return true;
        }

        public TicketReadDto Update(int id, UpdateTicketRequest request)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Nie znaleziono zgłoszenia o id {id}.");
            }

            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.Status = (TicketStatus)request.Status;

            _ticketRepository.Update(ticket);

            return new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };
        }
    }
}