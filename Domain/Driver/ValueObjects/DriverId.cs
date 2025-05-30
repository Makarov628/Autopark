using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Driver.ValueObjects;

public class DriverId : ValueObject
{
    public int Value { get; protected set; }

    private DriverId(int value)
    {
        Value = value;
    }

    public static DriverId Empty => new(0);

    public static DriverId Create(int value)
    {
        return new DriverId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}