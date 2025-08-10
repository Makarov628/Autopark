using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Trip.ValueObjects;

public sealed class TripPointId : ValueObject
{
    public long Value { get; }

    private TripPointId(long value)
    {
        Value = value;
    }

    public static TripPointId Create(long value)
    {
        return new TripPointId(value);
    }

    public static TripPointId CreateNew()
    {
        return new TripPointId(0); // EF будет генерировать автоматически
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator long(TripPointId id) => id.Value;
}