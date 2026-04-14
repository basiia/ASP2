using Xunit;
using Moq;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using UniDesk.Web.DTOs;

namespace UniDesk.UnitTests.Services
{
	public class DbTicketServiceTests
	{
		private readonly Mock<ITicketRepository> _mockRepo;
		private readonly DbTicketService _service;

		public DbTicketServiceTests()
		{
			_mockRepo = new Mock<ITicketRepository>();
			_service = new DbTicketService(_mockRepo.Object);
		}

		[Fact]
		public void UpdateStatus_ShouldChangeStatus_WhenValidStatusIsProvided()
		{
			var ticket = new Ticket
			{
				Id = 1,
				Title = "Sample Title",
				Description = "Sample Description",
				Status = TicketStatus.Open
			};

			_mockRepo.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(ticket);

			_service.UpdateStatus(ticket.Id, TicketStatus.InProgress);

			Assert.Equal(TicketStatus.InProgress, ticket.Status);
			_mockRepo.Verify(repo => repo.Update(ticket), Times.Once);
		}

		[Fact]
		public void Add_ShouldAddTicket_WhenValidTicket()
		{
			var ticket = new Ticket
			{
				Title = "New Ticket",
				Description = "Sample Description",
				Status = TicketStatus.Open
			};

			_service.Add(ticket);

			_mockRepo.Verify(m => m.Add(It.IsAny<Ticket>()), Times.Once);
		}

		[Fact]
		public void GetAll_ShouldReturnPagedResults_WhenPageSizeIsSet()
		{
			// Arrange: Define query parameters for pagination
			var queryParams = new TicketQueryParameters
			{
				Page = 1,   // First page
				PageSize = 10, // Page size of 10
			};

			// Create a list of tickets
			var tickets = new List<Ticket>
	{
		new Ticket { Id = 1, Title = "Ticket 1" },
		new Ticket { Id = 2, Title = "Ticket 2" },
		new Ticket { Id = 3, Title = "Ticket 3" },
		new Ticket { Id = 4, Title = "Ticket 4" },
		new Ticket { Id = 5, Title = "Ticket 5" },
		new Ticket { Id = 6, Title = "Ticket 6" },
		new Ticket { Id = 7, Title = "Ticket 7" },
		new Ticket { Id = 8, Title = "Ticket 8" },
		new Ticket { Id = 9, Title = "Ticket 9" },
		new Ticket { Id = 10, Title = "Ticket 10" },
		new Ticket { Id = 11, Title = "Ticket 11" },
		new Ticket { Id = 12, Title = "Ticket 12" },
		new Ticket { Id = 13, Title = "Ticket 13" },
		new Ticket { Id = 14, Title = "Ticket 14" },
		new Ticket { Id = 15, Title = "Ticket 15" }
	};

			// Setup mock repository to return tickets
			_mockRepo.Setup(repo => repo.GetAll(queryParams)).Returns(tickets.AsQueryable());

			// Act: Get paged result
			var result = _service.GetAll(queryParams);

			// Assert: Check that the result has 10 items
			Assert.Equal(10, result.Items.Count);
			// Additionally, you can check that the items returned correspond to the expected ones.
			Assert.Equal("Ticket 1", result.Items[0].Title);
			Assert.Equal("Ticket 10", result.Items[9].Title);
		}
	}
}