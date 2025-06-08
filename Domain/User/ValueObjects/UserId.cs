using Autopark.Domain.Common.Models;

namespace Autopark.Domain.User.ValueObjects;

public class UserId : ValueObject
{
    public int Value { get; protected set; }

    private UserId(int value)
    {
        Value = value;
    }

    public static UserId Empty => new(0);

    public static UserId Create(int value)
    {
        return new UserId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}