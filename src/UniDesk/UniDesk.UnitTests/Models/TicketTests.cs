using Xunit;
using UniDesk.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace UniDesk.UnitTests.Models
{
	public class TicketTests
	{
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
	}
}