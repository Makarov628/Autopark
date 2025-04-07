using Autopark.Domain.Common.Models;

namespace Autopark.Domain.BrandModel.ValueObjects;

public class BrandModelId : ValueObject
{
    public int Value { get; protected set; }

    private BrandModelId(int value)
    {
        Value = value;
    }

    public static BrandModelId Empty => new(0);

    public static BrandModelId Create(int value)
    {
        return new BrandModelId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}