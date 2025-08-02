using System.Text.Json;
using System.Net.Http;
using Octonica.ClickHouseClient;
using System.Globalization;

// Параметры запуска
uint vehicleId = 42;
double centerLat = 43.2567;
double centerLon = 76.9286;
double radius = 500.0; // км
int lengthKm = 100;
int stepSec = 1;
string clickhouseConn = "Host=localhost;Database=my_database;User=username;Password=password";

// Парсим аргументы (примитивно)
foreach (var arg in args)
{
    var parts = arg.Split('=', 2);
    if (parts.Length != 2) continue;
    switch (parts[0].ToLower())
    {
        case "vehicleid": vehicleId = uint.Parse(parts[1]); break;
        case "centerlat": centerLat = double.Parse(parts[1], CultureInfo.InvariantCulture); break;
        case "centerlon": centerLon = double.Parse(parts[1], CultureInfo.InvariantCulture); break;
        case "radius": radius = double.Parse(parts[1], CultureInfo.InvariantCulture); break;
        case "lengthkm": lengthKm = int.Parse(parts[1]); break;
        case "stepsec": stepSec = int.Parse(parts[1]); break;
        case "clickhouse": clickhouseConn = parts[1]; break;
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

// 4. Записываем точки в ClickHouse раз в stepSec секунд
using var conn = new ClickHouseConnection(clickhouseConn);
await conn.OpenAsync();

const string insertSql = @"INSERT INTO can_telemetry (vehicle_id, ts, pos, speed, rpm, fuel_lvl)
VALUES ({vid:UInt32}, {ts:DateTime64}, ({lon:Float64}, {lat:Float64}), {speed:Float32}, {rpm:UInt16}, {fuel:UInt8})";

var now = DateTime.UtcNow;
float speed = 30;
ushort rpm = 1500;
byte fuel = 80;

for (int i = 0; i < points.Count; i++)
{
    var (lon, lat, dist) = points[i];
    var ts = now.AddSeconds(i * stepSec);
    // Можно добавить случайный разброс скорости, топлива и т.д.
    speed = 30 + (float)(rnd.NextDouble() * 20);
    rpm = (ushort)(1500 + rnd.Next(0, 500));
    fuel = (byte)Math.Max(0, fuel - rnd.Next(0, 2));

    using var cmd = conn.CreateCommand(insertSql);
    cmd.Parameters.AddWithValue("vid", vehicleId);
    cmd.Parameters.AddWithValue("ts", ts);
    cmd.Parameters.AddWithValue("lon", lon);
    cmd.Parameters.AddWithValue("lat", lat);
    cmd.Parameters.AddWithValue("speed", speed);
    cmd.Parameters.AddWithValue("rpm", rpm);
    cmd.Parameters.AddWithValue("fuel", fuel);
    await cmd.ExecuteNonQueryAsync();

    Console.WriteLine($"[{ts:HH:mm:ss}] {lat:F5},{lon:F5} speed={speed:F1} fuel={fuel}");
    await Task.Delay(stepSec * 1000);
}

Console.WriteLine("Генерация трека завершена!");

// ---
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
