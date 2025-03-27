using System.ComponentModel.DataAnnotations.Schema;
using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Vehicle.ValueObjects;

public class VehicleId : ValueObject
{
    public int Value { get; protected set; }

    private VehicleId(int value)
    {
        Value = value;
    }

    public static VehicleId Empty => new(0);

    public static VehicleId Create(int value)
    {
        return new VehicleId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}