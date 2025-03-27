using System.Text.RegularExpressions;
using Autopark.Domain.Common.Models;
using LanguageExt;
using LanguageExt.Common;

namespace Autopark.Domain.Vehicle.ValueObjects;

public class Mileage : ValueObject
{
    public double ValueInKilometers { get; private set; }

    private Mileage(double value)
    {
        ValueInKilometers = value;
    }

    public static Fin<Mileage> Create(double value)
    {
        if (value < 0)
            return Error.New("Пройденное расстояние (Mileage) меньше 0");

        return new Mileage(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueInKilometers;
    }
}