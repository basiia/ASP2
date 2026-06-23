using Xunit;
using Moq;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using UniDesk.Web.DTOs;
using UniDesk.UnitTests.Fakes;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;

namespace UniDesk.UnitTests.Services;
public class TicketServiceTests
{
	private readonly Mock<ITicketRepository> _mockRepo;
	private readonly TicketService _service;
	private readonly ISystemClock _fakeClock;

    public TicketServiceTests()
    {
        _mockRepo = new Mock<ITicketRepository>();
        _fakeClock = new FakeClock();

        _service = new TicketService(
            _mockRepo.Object,
            _fakeClock,
            NullLogger<TicketService>.Instance);
    }

    [Fact]
	public void UpdateStatus_ShouldChangeStatus_WhenValidStatusIsProvided()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Title = "Sample Title",
			Description = "Sample Description"
		};

		_mockRepo.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(ticket);

		_service.UpdateStatus(ticket.Id, TicketStatus.InProgress);

		Assert.Equal(TicketStatus.InProgress, ticket.Status);
		_mockRepo.Verify(repo => repo.Update(ticket), Times.Once);  
	}

	[Fact]
	public void Add_ShouldAddTicket_WhenValidTicket()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Title = "New Ticket",
			Description = "Sample Description",
			Status = TicketStatus.New
		};

		_service.Add(ticket);

		_mockRepo.Verify(m => m.Add(It.IsAny<Ticket>()), Times.Once);
	}

	[Fact]
	public void GetAll_ShouldReturnPagedResults_WhenPageSizeIsSet()
	{
		var queryParams = new TicketQueryParameters
		{
			Page = 1,
			PageSize = 10,
		};

		var tickets = new List<Ticket>
		{
			new Ticket(_fakeClock) { Id = 1, Title = "Ticket 1" },
			new Ticket(_fakeClock) { Id = 2, Title = "Ticket 2" },
			new Ticket(_fakeClock) { Id = 3, Title = "Ticket 3" },
			new Ticket(_fakeClock) { Id = 4, Title = "Ticket 4" },
			new Ticket(_fakeClock) { Id = 5, Title = "Ticket 5" },
			new Ticket(_fakeClock) { Id = 6, Title = "Ticket 6" },
			new Ticket(_fakeClock) { Id = 7, Title = "Ticket 7" },
			new Ticket(_fakeClock) { Id = 8, Title = "Ticket 8" },
			new Ticket(_fakeClock) { Id = 9, Title = "Ticket 9" },
			new Ticket(_fakeClock) { Id = 10, Title = "Ticket 10" },
			new Ticket(_fakeClock) { Id = 11, Title = "Ticket 11" },
			new Ticket(_fakeClock) { Id = 12, Title = "Ticket 12" },
			new Ticket(_fakeClock) { Id = 13, Title = "Ticket 13" },
			new Ticket(_fakeClock) { Id = 14, Title = "Ticket 14" },
			new Ticket(_fakeClock) { Id = 15, Title = "Ticket 15" }
		};

		_mockRepo.Setup(repo => repo.GetAll(queryParams)).Returns(tickets.AsQueryable());

		var result = _service.GetAll(queryParams);

		Assert.Equal(10, result.Items.Count);  
		Assert.Equal("Ticket 15", result.Items[0].Title); 
		Assert.Equal("Ticket 6", result.Items[9].Title);  
	}

	[Fact]
	public void GetAll_ShouldSkipTenItems_WhenSecondPageIsRequested()
	{
		var queryParams = new TicketQueryParameters
		{
			Page = 2,
			PageSize = 10,
		};

		var tickets = new List<Ticket>
		{
			new Ticket(_fakeClock) { Id = 1, Title = "Ticket 1" },
			new Ticket(_fakeClock) { Id = 2, Title = "Ticket 2" },
			new Ticket(_fakeClock) { Id = 3, Title = "Ticket 3" },
			new Ticket(_fakeClock) { Id = 4, Title = "Ticket 4" },
			new Ticket(_fakeClock) { Id = 5, Title = "Ticket 5" },
			new Ticket(_fakeClock) { Id = 6, Title = "Ticket 6" },
			new Ticket(_fakeClock) { Id = 7, Title = "Ticket 7" },
			new Ticket(_fakeClock) { Id = 8, Title = "Ticket 8" },
			new Ticket(_fakeClock) { Id = 9, Title = "Ticket 9" },
			new Ticket(_fakeClock) { Id = 10, Title = "Ticket 10" },
			new Ticket(_fakeClock) { Id = 11, Title = "Ticket 11" },
			new Ticket(_fakeClock) { Id = 12, Title = "Ticket 12" },
			new Ticket(_fakeClock) { Id = 13, Title = "Ticket 13" },
			new Ticket(_fakeClock) { Id = 14, Title = "Ticket 14" },
			new Ticket(_fakeClock) { Id = 15, Title = "Ticket 15" }
		};

		_mockRepo.Setup(repo => repo.GetAll(queryParams)).Returns(tickets.AsQueryable());

		var result = _service.GetAll(queryParams);

		Assert.Equal(5, result.Items.Count);
		Assert.Equal("Ticket 5", result.Items[0].Title);
		Assert.Equal("Ticket 1", result.Items[4].Title);
	}
}

