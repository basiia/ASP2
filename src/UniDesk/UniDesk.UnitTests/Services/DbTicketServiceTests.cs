using Xunit;
using Moq;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using UniDesk.Web.DTOs;
using UniDesk.UnitTests.Fakes;
using System.Collections.Generic;

namespace UniDesk.UnitTests.Services
{
	public class DbTicketServiceTests
	{
		private readonly Mock<ITicketRepository> _mockRepo;
		private readonly DbTicketService _service;
		private readonly ISystemClock _fakeClock;

		// Конструктор для инициализации объектов
		public DbTicketServiceTests()
		{
			_mockRepo = new Mock<ITicketRepository>();  // Инициализация мок-объекта для репозитория
			_fakeClock = new FakeClock();  // Инициализация FakeClock для использования в тестах
			_service = new DbTicketService(_mockRepo.Object, _fakeClock);  // Создание экземпляра DbTicketService с _fakeClock
		}

		// Тест для обновления статуса тикета
		[Fact]
		public void UpdateStatus_ShouldChangeStatus_WhenValidStatusIsProvided()
		{
			// Создаем тикет, передаем _fakeClock в конструктор
			var ticket = new Ticket(_fakeClock)
			{
				Title = "Sample Title",
				Description = "Sample Description"
			};

			// Настройка мока репозитория для возврата созданного тикета
			_mockRepo.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(ticket);

			// Обновление статуса тикета через сервис
			_service.UpdateStatus(ticket.Id, TicketStatus.InProgress);

			// Проверка, что статус был изменен на "InProgress"
			Assert.Equal(TicketStatus.InProgress, ticket.Status);
			_mockRepo.Verify(repo => repo.Update(ticket), Times.Once);  // Проверяем, что обновление было вызвано один раз
		}

		// Тест для добавления тикета
		[Fact]
		public void Add_ShouldAddTicket_WhenValidTicket()
		{
			// Создаем новый тикет
			var ticket = new Ticket(_fakeClock)
			{
				Title = "New Ticket",
				Description = "Sample Description",
				Status = TicketStatus.Open
			};

			// Вызов метода Add сервиса
			_service.Add(ticket);

			// Проверка, что метод Add был вызван один раз
			_mockRepo.Verify(m => m.Add(It.IsAny<Ticket>()), Times.Once);
		}

		// Тест для получения списка тикетов с пагинацией
		[Fact]
		public void GetAll_ShouldReturnPagedResults_WhenPageSizeIsSet()
		{
			// Настройка параметров запроса
			var queryParams = new TicketQueryParameters
			{
				Page = 1,
				PageSize = 10,
			};

			// Мокируем данные тикетов
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

			// Настройка мока репозитория для возврата списка тикетов
			_mockRepo.Setup(repo => repo.GetAll(queryParams)).Returns(tickets.AsQueryable());

			// Вызов метода GetAll для получения тикетов с пагинацией
			var result = _service.GetAll(queryParams);

			// Проверка правильности пагинации
			Assert.Equal(10, result.Items.Count);  // Проверка, что в ответе 10 элементов
			Assert.Equal("Ticket 1", result.Items[0].Title);  // Проверка первого элемента
			Assert.Equal("Ticket 10", result.Items[9].Title);  // Проверка последнего элемента на странице
		}
	}
}