using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Services;
using UniDesk.Web.Models;

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
				return View("Index", _ticketService.GetAll());
			}

			_ticketService.Add(ticket);

			return RedirectToAction(nameof(Index));
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