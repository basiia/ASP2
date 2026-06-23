using Xunit;
using Moq;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using System;
using System.ComponentModel.DataAnnotations;
using UniDesk.UnitTests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace UniDesk.UnitTests.Models;
public class TicketTests
{
	private readonly Mock<ITicketRepository> _mockRepo;
	private readonly TicketService _service;
	private readonly ISystemClock _fakeClock;

	public TicketTests()
	{
		_mockRepo = new Mock<ITicketRepository>();
		_fakeClock = new FakeClock();
        _service = new TicketService(
            _mockRepo.Object,
            _fakeClock,
            NullLogger<TicketService>.Instance);
    }

	[Fact]
	public void Ticket_ShouldHaveCreatedAt_WhenCreated()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Title = "Sample Title",
			Description = "Sample Description"
		};

		Assert.Equal(new DateTime(2026, 04, 08, 12, 00, 00, DateTimeKind.Utc), ticket.CreatedAt);
	}

	[Fact]
	public void Ticket_ShouldHaveStatusNew_WhenCreated()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Title = "Sample Title",
			Description = "Sample Description"
		};

		Assert.Equal(TicketStatus.New, ticket.Status);
	}

	[Fact]
	public void Ticket_ShouldHaveRequiredTitle_WhenCreated()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Title = "Sample Title",
			Description = "Sample Description"
		};

		var validationContext = new ValidationContext(ticket) { MemberName = "Title" };
		var validationResults = new System.Collections.Generic.List<ValidationResult>();
		bool isValid = Validator.TryValidateProperty(ticket.Title, validationContext, validationResults);

		Assert.True(isValid);
	}

	[Fact]
	public void Ticket_ShouldHaveRequiredDescription_WhenCreated()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Title = "Sample Title",
			Description = "Sample Description"
		};

		var validationContext = new ValidationContext(ticket) { MemberName = "Description" };
		var validationResults = new System.Collections.Generic.List<ValidationResult>();
		bool isValid = Validator.TryValidateProperty(ticket.Description, validationContext, validationResults);

		Assert.True(isValid);
	}

	[Fact]
	public void UpdateStatus_ShouldThrowException_WhenTicketIsAlreadyClosed()
	{
		var ticket = new Ticket(_fakeClock)
		{
			Id = 1,
			Status = TicketStatus.Closed,
		};

		_mockRepo.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(ticket);

		Assert.Throws<InvalidOperationException>(() => _service.UpdateStatus(ticket.Id, TicketStatus.InProgress));
	}
}

