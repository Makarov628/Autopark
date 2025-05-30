using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Enterprise.ValueObjects;

public class EnterpriseId : ValueObject
{
    public int Value { get; protected set; }

    private EnterpriseId(int value)
    {
        Value = value;
    }

    public static EnterpriseId Empty => new(0);

    public static EnterpriseId Create(int value)
    {
        return new EnterpriseId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}