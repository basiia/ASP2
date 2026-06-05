using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace UniDesk.IntegrationTests;

public static class TestAuthHelper
{
    public static async Task LoginAsEmployeeAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/ambitne/login",
            new
            {
                email = "employee@top-uni.edu.pl",
                password = "Employee123!"
            });

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var token = document.RootElement.GetProperty("accessToken").GetString();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}
