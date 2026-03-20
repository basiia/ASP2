using UniDesk.Web.Models;
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

		public List<Ticket> GetAll()
		{
			return _context.Tickets.ToList();
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
			ticket.CreatedAt = DateTime.Now;

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
				existingTicket.UpdatedAt = DateTime.Now; 

				_context.SaveChanges(); 
			}
		}
	}
}