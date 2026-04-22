using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
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
		Assert.NotNull(result!.Items);
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
		Assert.Equal("Test ticket", created!.Title);
		Assert.Equal("Open", created.Status);
	}

	[Fact]
	public async Task CreateTicket_ShouldReturnValidationProblemDetails_WhenTitleIsEmpty()
	{
		var request = new CreateTicketRequest
		{
			Title = "",
			Description = "Test description"
		};

		var response = await _client.PostAsJsonAsync("/api/tickets", request);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

		var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

		Assert.NotNull(problem);
		Assert.Equal(400, problem!.Status);
		Assert.False(string.IsNullOrWhiteSpace(problem.Title));
		Assert.True(problem.Errors.ContainsKey("Title"));
	}

	[Fact]
	public async Task GetTicketById_ShouldReturnNotFound()
	{
		var response = await _client.GetAsync("/api/tickets/9999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task UpdateStatus_ShouldReturnNoContent()
	{
		var createRequest = new CreateTicketRequest
		{
			Title = "Test ticket",
			Description = "Test description"
		};

		var createResponse = await _client.PostAsJsonAsync("/api/tickets", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<TicketReadDto>();

		Assert.NotNull(created);

		var updateRequest = new UpdateTicketStatusRequest
		{
			Status = 2
		};

		var response = await _client.PatchAsJsonAsync(
			$"/api/tickets/{created!.Id}/status",
			updateRequest
		);

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Fact]
	public async Task SwaggerDocument_ShouldBeAvailable()
	{
		var response = await _client.GetAsync("/swagger/v1/swagger.json");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var content = await response.Content.ReadAsStringAsync();

		Assert.False(string.IsNullOrWhiteSpace(content));
		Assert.Contains("openapi", content.ToLower());
		Assert.Contains("/api/tickets", content);
	}
}