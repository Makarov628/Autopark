using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Autopark.Infrastructure.Database.Services;

public interface IVehicleTrackingService
{
    Task<IEnumerable<VehicleTrackPointEntity>> GetTrackPointsAsync(
        VehicleId vehicleId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);

    Task AddTrackPointAsync(
        VehicleTrackPointEntity trackPoint,
        CancellationToken cancellationToken = default);

    Task AddTrackPointsAsync(
        IEnumerable<VehicleTrackPointEntity> trackPoints,
        CancellationToken cancellationToken = default);

    Task DeleteTrackPointsAsync(
        VehicleId vehicleId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<VehicleTrackPointEntity>> GetTrackPointsInRadiusAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);
}

public class VehicleTrackingService : IVehicleTrackingService
{
    private readonly AutoparkDbContext _context;
    private readonly GeometryFactory _geometryFactory;

    public VehicleTrackingService(AutoparkDbContext context)
    {
        _context = context;
        _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
    }

    public async Task<IEnumerable<VehicleTrackPointEntity>> GetTrackPointsAsync(
        VehicleId vehicleId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTrackPoints
            .Where(tp => tp.VehicleId == vehicleId &&
                        tp.TimestampUtc >= fromUtc &&
                        tp.TimestampUtc <= toUtc)
            .OrderBy(tp => tp.TimestampUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddTrackPointAsync(
        VehicleTrackPointEntity trackPoint,
        CancellationToken cancellationToken = default)
    {
        _context.VehicleTrackPoints.Add(trackPoint);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddTrackPointsAsync(
        IEnumerable<VehicleTrackPointEntity> trackPoints,
        CancellationToken cancellationToken = default)
    {
        _context.VehicleTrackPoints.AddRange(trackPoints);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTrackPointsAsync(
        VehicleId vehicleId,
        CancellationToken cancellationToken = default)
    {
        var trackPoints = await _context.VehicleTrackPoints
            .Where(tp => tp.VehicleId == vehicleId)
            .ToListAsync(cancellationToken);

        _context.VehicleTrackPoints.RemoveRange(trackPoints);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<VehicleTrackPointEntity>> GetTrackPointsInRadiusAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var centerPoint = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        var query = _context.VehicleTrackPoints
            .Where(tp => tp.Location.Distance(centerPoint) <= radiusMeters);

        if (fromUtc.HasValue)
            query = query.Where(tp => tp.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(tp => tp.TimestampUtc <= toUtc.Value);

        return await query
            .OrderBy(tp => tp.TimestampUtc)
            .ToListAsync(cancellationToken);
    }
}