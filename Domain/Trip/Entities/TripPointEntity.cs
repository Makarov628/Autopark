using Autopark.Domain.Common.Models;
using Autopark.Domain.Trip.ValueObjects;
using NetTopologySuite.Geometries;

namespace Autopark.Domain.Trip.Entities;

public class TripPointEntity : Entity<TripPointId>
{
    public Point Location { get; protected set; } = null!;
    public double Latitude => Location.Y;
    public double Longitude => Location.X;
    public string? Address { get; protected set; }
    public DateTime? AddressResolvedAt { get; protected set; }

    // Конструктор для Entity Framework
    private TripPointEntity() : base()
    {
    }

    private TripPointEntity(
        TripPointId id,
        Point location,
        string? address = null) : base(id)
    {
        Location = location;
        Address = address;
        AddressResolvedAt = address != null ? DateTime.UtcNow : null;
    }

    public static TripPointEntity Create(
        double latitude,
        double longitude,
        string? address = null,
        GeometryFactory? geometryFactory = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90 degrees");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180 degrees");

        // Используем переданную GeometryFactory или создаем новую с SRID 4326 (WGS84)
        var factory = geometryFactory ?? new GeometryFactory(new PrecisionModel(), 4326);
        var location = factory.CreatePoint(new Coordinate(longitude, latitude));

        return new TripPointEntity(TripPointId.CreateNew(), location, address);
    }

    public void UpdateAddress(string address)
    {
        Address = address;
        AddressResolvedAt = DateTime.UtcNow;
        RenewUpdateDate();
    }
}