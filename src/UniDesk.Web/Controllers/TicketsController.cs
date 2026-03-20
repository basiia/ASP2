using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Services;
using UniDesk.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace UniDesk.Web.Controllers
{
	public class TicketsController : Controller
	{
		private readonly ITicketService _ticketService;

		public TicketsController(ITicketService ticketService)
		{
			_ticketService = ticketService;
		}

		// POST: /Tickets/Create
		[HttpPost]
		public IActionResult Create(Ticket ticket)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View("Index", _ticketService.GetAll());
				}

				_ticketService.Add(ticket);

				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException ex)
			{
				return BadRequest($"Ошибка сохранения данных: {ex.Message}");
			}
		}

		// GET: /Tickets/Edit/5
		public IActionResult Edit(int id)
		{
			var ticket = _ticketService.GetById(id); 

			if (ticket == null)
			{
				return NotFound(); 
			}

			return View(ticket); 
		}

		// POST: /Tickets/Edit/5
		[HttpPost]
		public IActionResult Edit(int id, Ticket ticket)
		{
			if (id != ticket.Id)
			{
				return NotFound(); 
			}

			if (ModelState.IsValid)
			{
				try
				{
					_ticketService.Update(ticket); 
				}
				catch (DbUpdateConcurrencyException)
				{
					return BadRequest("Ошибка при обновлении тикета.");
				}

				return RedirectToAction(nameof(Index)); 
			}

			return View(ticket);
		}

		public IActionResult Details(int id)
		{
			var ticket = _ticketService.GetById(id);

			if (ticket == null)
			{
				return NotFound();
			}

			return View(ticket);
		}

		public IActionResult Index(string? search)
		{
			var tickets = string.IsNullOrEmpty(search)
				? _ticketService.GetAll()
				: _ticketService.Search(search);

			return View(tickets);
		}
	}
}