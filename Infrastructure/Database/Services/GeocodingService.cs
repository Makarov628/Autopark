using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Autopark.Infrastructure.Database.Services;

public interface IGeocodingService
{
    Task<string?> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string?>> GetAddressesBatchAsync(IEnumerable<(double Latitude, double Longitude)> coordinates, CancellationToken cancellationToken = default);
}

public class GeoapifyGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.geoapify.com/v1/geocode";

    public GeoapifyGeocodingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Geoapify:ApiKey"] ?? throw new InvalidOperationException("Geoapify API key is not configured");
    }

    public async Task<string?> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/reverse?lat={latitude:F6}&lon={longitude:F6}&apiKey={_apiKey}&format=json";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GeoapifyReverseResponse>(json);

            return result?.Results?.FirstOrDefault()?.Formatted;
        }
        catch (Exception)
        {
            // Логируем ошибку, но не прерываем выполнение
            return null;
        }
    }

    public async Task<Dictionary<string, string?>> GetAddressesBatchAsync(
        IEnumerable<(double Latitude, double Longitude)> coordinates,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, string?>();
        var coordinatesList = coordinates.ToList();

        if (!coordinatesList.Any())
            return result;

        try
        {
            // Geoapify поддерживает batch запросы до 1000 точек
            const int batchSize = 100; // Используем меньший размер для надежности

            for (int i = 0; i < coordinatesList.Count; i += batchSize)
            {
                var batch = coordinatesList.Skip(i).Take(batchSize).ToList();
                var batchResults = await ProcessBatch(batch, cancellationToken);

                foreach (var kvp in batchResults)
                {
                    result[kvp.Key] = kvp.Value;
                }

                // Небольшая задержка между батчами для соблюдения лимитов API
                if (i + batchSize < coordinatesList.Count)
                {
                    await Task.Delay(100, cancellationToken);
                }
            }
        }
        catch (Exception)
        {
            // В случае ошибки возвращаем частичные результаты
        }

        return result;
    }

    private async Task<Dictionary<string, string?>> ProcessBatch(
        List<(double Latitude, double Longitude)> batch,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string?>();

        // Для простоты делаем отдельные запросы для каждой точки
        // В реальном проекте можно использовать batch API Geoapify
        foreach (var (latitude, longitude) in batch)
        {
            var key = $"{latitude:F6},{longitude:F6}";
            var address = await GetAddressAsync(latitude, longitude, cancellationToken);
            result[key] = address;

            // Небольшая задержка между запросами
            await Task.Delay(50, cancellationToken);
        }

        return result;
    }
}

// DTO для ответа Geoapify
public class GeoapifyReverseResponse
{
    [JsonPropertyName("results")]
    public GeoapifyResult[]? Results { get; set; }
}

public class GeoapifyResult
{
    [JsonPropertyName("formatted")]
    public string? Formatted { get; set; }

    [JsonPropertyName("address_line1")]
    public string? AddressLine1 { get; set; }

    [JsonPropertyName("address_line2")]
    public string? AddressLine2 { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}