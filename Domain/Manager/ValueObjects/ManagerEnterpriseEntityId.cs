using Autopark.Domain.Common.Models;

namespace Autopark.Domain.Manager.ValueObjects;

public class ManagerEnterpriseEntityId : ValueObject
{
    public int Value { get; protected set; }

    private ManagerEnterpriseEntityId(int value)
    {
        Value = value;
    }

    public static ManagerEnterpriseEntityId Empty => new(0);

    public static ManagerEnterpriseEntityId Create(int value)
    {
        return new ManagerEnterpriseEntityId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}