using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Autopark.Infrastructure.Database.Services;

public interface ITripService
{
    Task<IEnumerable<TripEntity>> GetTripsByDateRangeAsync(
        VehicleId vehicleId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);

    Task<TripEntity?> GetTripByIdAsync(
        TripId tripId,
        CancellationToken cancellationToken = default);

    Task<TripEntity> CreateTripAsync(
        TripEntity trip,
        CancellationToken cancellationToken = default);

    Task UpdateTripAsync(
        TripEntity trip,
        CancellationToken cancellationToken = default);

    Task DeleteTripAsync(
        TripId tripId,
        CancellationToken cancellationToken = default);

    Task<TripPointEntity> CreateTripPointAsync(
        double latitude,
        double longitude,
        string? address = null,
        CancellationToken cancellationToken = default);

    Task UpdateTripPointAddressAsync(
        TripPointId tripPointId,
        string address,
        CancellationToken cancellationToken = default);

    Task ResolveAddressesForTripsAsync(
        IEnumerable<TripEntity> trips,
        CancellationToken cancellationToken = default);
}

public class TripService : ITripService
{
    private readonly AutoparkDbContext _context;
    private readonly IGeocodingService _geocodingService;

    public TripService(AutoparkDbContext context, IGeocodingService geocodingService)
    {
        _context = context;
        _geocodingService = geocodingService;
    }

    public async Task<IEnumerable<TripEntity>> GetTripsByDateRangeAsync(
        VehicleId vehicleId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.StartPoint)
            .Include(t => t.EndPoint)
            .Where(t => t.VehicleId == vehicleId &&
                       t.StartUtc >= fromUtc &&
                       t.EndUtc <= toUtc)
            .OrderBy(t => t.StartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<TripEntity?> GetTripByIdAsync(
        TripId tripId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.StartPoint)
            .Include(t => t.EndPoint)
            .FirstOrDefaultAsync(t => t.Id == tripId, cancellationToken);
    }

    public async Task<TripEntity> CreateTripAsync(
        TripEntity trip,
        CancellationToken cancellationToken = default)
    {
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync(cancellationToken);
        return trip;
    }

    public async Task UpdateTripAsync(
        TripEntity trip,
        CancellationToken cancellationToken = default)
    {
        _context.Trips.Update(trip);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTripAsync(
        TripId tripId,
        CancellationToken cancellationToken = default)
    {
        var trip = await _context.Trips.FindAsync(new object[] { tripId }, cancellationToken);
        if (trip != null)
        {
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TripPointEntity> CreateTripPointAsync(
        double latitude,
        double longitude,
        string? address = null,
        CancellationToken cancellationToken = default)
    {
        var tripPoint = TripPointEntity.Create(latitude, longitude, address);
        _context.TripPoints.Add(tripPoint);
        await _context.SaveChangesAsync(cancellationToken);
        return tripPoint;
    }

    public async Task UpdateTripPointAddressAsync(
        TripPointId tripPointId,
        string address,
        CancellationToken cancellationToken = default)
    {
        var tripPoint = await _context.TripPoints.FindAsync(new object[] { tripPointId }, cancellationToken);
        if (tripPoint != null)
        {
            tripPoint.UpdateAddress(address);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ResolveAddressesForTripsAsync(
        IEnumerable<TripEntity> trips,
        CancellationToken cancellationToken = default)
    {
        var pointsToResolve = new List<(TripPointEntity Point, double Latitude, double Longitude)>();

        foreach (var trip in trips)
        {
            if (trip.StartPoint != null && string.IsNullOrEmpty(trip.StartPoint.Address))
            {
                pointsToResolve.Add((trip.StartPoint, trip.StartPoint.Latitude, trip.StartPoint.Longitude));
            }

            if (trip.EndPoint != null && string.IsNullOrEmpty(trip.EndPoint.Address))
            {
                pointsToResolve.Add((trip.EndPoint, trip.EndPoint.Latitude, trip.EndPoint.Longitude));
            }
        }

        if (!pointsToResolve.Any())
            return;

        // Получаем адреса батчем
        var coordinates = pointsToResolve.Select(p => (p.Latitude, p.Longitude)).ToList();
        var addresses = await _geocodingService.GetAddressesBatchAsync(coordinates, cancellationToken);

        // Обновляем адреса
        foreach (var (point, latitude, longitude) in pointsToResolve)
        {
            var key = $"{latitude:F6},{longitude:F6}";
            if (addresses.TryGetValue(key, out var address) && !string.IsNullOrEmpty(address))
            {
                point.UpdateAddress(address);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}