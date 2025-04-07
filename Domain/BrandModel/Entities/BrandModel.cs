using Autopark.Domain.Common.Models;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.BrandModel.Enums;
using Autopark.Domain.Vehicle.Entities;

namespace Autopark.Domain.BrandModel.Entities;

public class BrandModelEntity : Entity<BrandModelId>
{
    private readonly List<VehicleEntity> _vehicles = new();

    public string BrandName { get; protected set; }
    public string ModelName { get; protected set; }
    public TransportType TransportType { get; set; }
    public FuelType FuelType { get; protected set; }
    public uint SeatsNumber { get; protected set; }
    public uint MaximumLoadCapacityInKillograms { get; protected set; }

    public IReadOnlyList<VehicleEntity> Vehicles => _vehicles.AsReadOnly();

    private BrandModelEntity(
        BrandModelId id,
        string brandName,
        string modelName,
        TransportType transportType,
        FuelType fuelType,
        uint seatsNumber,
        uint maximumLoadCapacityInKillograms) : base(id)
    {
        BrandName = brandName;
        ModelName = modelName;
        TransportType = transportType;
        FuelType = fuelType;
        SeatsNumber = seatsNumber;
        MaximumLoadCapacityInKillograms = maximumLoadCapacityInKillograms;
    }

    public static BrandModelEntity Create(
        string brandName,
        string modelName,
        TransportType transportType,
        FuelType fuelType,
        uint seatsNumber,
        uint maximumLoadCapacityInKillograms) =>
            Create(
                BrandModelId.Empty,
                brandName,
                modelName,
                transportType,
                fuelType,
                seatsNumber,
                maximumLoadCapacityInKillograms);

    public static BrandModelEntity Create(
        BrandModelId id,
        string brandName,
        string modelName,
        TransportType transportType,
        FuelType fuelType,
        uint seatsNumber,
        uint maximumLoadCapacityInKillograms) =>
            new BrandModelEntity(
                id,
                brandName,
                modelName,
                transportType,
                fuelType,
                seatsNumber,
                maximumLoadCapacityInKillograms);

    // TODO: Temp method. Needs to decompose to separate methods
    public void Update(
        string brandName,
        string modelName,
        TransportType transportType,
        FuelType fuelType,
        uint seatsNumber,
        uint maximumLoadCapacityInKillograms)
    {
        BrandName = brandName;
        ModelName = modelName;
        TransportType = transportType;
        FuelType = fuelType;
        SeatsNumber = seatsNumber;
        MaximumLoadCapacityInKillograms = maximumLoadCapacityInKillograms;
    }

    protected BrandModelEntity()
    {
    }
}
