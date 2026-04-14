using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniDesk.UnitTests.Fakes;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

namespace UniDesk.UnitTests.Services
{
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
				Status = TicketStatus.Open  // Устанавливаем статус "Open"
			};

			var ticketMapper = new TicketMapper();  // Используем наш маппер

			// Act
			var dto = ticketMapper.MapTicketToDto(ticket);  // Маппим Ticket в TicketReadDto

			// Assert
			Assert.Equal(ticket.Id, dto.Id);  // Проверка, что ID правильный
			Assert.Equal(ticket.Title, dto.Title);  // Проверка, что Title правильный
			Assert.Equal(ticket.Status.ToString(), dto.Status);  // Проверка, что статус преобразован в строку
		}
	}
}
