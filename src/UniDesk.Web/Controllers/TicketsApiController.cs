using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

namespace UniDesk.Web.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Produces("application/json")]
    [Tags("Tickets")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsApiController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TicketListDto>), StatusCodes.Status200OK)]
        public IActionResult GetAll([FromQuery] TicketQueryParameters query)
        {
            var result = _ticketService.GetAll(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TicketReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TicketReadDto> GetTicketById(int id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var dto = new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
            };

            return Ok(dto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TicketReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<TicketReadDto> CreateTicket([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var dto = _ticketService.Create(request);

            return CreatedAtAction(nameof(GetTicketById), new { id = dto.Id }, dto);
        }

        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateTicketStatusRequest request)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = (TicketStatus)request.Status;
            _ticketService.Update(ticket);

            return NoContent();
        }
    }
}