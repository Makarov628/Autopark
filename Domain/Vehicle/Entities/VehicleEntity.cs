using LanguageExt;
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Vehicle.ValueObjects;
using LanguageExt.Common;

namespace Autopark.Domain.Vehicle.Entities;

public class VehicleEntity : Entity<VehicleId>
{
    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    public double MileageInKilometers { get; set; }
    public string Color { get; protected set; }

    private VehicleEntity(
        VehicleId id,
        string name,
        decimal price,
        double mileageInKilometers,
        string color) : base(id)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
    }

    public static Fin<VehicleEntity> Create(string name, decimal price, double mileageInKilometers, string color) =>
        Validate(name, price, mileageInKilometers, color)
            .Map(_ => new VehicleEntity(VehicleId.CreateUnique(), name, price, mileageInKilometers, color));

    // TODO: Temp method. Needs to decompose to separate methods
    public Fin<Unit> Update(string name, decimal price, double mileageInKilometers, string color) =>
        Validate(name, price, mileageInKilometers, color)
            .Do(_ =>
            {
                Name = name;
                Price = price;
                MileageInKilometers = mileageInKilometers;
                Color = color;
            });


    private static Fin<Unit> Validate(
        string name,
        decimal price,
        double mileageInKilometers,
        string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.New("Name is empty");

        if (price < 0)
            return Error.New("Price is less than 0");

        if (mileageInKilometers < 0)
            return Error.New("Mileage is less than 0");

        if (string.IsNullOrWhiteSpace(color))
            return Error.New("Color is empty");

        return Unit.Default;
    }

    protected VehicleEntity()
    {
    }
}
