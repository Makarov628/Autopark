namespace Autopark.Web.Models;

public sealed record TrackPoint(
    DateTime LocalTime,
    double Latitude,
    double Longitude,
    float Speed,
    ushort Rpm,
    byte FuelLevel);