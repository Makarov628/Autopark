using Autopark.Domain.Common.Models;
using Autopark.Domain.Trip.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;

namespace Autopark.Domain.Trip.Entities;

public class TripEntity : Entity<TripId>
{
    public VehicleId VehicleId { get; protected set; }
    public DateTime StartUtc { get; protected set; }
    public DateTime EndUtc { get; protected set; }
    public double? DistanceKm { get; protected set; }

    private TripEntity(
        TripId id,
        VehicleId vehicleId,
        DateTime startUtc,
        DateTime endUtc,
        double? distanceKm = null) : base(id)
    {
        VehicleId = vehicleId;
        StartUtc = startUtc;
        EndUtc = endUtc;
        DistanceKm = distanceKm;
    }

    public static TripEntity Create(
        TripId id,
        VehicleId vehicleId,
        DateTime startUtc,
        DateTime endUtc,
        double? distanceKm = null)
    {
        if (startUtc >= endUtc)
            throw new ArgumentException("StartUtc must be before EndUtc");

        return new TripEntity(id, vehicleId, startUtc, endUtc, distanceKm);
    }

    public void UpdateDistance(double distanceKm)
    {
        DistanceKm = distanceKm;
    }

    public TimeSpan GetDuration()
    {
        return EndUtc - StartUtc;
    }

    public int GetDurationMinutes()
    {
        return (int)GetDuration().TotalMinutes;
    }
}