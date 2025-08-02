using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Trip.ValueObjects;

public sealed class TripId : ValueObject
{
    public int Value { get; private set; }

    private TripId(int value)
    {
        Value = value;
    }

    public static TripId Create(int value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}