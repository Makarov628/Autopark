using LanguageExt;
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Vehicle.ValueObjects;
using LanguageExt.Common;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.BrandModel.Entities;

namespace Autopark.Domain.Vehicle.Entities;

public class VehicleEntity : Entity<VehicleId>
{
    public CyrillicString Name { get; protected set; }
    public Price Price { get; protected set; }
    public Mileage MileageInKilometers { get; set; }
    public CyrillicString Color { get; protected set; }
    public RegistrationNumber RegistrationNumber { get; protected set; }

    public BrandModelId BrandModelId { get; protected set; }
    public BrandModelEntity BrandModel { get; protected set; }

    private VehicleEntity(
        VehicleId id,
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId) : base(id)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
        RegistrationNumber = registrationNumber;
        BrandModelId = brandModelId;
    }

    public static VehicleEntity Create(
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId) =>
            Create(
                VehicleId.Empty,
                name,
                price,
                mileageInKilometers,
                color,
                registrationNumber,
                brandModelId);

    public static VehicleEntity Create(
        VehicleId id,
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId) =>
            new VehicleEntity(
                id,
                name,
                price,
                mileageInKilometers,
                color,
                registrationNumber,
                brandModelId);

    // TODO: Temp method. Needs to decompose to separate methods
    public void Update(
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
        RegistrationNumber = registrationNumber;
        BrandModelId = brandModelId;
    }

    protected VehicleEntity()
    {
    }
}
