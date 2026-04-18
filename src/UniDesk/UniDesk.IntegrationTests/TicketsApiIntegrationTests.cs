using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UniDesk.Web.DTOs;
using Xunit;

namespace UniDesk.IntegrationTests;

public class TicketsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public TicketsApiIntegrationTests(WebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task GetTickets_ShouldReturnOkAndPagedResult()
	{
		var response = await _client.GetAsync("/api/tickets");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<PagedResult<TicketListDto>>();

		Assert.NotNull(result);
		Assert.NotNull(result.Items);
	}

	[Fact]
	public async Task CreateTicket_ShouldReturnCreated()
	{
		var request = new CreateTicketRequest
		{
			Title = "Test ticket",
			Description = "Test description"
		};

		var response = await _client.PostAsJsonAsync("/api/tickets", request);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var created = await response.Content.ReadFromJsonAsync<TicketReadDto>();

		Assert.NotNull(created);
		Assert.Equal("Test ticket", created.Title);
		Assert.Equal("Open", created.Status);
	}

	[Fact]
	public async Task CreateTicket_ShouldReturnBadRequest_WhenTitleIsEmpty()
	{
		var request = new CreateTicketRequest
		{
			Title = "",
			Description = "Test description"
		};

		var response = await _client.PostAsJsonAsync("/api/tickets", request);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}