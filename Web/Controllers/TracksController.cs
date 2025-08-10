using Microsoft.AspNetCore.Mvc;
using TimeZoneConverter;
using Autopark.Infrastructure.Database.Services;
using Autopark.Domain.Vehicle.ValueObjects;

public sealed record TrackPoint(
    DateTime LocalTime,
    double Latitude,
    double Longitude,
    float Speed,
    ushort Rpm,
    byte FuelLevel);

[ApiController]
[Route("api/[controller]")]
public sealed class TracksController : ControllerBase
{
    private readonly IVehicleTrackingService _trackingService;

    public TracksController(IVehicleTrackingService trackingService) => _trackingService = trackingService;

    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> Get(
        int vehicleId,
        DateTime from,
        DateTime to,
        string tz = "UTC",
        string format = "json")
    {
        try
        {
            var vehicleIdValue = VehicleId.Create(vehicleId);

            // Получаем точки трекинга из PostgreSQL
            var trackPoints = await _trackingService.GetTrackPointsAsync(vehicleIdValue, from, to);

            var tzInfo = TZConvert.GetTimeZoneInfo(tz);
            var list = trackPoints.Select(tp =>
            {
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(tp.TimestampUtc, tzInfo);
                return new TrackPoint(
                    localTime,
                    tp.Latitude,
                    tp.Longitude,
                    tp.Speed,
                    tp.Rpm,
                    tp.FuelLevel
                );
            }).ToList();

            return format.Equals("geojson", StringComparison.OrdinalIgnoreCase)
                ? Ok(GeoJsonHelper.ToFeatureCollection(list))
                : Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving track data: {ex.Message}");
        }
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        double latitude,
        double longitude,
        double radiusMeters = 1000,
        DateTime? from = null,
        DateTime? to = null,
        string tz = "UTC",
        string format = "json")
    {
        try
        {
            var trackPoints = await _trackingService.GetTrackPointsInRadiusAsync(
                latitude, longitude, radiusMeters, from, to);

            var tzInfo = TZConvert.GetTimeZoneInfo(tz);
            var list = trackPoints.Select(tp =>
            {
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(tp.TimestampUtc, tzInfo);
                return new TrackPoint(
                    localTime,
                    tp.Latitude,
                    tp.Longitude,
                    tp.Speed,
                    tp.Rpm,
                    tp.FuelLevel
                );
            }).ToList();

            return format.Equals("geojson", StringComparison.OrdinalIgnoreCase)
                ? Ok(GeoJsonHelper.ToFeatureCollection(list))
                : Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving nearby track data: {ex.Message}");
        }
    }
}
public static class GeoJsonHelper
{
    public static object ToFeatureCollection(IEnumerable<TrackPoint> points)
    {
        var coords = points.Select(p => new[] { p.Longitude, p.Latitude });
        return new
        {
            type = "FeatureCollection",
            features = new[] {
                new {
                    type = "Feature",
                    geometry = new {
                        type  = "LineString",
                        coordinates = coords
                    },
                    properties = new { }
                }
            }
        };
    }
}