using LanguageExt;
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Vehicle.ValueObjects;
using LanguageExt.Common;
using Autopark.Domain.Common.ValueObjects;

namespace Autopark.Domain.Vehicle.Entities;

public class VehicleEntity : Entity<VehicleId>
{
    public CyrillicString Name { get; protected set; }
    public Price Price { get; protected set; }
    public Mileage MileageInKilometers { get; set; }
    public CyrillicString Color { get; protected set; }
    public RegistrationNumber RegistrationNumber { get; protected set; }

    private VehicleEntity(
        VehicleId id,
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber) : base(id)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
        RegistrationNumber = registrationNumber;
    }

    public static VehicleEntity Create(
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber) =>
            Create(
                VehicleId.Empty,
                name,
                price,
                mileageInKilometers,
                color,
                registrationNumber);

    public static VehicleEntity Create(
        VehicleId id,
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber) =>
            new VehicleEntity(
                id,
                name,
                price,
                mileageInKilometers,
                color,
                registrationNumber);

    // TODO: Temp method. Needs to decompose to separate methods
    public void Update(
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
        RegistrationNumber = registrationNumber;
    }

    protected VehicleEntity()
    {
    }
}
