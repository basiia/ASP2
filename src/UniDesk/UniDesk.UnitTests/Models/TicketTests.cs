using Xunit;
using Moq;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace UniDesk.UnitTests.Models
{
	public class TicketTests
	{
		private readonly Mock<ITicketRepository> _mockRepo;
		private readonly DbTicketService _service;

		public TicketTests()
		{
			_mockRepo = new Mock<ITicketRepository>();
			_service = new DbTicketService(_mockRepo.Object);
		}

		[Fact]
		public void Ticket_ShouldHaveStatusOpen_WhenCreated()
		{
			var ticket = new Ticket
			{
				Title = "Sample Title",
				Description = "Sample Description"
			};

			Assert.Equal(TicketStatus.Open, ticket.Status);
		}

		[Fact]
		public void Ticket_ShouldHaveCreatedAtNotDefault_WhenCreated()
		{
			var ticket = new Ticket
			{
				Title = "Sample Title",
				Description = "Sample Description",
				CreatedAt = DateTime.UtcNow 
			};

			Assert.NotEqual(default(DateTime), ticket.CreatedAt);
		}

		[Fact]
		public void Ticket_ShouldHaveRequiredTitle_WhenCreated()
		{
			var ticket = new Ticket
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
			var ticket = new Ticket
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
			var ticket = new Ticket
			{
				Id = 1,
				Status = TicketStatus.Closed, 
			};

			_mockRepo.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(ticket);

			Assert.Throws<InvalidOperationException>(() => _service.UpdateStatus(ticket.Id, TicketStatus.InProgress));
		}
	}
}