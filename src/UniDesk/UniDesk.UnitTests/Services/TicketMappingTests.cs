using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniDesk.UnitTests.Fakes;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

namespace UniDesk.UnitTests.Services;
public class TicketMappingTests
{
	[Fact]
	public void Ticket_ShouldMapToTicketReadDto_Correctly()
	{
		// Arrange
		var ticket = new Ticket(new FakeClock())
		{
			Id = 1,
			Title = "Sample Title",
			Status = TicketStatus.New
		};

		var ticketMapper = new TicketMapper();

		// Act
		var dto = ticketMapper.MapTicketToDto(ticket);

		// Assert
		Assert.Equal(ticket.Id, dto.Id);
		Assert.Equal(ticket.Title, dto.Title);
		Assert.Equal(ticket.Status.ToString(), dto.Status);
	}
}

