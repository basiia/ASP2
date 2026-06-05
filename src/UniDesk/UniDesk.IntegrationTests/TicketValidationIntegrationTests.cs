using System.Net;
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
        public async Task CreateTicket_ShouldRejectInvalidInput_WhenTitleIsEmpty()
        {
            await TestAuthHelper.LoginAsEmployeeAsync(_client);

            var json = """
            {
                "title": "",
                "description": "Office printer not working"
            }
            """;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/tickets", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_ShouldRejectInvalidInput_WhenDescriptionIsEmpty()
        {
            await TestAuthHelper.LoginAsEmployeeAsync(_client);

            var json = """
            {
                "title": "Printer broken",
                "description": ""
            }
            """;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/tickets", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
