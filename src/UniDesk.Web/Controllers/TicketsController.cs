using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
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

        public IActionResult Index(TicketQueryParameters query)
        {
            var result = _ticketService.GetAll(query);
            return View(result);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                var result = _ticketService.GetAll(new TicketQueryParameters());
                return View("Index", result);
            }

            var request = new CreateTicketRequest
            {
                Title = ticket.Title,
                Description = ticket.Description
            };

            _ticketService.Create(request);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            _ticketService.Update(ticket);

            return RedirectToAction(nameof(Index));
        }
    }
}