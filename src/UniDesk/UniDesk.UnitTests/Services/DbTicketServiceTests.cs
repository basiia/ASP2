using Xunit;
using Moq;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

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
	}
}