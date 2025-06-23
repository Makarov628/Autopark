using Autopark.Domain.Common.Models;

namespace Autopark.Domain.User.ValueObjects;

public class CredentialsId : ValueObject
{
    public int Value { get; protected set; }

    private CredentialsId(int value)
    {
        Value = value;
    }

    public static CredentialsId Empty => new(0);

    public static CredentialsId Create(int value)
    {
        return new CredentialsId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}