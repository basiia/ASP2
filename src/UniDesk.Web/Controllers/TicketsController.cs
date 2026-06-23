using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using UniDesk.Web.ViewModels;

namespace UniDesk.Web.Controllers;
[Authorize]
public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;
    private readonly ITicketCommentService _ticketCommentService;
    private readonly IAuthorizationService _authorizationService;

    public TicketsController(
        ITicketService ticketService,
        ITicketCommentService ticketCommentService,
        IAuthorizationService authorizationService)
    {
        _ticketService = ticketService;
        _ticketCommentService = ticketCommentService;
        _authorizationService = authorizationService;
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

        var canUseDiscussion = await CanUseDiscussionAsync(ticket);
        var model = await BuildDetailsModelAsync(ticket, canUseDiscussion);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("comments")]
    public async Task<IActionResult> AddComment(
        int ticketId,
        [Bind(Prefix = "NewComment")] CreateTicketCommentRequest request)
    {
        var ticket = _ticketService.GetById(ticketId);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanUseDiscussionAsync(ticket))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            var model = await BuildDetailsModelAsync(ticket, canUseDiscussion: true);
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
            ViewData["CreateTitle"] = request.Title;
            ViewData["CreateDescription"] = request.Description;

            var result = _ticketService.GetAll(new TicketQueryParameters());
            return View("Index", result);
        }

        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (ownerId == null)
        {
            return Challenge();
        }

        var ownerName = User.Identity?.Name ?? "Uzytkownik";
        _ticketService.Create(request, ownerId, ownerName);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ticket = _ticketService.GetById(id);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanUseDiscussionAsync(ticket))
        {
            return Forbid();
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
    public async Task<IActionResult> Edit(int id, TicketEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var ticket = _ticketService.GetById(id);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanUseDiscussionAsync(ticket))
        {
            return Forbid();
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

    private async Task<bool> CanUseDiscussionAsync(Ticket ticket)
    {
        var result = await _authorizationService.AuthorizeAsync(
            User,
            ticket,
            "CanAccessTicket");

        return result.Succeeded;
    }

    private async Task<TicketDetailsViewModel> BuildDetailsModelAsync(
        Ticket ticket,
        bool canUseDiscussion)
    {
        var comments = canUseDiscussion
            ? await _ticketCommentService.GetForTicketAsync(ticket.Id)
            : new List<TicketCommentDto>();

        return new TicketDetailsViewModel
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            CreatedAt = ticket.CreatedAt,
            OwnerName = ticket.OwnerName,
            CanUseDiscussion = canUseDiscussion,
            Comments = comments
        };
    }
}

