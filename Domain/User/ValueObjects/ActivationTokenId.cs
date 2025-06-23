using Autopark.Domain.Common.Models;

namespace Autopark.Domain.User.ValueObjects;

public class ActivationTokenId : ValueObject
{
    public int Value { get; protected set; }

    private ActivationTokenId(int value)
    {
        Value = value;
    }

    public static ActivationTokenId Empty => new(0);

    public static ActivationTokenId Create(int value)
    {
        return new ActivationTokenId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}