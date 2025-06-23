using System.Net;
using System.Text;
using System.Text.Json;

namespace Autopark.Tests.Infrastructure;

public class HttpStatusCodesIntegrationTests : AuthenticatedTestBase
{
    public HttpStatusCodesIntegrationTests() : base()
    {
    }

    #region 401 Unauthorized Tests

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
        var client = CreateAuthenticatedClient(await GetInvalidTokenAsync());

        var response = await client.GetAsync("/api/user/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region 403 Forbidden Tests

    [Fact]
    public async Task DeleteEnterprise_AsRegularUser_ShouldReturn403()
    {
        // Создаём обычного пользователя (без роли Manager/Admin)
        var client = CreateAuthenticatedClient(await GetUserTokenAsync());

        var response = await client.DeleteAsync("/api/enterprises/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateManager_AsManager_ShouldReturn403()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var managerData = new
        {
            userId = 1,
            enterpriseIds = new[] { 1 }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(managerData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/managers", content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateEnterprise_AsManager_ShouldReturn403()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var enterpriseData = new
        {
            name = "Test Enterprise",
            address = "Test Address"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(enterpriseData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/enterprises", content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region 404 Not Found Tests

    [Fact]
    public async Task GetEnterprise_NonExistentId_ShouldReturn404()
    {
        var client = CreateAuthenticatedClient(await GetAdminTokenAsync());

        var response = await client.GetAsync("/api/enterprises/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetVehicle_NonExistentId_ShouldReturn404()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var response = await client.GetAsync("/api/vehicles/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDriver_NonExistentId_ShouldReturn404()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var response = await client.GetAsync("/api/drivers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region 409 Conflict Tests

    [Fact]
    public async Task DeleteEnterprise_WithVehicles_ShouldReturn409()
    {
        // Создаём предприятие и транспортное средство
        var client = CreateAuthenticatedClient(await GetAdminTokenAsync());

        // Сначала создаём предприятие
        var enterpriseData = new
        {
            name = "Test Enterprise for Conflict",
            address = "Test Address"
        };

        var enterpriseContent = new StringContent(
            JsonSerializer.Serialize(enterpriseData),
            Encoding.UTF8,
            "application/json");

        var enterpriseResponse = await client.PostAsync("/api/enterprises", enterpriseContent);
        Assert.Equal(HttpStatusCode.Created, enterpriseResponse.StatusCode);

        // Получаем ID созданного предприятия
        var enterprisesResponse = await client.GetAsync("/api/enterprises");
        var enterprisesContent = await enterprisesResponse.Content.ReadAsStringAsync();
        var enterprises = JsonSerializer.Deserialize<List<dynamic>>(enterprisesContent);
        var enterpriseId = enterprises.First(e => e.GetProperty("name").GetString() == "Test Enterprise for Conflict").GetProperty("id").GetInt32();

        // Создаём модель бренда
        var brandModelData = new
        {
            brandName = "Test Brand",
            modelName = "Test Model",
            transportType = 0,
            fuelType = 0,
            seatsNumber = 5u,
            maximumLoadCapacityInKillograms = 1000u
        };

        var brandModelContent = new StringContent(
            JsonSerializer.Serialize(brandModelData),
            Encoding.UTF8,
            "application/json");

        var brandModelResponse = await client.PostAsync("/api/brandmodels", brandModelContent);
        Assert.Equal(HttpStatusCode.Created, brandModelResponse.StatusCode);

        // Получаем ID созданной модели бренда
        var brandModelsResponse = await client.GetAsync("/api/brandmodels");
        var brandModelsContent = await brandModelsResponse.Content.ReadAsStringAsync();
        var brandModels = JsonSerializer.Deserialize<List<dynamic>>(brandModelsContent);
        var brandModelId = brandModels.First(b => b.GetProperty("brandName").GetString() == "Test Brand").GetProperty("id").GetInt32();

        // Создаём транспортное средство
        var vehicleData = new
        {
            name = "Test Vehicle",
            price = 100000m,
            mileageInKilometers = 1000.0,
            color = "Красный",
            registrationNumber = "А123БВ77",
            brandModelId = brandModelId,
            enterpriseId = enterpriseId
        };

        var vehicleContent = new StringContent(
            JsonSerializer.Serialize(vehicleData),
            Encoding.UTF8,
            "application/json");

        var vehicleResponse = await client.PostAsync("/api/vehicles", vehicleContent);
        Assert.Equal(HttpStatusCode.Created, vehicleResponse.StatusCode);

        // Теперь пытаемся удалить предприятие - должно вернуть 409
        var deleteResponse = await client.DeleteAsync($"/api/enterprises/{enterpriseId}");

        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
    }

    #endregion

    #region 400 Bad Request Tests

    [Fact]
    public async Task CreateEnterprise_InvalidData_ShouldReturn400()
    {
        var client = CreateAuthenticatedClient(await GetAdminTokenAsync());

        var enterpriseData = new
        {
            name = "", // Пустое обязательное поле
            address = ""
        };

        var content = new StringContent(
            JsonSerializer.Serialize(enterpriseData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/enterprises", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_InvalidData_ShouldReturn400()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var vehicleData = new
        {
            registrationNumber = "", // Пустое обязательное поле
            brandModelId = 0 // Невалидный ID
        };

        var content = new StringContent(
            JsonSerializer.Serialize(vehicleData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/vehicles", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateDriver_InvalidData_ShouldReturn400()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var driverData = new
        {
            userId = 0, // Невалидный ID
            licenseNumber = "" // Пустое обязательное поле
        };

        var content = new StringContent(
            JsonSerializer.Serialize(driverData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/drivers", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Success Status Codes Tests

    [Fact]
    public async Task GetEnterprises_AsManager_ShouldReturn200()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var response = await client.GetAsync("/api/enterprises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetVehicles_AsManager_ShouldReturn200()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var response = await client.GetAsync("/api/vehicles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDrivers_AsManager_ShouldReturn200()
    {
        var client = CreateAuthenticatedClient(await GetManagerTokenAsync());

        var response = await client.GetAsync("/api/drivers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region System Endpoints Tests

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

    [Fact]
    public async Task SwaggerJson_ShouldBeAvailable()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion
}