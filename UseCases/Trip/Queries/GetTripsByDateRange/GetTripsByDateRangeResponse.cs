namespace Autopark.UseCases.Trip.Queries.GetTripsByDateRange;

public sealed record GetTripsByDateRangeResponse(
    TripInfo[] Trips,
    TrackPoint[] TrackPoints,
    double TotalDistanceKm,
    int TotalDurationMinutes
);

public sealed record TripInfo(
    long TripId,
    uint VehicleId,
    DateTime StartUtc,
    DateTime EndUtc,
    int DurationMinutes,
    double? DistanceKm
);

public sealed record TrackPoint(
    DateTime LocalTime,
    double Latitude,
    double Longitude,
    float Speed,
    ushort Rpm,
    byte FuelLevel
);