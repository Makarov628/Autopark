using Autopark.Infrastructure.Database.Services;
using Autopark.Domain.Vehicle.ValueObjects;
using MediatR;

namespace Autopark.UseCases.Trip.Queries.GetTrips;

internal class GetTripsQueryHandler : IRequestHandler<GetTripsQuery, GetTripsResponse>
{
    private readonly ITripService _tripService;
    private readonly IVehicleTrackingService _trackingService;
    private readonly ITimeZoneService _timeZoneService;

    public GetTripsQueryHandler(
        ITripService tripService,
        IVehicleTrackingService trackingService,
        ITimeZoneService timeZoneService)
    {
        _tripService = tripService;
        _trackingService = trackingService;
        _timeZoneService = timeZoneService;
    }

    public async Task<GetTripsResponse> Handle(
        GetTripsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Конвертируем входные даты из локального времени в UTC
        var fromUtc = _timeZoneService.ConvertFromEnterpriseTimeToUtc(request.From, request.TimeZoneId);
        var toUtc = _timeZoneService.ConvertFromEnterpriseTimeToUtc(request.To, request.TimeZoneId);

        // 2. Получаем поездки из БД
        var vehicleId = VehicleId.Create(request.VehicleId);
        var trips = await _tripService.GetTripsByDateRangeAsync(vehicleId, fromUtc, toUtc, cancellationToken);

        var tripsList = trips.ToList();

        // 3. Разрешаем адреса для точек, которые еще не имеют адресов
        await _tripService.ResolveAddressesForTripsAsync(tripsList, cancellationToken);

        // 4. Преобразуем в DTO
        var tripDtos = tripsList.Select(trip => new TripDto(
            trip.Id.Value,
            (uint)trip.VehicleId.Value,
            trip.StartUtc,
            trip.EndUtc,
            trip.GetDurationMinutes(),
            trip.DistanceKm,
            trip.StartPoint != null ? new TripPointDto(
                trip.StartPoint.Id.Value,
                trip.StartPoint.Latitude,
                trip.StartPoint.Longitude,
                trip.StartPoint.Address,
                trip.StartPoint.AddressResolvedAt
            ) : null,
            trip.EndPoint != null ? new TripPointDto(
                trip.EndPoint.Id.Value,
                trip.EndPoint.Latitude,
                trip.EndPoint.Longitude,
                trip.EndPoint.Address,
                trip.EndPoint.AddressResolvedAt
            ) : null
        )).ToArray();

        // 5. Получаем точки трека
        var trackPoints = await _trackingService.GetTrackPointsAsync(vehicleId, fromUtc, toUtc, cancellationToken);
        var trackPointDtos = trackPoints.Select(tp => new TrackPointDto(
            tp.Id.Value,
            tp.Latitude,
            tp.Longitude,
            tp.TimestampUtc,
            _timeZoneService.ConvertFromUtcToEnterpriseTime(tp.TimestampUtc, request.TimeZoneId),
            tp.Speed,
            tp.Rpm,
            tp.FuelLevel
        )).ToArray();

        // 6. Вычисляем статистику
        var totalDistance = tripsList.Sum(t => t.DistanceKm ?? 0);
        var totalDuration = tripsList.Sum(t => t.GetDurationMinutes());

        return new GetTripsResponse(
            tripDtos,
            trackPointDtos,
            tripsList.Count,
            totalDistance,
            totalDuration);
    }
}