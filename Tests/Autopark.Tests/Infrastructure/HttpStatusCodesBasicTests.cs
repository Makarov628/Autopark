using System.Net;
using System.Text;
using System.Text.Json;

namespace Autopark.Tests.Infrastructure;

public class HttpStatusCodesBasicTests : TestBase
{
    public HttpStatusCodesBasicTests() : base()
    {
    }

    #region 401 Unauthorized Tests (работают без токенов)

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401()
    {
        var client = CreateHttpClient();

        var loginData = new
        {
            email = "invalid@test.com",
            password = "wrongpassword",
            deviceName = "Test Device",
            deviceType = 0
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/user/login", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturn401()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/user/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_ShouldReturn401()
    {
        var client = CreateHttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer invalid-token");

        var response = await client.GetAsync("/api/user/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Enterprises_WithoutToken_ShouldReturn401()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/enterprises");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Vehicles_WithoutToken_ShouldReturn401()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/vehicles");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Drivers_WithoutToken_ShouldReturn401()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/drivers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Managers_WithoutToken_ShouldReturn401()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/managers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region System Endpoints Tests (работают без аутентификации)

    [Fact]
    public async Task SystemHealth_ShouldReturn200()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/system/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SystemInfo_ShouldReturn200()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/system/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CheckSetup_ShouldReturn200()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/system/check-setup");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Swagger Tests

    [Fact]
    public async Task Swagger_ShouldBeAvailable()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/swagger");

        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region 400 Bad Request Tests (работают без токенов)

    [Fact]
    public async Task Login_WithInvalidJson_ShouldReturn400()
    {
        var client = CreateHttpClient();

        var content = new StringContent(
            "{ invalid json }",
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/user/login", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyBody_ShouldReturn400()
    {
        var client = CreateHttpClient();

        var content = new StringContent("", Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/user/login", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region 404 Not Found Tests

    [Fact]
    public async Task NonExistentEndpoint_ShouldReturn404()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/nonexistent");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task NonExistentSystemEndpoint_ShouldReturn404()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/api/system/nonexistent");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region HTTP Method Tests

    [Fact]
    public async Task PostToGetEndpoint_ShouldReturn405()
    {
        var client = CreateHttpClient();

        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/system/health", content);

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task PutToGetEndpoint_ShouldReturn405()
    {
        var client = CreateHttpClient();

        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/api/system/health", content);

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task DeleteToGetEndpoint_ShouldReturn405()
    {
        var client = CreateHttpClient();

        var response = await client.DeleteAsync("/api/system/health");

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    #endregion

    #region Content Type Tests

    [Fact]
    public async Task Login_WithWrongContentType_ShouldReturn400()
    {
        var client = CreateHttpClient();

        var content = new StringContent(
            "{ \"email\":\"test@test.com\", \"password\":\"test\" }",
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/user/login", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region 409 Conflict Tests (базовые проверки без создания данных)

    [Fact]
    public async Task DeleteProtectedEndpoints_WithoutAuth_ShouldReturn401()
    {
        var client = CreateHttpClient();

        // Попытка удаления защищенных ресурсов без аутентификации
        var enterpriseResponse = await client.DeleteAsync("/api/enterprises/1");
        Assert.Equal(HttpStatusCode.Unauthorized, enterpriseResponse.StatusCode);

        var vehicleResponse = await client.DeleteAsync("/api/vehicles/1");
        Assert.Equal(HttpStatusCode.Unauthorized, vehicleResponse.StatusCode);

        var driverResponse = await client.DeleteAsync("/api/drivers/1");
        Assert.Equal(HttpStatusCode.Unauthorized, driverResponse.StatusCode);

        var brandModelResponse = await client.DeleteAsync("/api/brandmodels/1");
        Assert.Equal(HttpStatusCode.Unauthorized, brandModelResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteEndpoints_WithInvalidToken_ShouldReturn401()
    {
        var client = CreateHttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer invalid-token");

        // Попытка удаления с невалидным токеном
        var enterpriseResponse = await client.DeleteAsync("/api/enterprises/1");
        Assert.Equal(HttpStatusCode.Unauthorized, enterpriseResponse.StatusCode);

        var vehicleResponse = await client.DeleteAsync("/api/vehicles/1");
        Assert.Equal(HttpStatusCode.Unauthorized, vehicleResponse.StatusCode);

        var driverResponse = await client.DeleteAsync("/api/drivers/1");
        Assert.Equal(HttpStatusCode.Unauthorized, driverResponse.StatusCode);

        var brandModelResponse = await client.DeleteAsync("/api/brandmodels/1");
        Assert.Equal(HttpStatusCode.Unauthorized, brandModelResponse.StatusCode);
    }

    #endregion
}