using Autopark.Domain.Common.Models;
using Autopark.Domain.Vehicle.ValueObjects;
using NetTopologySuite.Geometries;

namespace Autopark.Domain.Vehicle.Entities;

public class VehicleTrackPointEntity : Entity<VehicleTrackPointId>
{
    public VehicleId VehicleId { get; protected set; }
    public VehicleEntity Vehicle { get; protected set; } = null!; // Navigation property
    public DateTime TimestampUtc { get; protected set; }
    public Point Location { get; protected set; } = null!;
    public double Latitude => Location.Y;
    public double Longitude => Location.X;
    public float Speed { get; protected set; }
    public ushort Rpm { get; protected set; }
    public byte FuelLevel { get; protected set; }

    // Конструктор для Entity Framework
    private VehicleTrackPointEntity() : base()
    {
    }

    private VehicleTrackPointEntity(
        VehicleTrackPointId id,
        VehicleId vehicleId,
        DateTime timestampUtc,
        Point location,
        float speed,
        ushort rpm,
        byte fuelLevel) : base(id)
    {
        VehicleId = vehicleId;
        TimestampUtc = timestampUtc;
        Location = location;
        Speed = speed;
        Rpm = rpm;
        FuelLevel = fuelLevel;
    }

    public static VehicleTrackPointEntity Create(
        VehicleId vehicleId,
        DateTime timestampUtc,
        double latitude,
        double longitude,
        float speed,
        ushort rpm,
        byte fuelLevel)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90 degrees");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180 degrees");

        if (speed < 0)
            throw new ArgumentException("Speed cannot be negative");

        if (fuelLevel > 100)
            throw new ArgumentException("Fuel level cannot exceed 100%");

        // Используем GeometryFactory с SRID 4326 (WGS84)
        var factory = new GeometryFactory(new PrecisionModel(), 4326);
        var location = factory.CreatePoint(new Coordinate(longitude, latitude));

        return new VehicleTrackPointEntity(
            VehicleTrackPointId.CreateNew(),
            vehicleId,
            timestampUtc,
            location,
            speed,
            rpm,
            fuelLevel);
    }
}