namespace Autopark.UseCases.Trip.Queries.GetTrips;

public sealed record GetTripsResponse(
    TripDto[] Trips,
    TrackPointDto[] TrackPoints,
    int TotalTrips,
    double TotalDistanceKm,
    int TotalDurationMinutes
);

public sealed record TripDto(
    long TripId,
    uint VehicleId,
    DateTime StartUtc,
    DateTime EndUtc,
    int DurationMinutes,
    double? DistanceKm,
    TripPointDto? StartPoint,
    TripPointDto? EndPoint
);

public sealed record TripPointDto(
    long PointId,
    double Latitude,
    double Longitude,
    string? Address,
    DateTime? AddressResolvedAt
);

public sealed record TrackPointDto(
    long Id,
    double Latitude,
    double Longitude,
    DateTime UtcTime,
    DateTime LocalTime,
    float Speed,
    ushort Rpm,
    byte FuelLevel
);