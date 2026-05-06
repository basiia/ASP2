using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace UniDesk.IntegrationTests
{
    public class TicketValidationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TicketValidationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void CreateTicket_ShouldRejectInvalidInput_WhenTitleIsEmpty()
        {
            var json = """
			{
				"title": "",
				"description": "Office printer not working"
			}
			""";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _client.PostAsync("/api/tickets", content)
                .GetAwaiter()
                .GetResult();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void CreateTicket_ShouldRejectInvalidInput_WhenDescriptionIsEmpty()
        {
            var json = """
			{
				"title": "Printer broken",
				"description": ""
			}
			""";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _client.PostAsync("/api/tickets", content)
                .GetAwaiter()
                .GetResult();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}