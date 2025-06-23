using Autopark.Domain.Common.Models;

namespace Autopark.Domain.User.ValueObjects;

public class UserRoleId : ValueObject
{
    public int Value { get; protected set; }

    private UserRoleId(int value)
    {
        Value = value;
    }

    public static UserRoleId Empty => new(0);

    public static UserRoleId Create(int value)
    {
        return new UserRoleId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}