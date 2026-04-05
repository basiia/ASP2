using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Services;
using UniDesk.Web.Models;
using Microsoft.EntityFrameworkCore;
using UniDesk.Web.DTOs;

namespace UniDesk.Web.Controllers
{
	public class TicketsController : Controller
	{
		private readonly ITicketService _ticketService;

		public TicketsController(ITicketService ticketService)
		{
			_ticketService = ticketService;
		}

		[HttpPost]
		public IActionResult Create(Ticket ticket)
		{
			if (!ModelState.IsValid)
			{
				var result = _ticketService.GetAll(new TicketQueryParameters());
				return View("Index", result);
			}

			_ticketService.Add(ticket);
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