using Autopark.Infrastructure.Database;
using Autopark.Infrastructure.Database.Services;
using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Octonica.ClickHouseClient;
using TimeZoneConverter;

namespace Autopark.UseCases.Trip.Queries.GetTripsByDateRange;

internal class GetTripsByDateRangeQueryHandler : IRequestHandler<GetTripsByDateRangeQuery, GetTripsByDateRangeResponse>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ClickHouseConnection _clickHouseConnection;
    private readonly ITimeZoneService _timeZoneService;

    public GetTripsByDateRangeQueryHandler(
        AutoparkDbContext dbContext,
        ClickHouseConnection clickHouseConnection,
        ITimeZoneService timeZoneService)
    {
        _dbContext = dbContext;
        _clickHouseConnection = clickHouseConnection;
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

        // 3. Получаем точки трека из ClickHouse для всех поездок
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
        const string sql = @"
            SELECT ts, pos.2 AS lat, pos.1 AS lon,
                   speed, rpm, fuel_lvl
            FROM   can_telemetry
            WHERE  vehicle_id = {vid:UInt32}
              AND  ts BETWEEN {from:DateTime64} AND {to:DateTime64}
            ORDER BY ts";

        await _clickHouseConnection.OpenAsync();
        await using var cmd = _clickHouseConnection.CreateCommand(sql);
        cmd.Parameters.AddWithValue("vid", vehicleId);
        cmd.Parameters.AddWithValue("from", startUtc);
        cmd.Parameters.AddWithValue("to", endUtc);

        var points = new List<TrackPoint>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var utc = reader.GetDateTime(0);
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, timeZoneInfo);
            points.Add(new TrackPoint(
                local,
                reader.GetDouble(1),
                reader.GetDouble(2),
                reader.GetFloat(3),
                reader.GetUInt16(4),
                reader.GetByte(5)
            ));
        }

        return points;
    }
}