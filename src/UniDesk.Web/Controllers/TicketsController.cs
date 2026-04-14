using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Services;
using UniDesk.Web.Models;
using UniDesk.Web.DTOs;
using Microsoft.EntityFrameworkCore;

namespace UniDesk.Web.Controllers
{
	public class TicketsController : Controller
	{
		private readonly ITicketService _ticketService;
		private readonly ISystemClock _systemClock;  // Добавляем зависимость от ISystemClock

		// Внедряем зависимость ISystemClock через конструктор
		public TicketsController(ITicketService ticketService, ISystemClock systemClock)
		{
			_ticketService = ticketService;
			_systemClock = systemClock;  // Инициализируем ISystemClock
		}

		[HttpPost]
		public IActionResult Create(Ticket ticket)
		{
			if (!ModelState.IsValid)
			{
				var result = _ticketService.GetAll(new TicketQueryParameters());
				return View("Index", result);
			}

			// Передаем _systemClock при создании нового тикета
			var newTicket = new Ticket(_systemClock)
			{
				Title = ticket.Title,
				Description = ticket.Description,
				Status = ticket.Status  // Статус остается как TicketStatus
			};

			_ticketService.Add(newTicket);
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Index(TicketQueryParameters query)
		{
			var result = _ticketService.GetAll(query);
			return View(result);
		}

		public IActionResult Details(int id)
		{
			var ticket = _ticketService.GetById(id);

			if (ticket == null)
				return NotFound();

			return View(ticket);
		}

		public IActionResult Edit(int id)
		{
			var ticket = _ticketService.GetById(id);

			if (ticket == null)
				return NotFound();

			return View(ticket);
		}

		[HttpPost]
		public IActionResult Edit(int id, Ticket ticket)
		{
			if (id != ticket.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				_ticketService.Update(ticket);
				return RedirectToAction(nameof(Index));
			}

			return View(ticket);
		}
	}
}