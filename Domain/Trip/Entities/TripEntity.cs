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

    public TripPointId? StartPointId { get; protected set; }
    public TripPointEntity? StartPoint { get; protected set; }

    public TripPointId? EndPointId { get; protected set; }
    public TripPointEntity? EndPoint { get; protected set; }

    // Конструктор для Entity Framework
    private TripEntity() : base()
    {
    }

    private TripEntity(
        TripId id,
        VehicleId vehicleId,
        DateTime startUtc,
        DateTime endUtc,
        double? distanceKm = null,
        TripPointId? startPointId = null,
        TripPointId? endPointId = null) : base(id)
    {
        VehicleId = vehicleId;
        StartUtc = startUtc;
        EndUtc = endUtc;
        DistanceKm = distanceKm;
        StartPointId = startPointId;
        EndPointId = endPointId;
    }

    public static TripEntity Create(
        TripId id,
        VehicleId vehicleId,
        DateTime startUtc,
        DateTime endUtc,
        double? distanceKm = null,
        TripPointId? startPointId = null,
        TripPointId? endPointId = null)
    {
        if (startUtc >= endUtc)
            throw new ArgumentException("StartUtc must be before EndUtc");

        return new TripEntity(id, vehicleId, startUtc, endUtc, distanceKm, startPointId, endPointId);
    }

    public void UpdateDistance(double distanceKm)
    {
        DistanceKm = distanceKm;
        RenewUpdateDate();
    }

    public void SetStartPoint(TripPointId startPointId)
    {
        StartPointId = startPointId;
        RenewUpdateDate();
    }

    public void SetEndPoint(TripPointId endPointId)
    {
        EndPointId = endPointId;
        RenewUpdateDate();
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