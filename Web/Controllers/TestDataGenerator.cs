using Autopark.Infrastructure.Database.Services;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;

public static class TestDataGenerator
{
    public static async Task GenerateTestTrack(IVehicleTrackingService trackingService)
    {
        var vehicleId = VehicleId.Create(42);

        // Очищаем старые данные для vehicle_id = 42
        await trackingService.DeleteTrackPointsAsync(vehicleId);

        // Создаем трек по кругу в центре Алматы
        var centerLat = 43.2567;
        var centerLon = 76.9286;
        var radius = 0.01; // ~1 км
        var startTime = DateTime.UtcNow.AddHours(-2); // 2 часа назад

        var trackPoints = new List<VehicleTrackPointEntity>();

        for (int i = 0; i < 100; i++)
        {
            var angle = (2 * Math.PI * i) / 100;
            var lat = centerLat + radius * Math.Cos(angle);
            var lon = centerLon + radius * Math.Sin(angle);
            var time = startTime.AddMinutes(i * 2); // каждые 2 минуты
            var speed = 30 + (i % 20); // 30-50 км/ч
            var rpm = (ushort)(1500 + (i % 500)); // 1500-2000 об/мин
            var fuel = (byte)(80 - (i / 10)); // постепенно уменьшается

            var trackPoint = VehicleTrackPointEntity.Create(
                vehicleId,
                time,
                lat,
                lon,
                speed,
                rpm,
                fuel
            );

            trackPoints.Add(trackPoint);
        }

        await trackingService.AddTrackPointsAsync(trackPoints);

        Console.WriteLine($"Создан тестовый трек для vehicle_id = 42 с 100 точками");
    }
}