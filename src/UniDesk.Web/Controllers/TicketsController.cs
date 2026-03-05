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

		public IActionResult Index()
		{
			var tickets = _ticketService.GetAll();
			return View(tickets);
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
	}
}