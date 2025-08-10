using Autopark.Infrastructure.Database;
using Autopark.Infrastructure.Database.Services;
using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeZoneConverter;

namespace Autopark.UseCases.Trip.Queries.GetTripsByDateRange;

internal class GetTripsByDateRangeQueryHandler : IRequestHandler<GetTripsByDateRangeQuery, GetTripsByDateRangeResponse>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly IVehicleTrackingService _trackingService;
    private readonly ITimeZoneService _timeZoneService;

    public GetTripsByDateRangeQueryHandler(
        AutoparkDbContext dbContext,
        IVehicleTrackingService trackingService,
        ITimeZoneService timeZoneService)
    {
        _dbContext = dbContext;
        _trackingService = trackingService;
        _timeZoneService = timeZoneService;
    }

    public async Task<GetTripsByDateRangeResponse> Handle(
        GetTripsByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Конвертируем входные даты из локального времени в UTC
        var fromUtc = _timeZoneService.ConvertFromEnterpriseTimeToUtc(request.From, request.TimeZoneId);
        var toUtc = _timeZoneService.ConvertFromEnterpriseTimeToUtc(request.To, request.TimeZoneId);

        // 2. Получаем поездки из основной БД
        var trips = await _dbContext.Trips
            .Where(t => t.VehicleId == VehicleId.Create(request.VehicleId))
            .Where(t => t.StartUtc >= fromUtc && t.EndUtc <= toUtc)
            .OrderBy(t => t.StartUtc)
            .Select(t => new TripInfo(
                t.Id.Value,
                (uint)t.VehicleId.Value,
                t.StartUtc,
                t.EndUtc,
                t.GetDurationMinutes(),
                t.DistanceKm))
            .ToArrayAsync(cancellationToken);

        if (!trips.Any())
        {
            return new GetTripsByDateRangeResponse(
                Array.Empty<TripInfo>(),
                Array.Empty<TrackPoint>(),
                0,
                0);
        }

        // 3. Получаем точки трека из PostgreSQL для всех поездок
        var allTrackPoints = new List<TrackPoint>();
        var tzInfo = TZConvert.GetTimeZoneInfo(request.TimeZoneId);

        foreach (var trip in trips)
        {
            var tripPoints = await GetTrackPointsForTrip(request.VehicleId, trip.StartUtc, trip.EndUtc, tzInfo);
            allTrackPoints.AddRange(tripPoints);
        }

        // 4. Вычисляем общую статистику
        var totalDistance = trips.Sum(t => t.DistanceKm ?? 0);
        var totalDuration = trips.Sum(t => t.DurationMinutes);

        return new GetTripsByDateRangeResponse(
            trips,
            allTrackPoints.ToArray(),
            totalDistance,
            totalDuration);
    }

    private async Task<List<TrackPoint>> GetTrackPointsForTrip(
        int vehicleId,
        DateTime startUtc,
        DateTime endUtc,
        TimeZoneInfo timeZoneInfo)
    {
        var vehicleIdValue = VehicleId.Create(vehicleId);
        var trackPoints = await _trackingService.GetTrackPointsAsync(vehicleIdValue, startUtc, endUtc);

        var points = trackPoints.Select(tp =>
        {
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(tp.TimestampUtc, timeZoneInfo);
            return new TrackPoint(
                localTime,
                tp.Latitude,
                tp.Longitude,
                tp.Speed,
                tp.Rpm,
                tp.FuelLevel
            );
        }).ToList();

        return points;
    }
}