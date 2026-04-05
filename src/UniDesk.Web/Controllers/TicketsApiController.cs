using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.DTOs;
using UniDesk.Web.Services;
using UniDesk.Web.Models;

namespace UniDesk.Web.Controllers
{
	[ApiController]
	[Route("api/tickets")]
	public class TicketsApiController : ControllerBase
	{
		private readonly ITicketService _ticketService;

		public TicketsApiController(ITicketService ticketService)
		{
			_ticketService = ticketService;
		}

		[HttpGet]
		public IActionResult GetAll([FromQuery] TicketQueryParameters query)
		{
			var result = _ticketService.GetAll(query);
			return Ok(result);
		}

		[HttpGet("{id}")]
		public ActionResult<TicketReadDto> GetTicketById(int id)
		{
			var ticket = _ticketService.GetById(id);

			if (ticket == null)
				return NotFound();

			var dto = new TicketReadDto(ticket.Title, ticket.Status);
			return Ok(dto);
		}

		[HttpPost]
		public ActionResult<TicketReadDto> CreateTicket(CreateTicketRequest request)
		{
			var ticket = new Ticket
			{
				Title = request.Title,
				Description = request.Description,
				Status = TicketStatus.Open
			};

			_ticketService.Add(ticket);

			var dto = new TicketReadDto(ticket.Title, ticket.Status);

			return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, dto);
		}

		[HttpPatch("{id}/status")]
		public IActionResult UpdateStatus(int id, UpdateTicketStatusRequest request)
		{
			var ticket = _ticketService.GetById(id);

			if (ticket == null)
				return NotFound();

			ticket.Status = (TicketStatus)request.Status;
			_ticketService.Update(ticket);

			return NoContent();
		}
	}
}