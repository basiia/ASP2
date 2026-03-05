using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Services;

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
	}
}