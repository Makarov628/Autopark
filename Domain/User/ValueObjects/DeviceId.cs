using Autopark.Domain.Common.Models;

namespace Autopark.Domain.User.ValueObjects;

public class DeviceId : ValueObject
{
    public int Value { get; protected set; }

    private DeviceId(int value)
    {
        Value = value;
    }

    public static DeviceId Empty => new(0);

    public static DeviceId Create(int value)
    {
        return new DeviceId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}