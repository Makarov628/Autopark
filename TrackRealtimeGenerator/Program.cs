using System.Text.Json;
using System.Net.Http;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using NetTopologySuite.Geometries;
using NetTopologySuite;

// Параметры запуска
int vehicleId = 42;
double centerLat = 43.2567;
double centerLon = 76.9286;
double radius = 500.0; // км
int lengthKm = 100;
int stepSec = 1;
string connectionString = "Data Source=localhost;Initial Catalog=Autopark;User Id=sa;Password=L@m3daSc;TrustServerCertificate=True";
string geoapifyApiKey = "3da887bda8ab4509ba8776df90d1c554"; // Замените на ваш API ключ

// Парсим аргументы (примитивно)
foreach (var arg in args)
{
    var parts = arg.Split('=', 2);
    if (parts.Length != 2) continue;
    switch (parts[0].ToLower())
    {
        case "vehicleid": vehicleId = int.Parse(parts[1]); break;
        case "centerlat": centerLat = double.Parse(parts[1], CultureInfo.InvariantCulture); break;
        case "centerlon": centerLon = double.Parse(parts[1], CultureInfo.InvariantCulture); break;
        case "radius": radius = double.Parse(parts[1], CultureInfo.InvariantCulture); break;
        case "lengthkm": lengthKm = int.Parse(parts[1]); break;
        case "stepsec": stepSec = int.Parse(parts[1]); break;
        case "connectionstring": connectionString = parts[1]; break;
        case "geoapifykey": geoapifyApiKey = parts[1]; break;
    }
}

Console.WriteLine($"Генерация трека для vehicleId={vehicleId}, центр=({centerLat},{centerLon}), радиус={radius}км, длина={lengthKm}км, шаг={stepSec}с");

// 1. Случайно выбираем старт и финиш в радиусе
Random rnd = new();
(double dx, double dy) RandInRadius()
{
    double angle = rnd.NextDouble() * 2 * Math.PI;
    double r = radius * Math.Sqrt(rnd.NextDouble());
    double dx = r * Math.Cos(angle) / 111.0; // ~1 градус = 111 км
    double dy = r * Math.Sin(angle) / 111.0;
    return (dx, dy);
}
(double dx1, double dy1) = RandInRadius();
(double dx2, double dy2) = RandInRadius();
double startLat = centerLat + dy1;
double startLon = centerLon + dx1;
double endLat = centerLat + dy2;
double endLon = centerLon + dx2;

// 2. Запрашиваем маршрут у OSRM
string osrmUrl = $"https://router.project-osrm.org/route/v1/driving/{startLon.ToString(CultureInfo.InvariantCulture)},{startLat.ToString(CultureInfo.InvariantCulture)};{endLon.ToString(CultureInfo.InvariantCulture)},{endLat.ToString(CultureInfo.InvariantCulture)}?overview=full&geometries=geojson";
Console.WriteLine($"OSRM route: {osrmUrl}");

using var http = new HttpClient();
var resp = await http.GetAsync(osrmUrl);
if (!resp.IsSuccessStatusCode)
{
    Console.WriteLine($"Ошибка OSRM: {resp.StatusCode}");
    return;
}
using var stream = await resp.Content.ReadAsStreamAsync();
using var doc = await JsonDocument.ParseAsync(stream);
var coords = doc.RootElement
    .GetProperty("routes")[0]
    .GetProperty("geometry")
    .GetProperty("coordinates")
    .EnumerateArray()
    .Select(x => (lon: x[0].GetDouble(), lat: x[1].GetDouble()))
    .ToList();

if (coords.Count < 2)
{
    Console.WriteLine("OSRM не вернул маршрут");
    return;
}

// 3. Генерируем точки вдоль маршрута с шагом по времени
var totalDist = 0.0;
var points = new List<(double lon, double lat, double dist)>();
(double prevLon, double prevLat) = coords[0];
points.Add((prevLon, prevLat, 0));
for (int i = 1; i < coords.Count; i++)
{
    var (lon, lat) = coords[i];
    double d = Haversine(prevLat, prevLon, lat, lon);
    totalDist += d;
    points.Add((lon, lat, totalDist));
    prevLon = lon; prevLat = lat;
}
if (totalDist < 0.1)
{
    Console.WriteLine("Маршрут слишком короткий");
    return;
}

// Обрезаем маршрут до нужной длины
points = points.Where(p => p.dist <= lengthKm).ToList();
if (points.Count < 2)
{
    Console.WriteLine("Маршрут слишком короткий после обрезки");
    return;
}

// 4. Создаем поездку и записываем точки в базу данных
var options = new DbContextOptionsBuilder<AutoparkDbContext>()
    .UseSqlServer(connectionString, x => x.UseNetTopologySuite())
    .Options;

using var context = new AutoparkDbContext(options);

var vehicleIdValue = VehicleId.Create(vehicleId);
float speed = 30;
ushort rpm = 1500;
byte fuel = 80;

// Создаем геометрическую фабрику для работы с координатами
var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

// Находим последнюю поездку для этого автомобиля
var lastTrip = await context.Trips
    .Where(t => t.VehicleId == vehicleIdValue)
    .OrderByDescending(t => t.EndUtc)
    .FirstOrDefaultAsync();

// Определяем время начала новой поездки
var startTime = lastTrip != null
    ? lastTrip.EndUtc.AddMinutes(1) // Добавляем минуту после окончания последней поездки
    : DateTime.UtcNow; // Если поездок нет, используем текущее время

var endTime = startTime.AddSeconds((points.Count - 1) * stepSec);

Console.WriteLine($"Последняя поездка: {(lastTrip != null ? $"#{lastTrip.Id.Value} завершена {lastTrip.EndUtc:yyyy-MM-dd HH:mm:ss}" : "не найдена")}");
Console.WriteLine($"Новая поездка будет: {startTime:yyyy-MM-dd HH:mm:ss} - {endTime:yyyy-MM-dd HH:mm:ss}");
// Используем координаты из маршрута OSRM
var (routeStartLon, routeStartLat, _) = points.First();
var (routeEndLon, routeEndLat, _) = points.Last();

// Получаем адреса для начальной и конечной точек через Geoapify
Console.WriteLine("Получение адресов через Geoapify...");
string? startAddress = await GetAddressFromGeoapifyAsync(routeStartLat, routeStartLon, geoapifyApiKey, http);
string? endAddress = await GetAddressFromGeoapifyAsync(routeEndLat, routeEndLon, geoapifyApiKey, http);

Console.WriteLine($"Начальный адрес: {startAddress ?? "Не определен"}");
Console.WriteLine($"Конечный адрес: {endAddress ?? "Не определен"}");

// Создаем поездку (ID будет автоматически сгенерирован базой данных)
var trip = TripEntity.Create(
    TripId.Create(0), // Временный ID, будет заменен при сохранении
    vehicleIdValue,
    startTime,
    endTime,
    (double)points.Last().dist
);

context.Trips.Add(trip);
await context.SaveChangesAsync();

Console.WriteLine($"Создана поездка #{trip.Id.Value} от {startTime:HH:mm:ss} до {endTime:HH:mm:ss}");

// Создаем начальную точку поездки с адресом
var startPoint = TripPointEntity.Create(routeStartLat, routeStartLon, startAddress, geometryFactory);
context.TripPoints.Add(startPoint);
await context.SaveChangesAsync();

// Создаем конечную точку поездки с адресом
var endPoint = TripPointEntity.Create(routeEndLat, routeEndLon, endAddress, geometryFactory);
context.TripPoints.Add(endPoint);
await context.SaveChangesAsync();

// Связываем точки с поездкой
trip.SetStartPoint(startPoint.Id);
trip.SetEndPoint(endPoint.Id);
await context.SaveChangesAsync();

Console.WriteLine($"Созданы начальная и конечная точки поездки с адресами");

// Записываем точки трека
for (int i = 0; i < points.Count; i++)
{
    var (lon, lat, dist) = points[i];
    var ts = startTime.AddSeconds(i * stepSec);

    // Добавляем случайный разброс параметров
    speed = 30 + (float)(rnd.NextDouble() * 20);
    rpm = (ushort)(1500 + rnd.Next(0, 500));
    fuel = (byte)Math.Max(0, fuel - rnd.Next(0, 1));

    var trackPoint = VehicleTrackPointEntity.Create(
        vehicleIdValue,
        ts,
        lat,
        lon,
        speed,
        rpm,
        fuel
    );

    context.VehicleTrackPoints.Add(trackPoint);
    await context.SaveChangesAsync();


    Console.WriteLine($"[{ts:HH:mm:ss}] {lat:F5},{lon:F5} speed={speed:F1} fuel={fuel}");
    await Task.Delay(stepSec * 100);
}

Console.WriteLine("Генерация трека завершена!");

// ---
// Функция для получения адреса по координатам через Geoapify
async Task<string?> GetAddressFromGeoapifyAsync(double lat, double lon, string apiKey, HttpClient httpClient)
{
    try
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEOAPIFY_API_KEY")
        {
            Console.WriteLine("API ключ Geoapify не настроен");
            return null;
        }

        string url = $"https://api.geoapify.com/v1/geocode/reverse?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&apiKey={apiKey}";

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Ошибка получения адреса от Geoapify: {response.StatusCode}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("features", out var features) && features.GetArrayLength() > 0)
        {
            var firstFeature = features[0];
            if (firstFeature.TryGetProperty("properties", out var properties))
            {
                if (properties.TryGetProperty("formatted", out var formatted))
                {
                    return formatted.GetString();
                }
            }
        }

        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при получении адреса от Geoapify: {ex.Message}");
        return null;
    }
}

// Haversine formula для расчёта расстояния между точками (км)
double Haversine(double lat1, double lon1, double lat2, double lon2)
{
    double R = 6371; // радиус Земли, км
    double dLat = (lat2 - lat1) * Math.PI / 180.0;
    double dLon = (lon2 - lon1) * Math.PI / 180.0;
    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
               Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
               Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    return R * c;
}
