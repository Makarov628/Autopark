using System.Net;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.DependencyInjection;

namespace Autopark.Tests.Infrastructure;

public class CsrfProtectionTests : TestBase
{
    public CsrfProtectionTests() : base()
    {
    }

    [Fact]
    public void AntiforgeryServiceRegistered()
    {
        using var scope = Factory.Services.CreateScope();
        var antiforgery = scope.ServiceProvider.GetService<IAntiforgery>();

        Assert.NotNull(antiforgery);
    }

    [Fact]
    public async Task Post_WithoutCsrfToken_ShouldReturn400()
    {
        var client = CreateHttpClient();

        var content = new StringContent("{ \"name\": \"Предприятие\", \"address\": \"Адрес\" }", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/Enterprises", content);
        var data = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid CSRF token", data);
    }

    [Fact]
    public async Task Post_WithValidCsrfToken_ShouldSucceed()
    {
        var client = CreateHttpClient();

        var tokenResponse = await client.GetAsync("/api/csrf/token");
        Assert.True(tokenResponse.IsSuccessStatusCode);

        var tokenData = await tokenResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(tokenData);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Enterprises");
        request.Content = new StringContent("{ \"name\": \"Предприятие\", \"address\": \"Адрес\" }", Encoding.UTF8, "application/json");
        request.Headers.Add("X-CSRF-TOKEN", token);

        var response = await client.SendAsync(request);

        Assert.NotEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithoutCsrfToken_ShouldSucceed()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/Vehicles");

        Assert.NotEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CsrfToken_Endpoint_ShouldReturnToken()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/csrf/token");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", content);
        Assert.Contains("headerName", content);
    }

    [Fact]
    public async Task Login_WithoutCsrfToken_ShouldSucceed()
    {
        var client = CreateHttpClient();

        var content = new StringContent("{\"email\":\"test@test.com\",\"password\":\"test\"}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/login?useCookies=true", content);

        Assert.NotEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestServer_ShouldHaveSwaggerEndpoint()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/swagger");

        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    private string ExtractTokenFromResponse(string jsonResponse)
    {
        var tokenStart = jsonResponse.IndexOf("\"token\":\"") + 9;
        var tokenEnd = jsonResponse.IndexOf("\"", tokenStart);
        return jsonResponse.Substring(tokenStart, tokenEnd - tokenStart);
    }
}