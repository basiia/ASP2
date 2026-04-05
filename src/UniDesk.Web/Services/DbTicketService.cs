using UniDesk.Web.Models;
using UniDesk.Web.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

		// 🔥 НОВЫЙ МЕТОД (LAB 5 CORE)
		public List<TicketListDto> GetAll(string? status, int page, int pageSize, bool desc)
		{
			IQueryable<Ticket> query = _context.Tickets.AsQueryable();

			// 🔹 ФИЛЬТР
			if (!string.IsNullOrEmpty(status))
			{
				if (Enum.TryParse<TicketStatus>(status, out var parsedStatus))
				{
					query = query.Where(t => t.Status == parsedStatus);
				}
			}

			// 🔹 СОРТИРОВКА
			if (desc)
			{
				query = query.OrderByDescending(t => t.CreatedAt);
			}
			else
			{
				query = query.OrderBy(t => t.CreatedAt);
			}

			// 🔹 PAGING
			query = query
				.Skip((page - 1) * pageSize)
				.Take(pageSize);

			// 🔹 DTO
			return query
				.Select(t => new TicketListDto
				{
					Id = t.Id,
					Title = t.Title,
					Status = t.Status.ToString()
				})
				.ToList();
		}

		public Ticket? GetById(int id)
		{
			return _context.Tickets.FirstOrDefault(t => t.Id == id);
		}

		public List<Ticket> Search(string search)
		{
			return _context.Tickets
				.Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
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
			var existingTicket = _context.Tickets.Find(ticket.Id);

			if (existingTicket != null)
			{
				existingTicket.Title = ticket.Title;
				existingTicket.Description = ticket.Description;
				existingTicket.Status = ticket.Status;
				existingTicket.UpdatedAt = DateTime.UtcNow;

				_context.SaveChanges();
			}
		}
	}
}