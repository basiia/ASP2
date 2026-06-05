using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniDesk.Web.DTOs;
using UniDesk.Web.Services;
using UniDesk.Web.ViewModels;

namespace UniDesk.Web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketCommentService _ticketCommentService;

        public TicketsController(
            ITicketService ticketService,
            ITicketCommentService ticketCommentService)
        {
            _ticketService = ticketService;
            _ticketCommentService = ticketCommentService;
        }

        public IActionResult Index(TicketQueryParameters query)
        {
            var result = _ticketService.GetAll(query);
            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = await BuildDetailsModelAsync(ticket.Id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(
            int ticketId,
            [Bind(Prefix = "NewComment")] CreateTicketCommentRequest request)
        {
            var ticket = _ticketService.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var model = await BuildDetailsModelAsync(ticketId);
                model.NewComment = request;

                return View(nameof(Details), model);
            }

            var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (authorId == null)
            {
                return Challenge();
            }

            var authorName = User.Identity?.Name ?? "Uzytkownik";

            await _ticketCommentService.CreateAsync(ticketId, authorId, authorName, request);

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                var result = _ticketService.GetAll(new TicketQueryParameters());
                return View("Index", result);
            }

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

            var model = new TicketEditViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TicketEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new UpdateTicketRequest
            {
                Title = model.Title,
                Description = model.Description,
                Status = (int)model.Status
            };

            _ticketService.Update(id, request);

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<TicketDetailsViewModel> BuildDetailsModelAsync(int ticketId)
        {
            var ticket = _ticketService.GetById(ticketId);

            if (ticket == null)
            {
                throw new InvalidOperationException("Ticket not found.");
            }

            return new TicketDetailsViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt,
                Comments = await _ticketCommentService.GetForTicketAsync(ticketId)
            };
        }
    }
}
