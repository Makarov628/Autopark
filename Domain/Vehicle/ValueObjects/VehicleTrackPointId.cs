using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Vehicle.ValueObjects;

public sealed class VehicleTrackPointId : ValueObject
{
    public long Value { get; }

    private VehicleTrackPointId(long value)
    {
        Value = value;
    }

    public static VehicleTrackPointId Create(long value)
    {
        return new VehicleTrackPointId(value);
    }

    public static VehicleTrackPointId CreateNew()
    {
        return new VehicleTrackPointId(0); // EF будет генерировать автоматически
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator long(VehicleTrackPointId id) => id.Value;
}