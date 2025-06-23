using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Manager.ValueObjects;

public class ManagerId : ValueObject
{
    public int Value { get; protected set; }

    private ManagerId(int value)
    {
        Value = value;
    }

    public static ManagerId Empty => new(0);

    public static ManagerId Create(int value)
    {
        return new ManagerId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}