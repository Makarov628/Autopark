using System.Text.RegularExpressions;
using Autopark.Domain.Common.Models;
using LanguageExt;
using LanguageExt.Common;

namespace Autopark.Domain.Vehicle.ValueObjects;

public class Price : ValueObject
{
    public decimal Value { get; private set; }

    private Price(decimal value)
    {
        Value = value;
    }

    public static Fin<Price> Create(decimal value)
    {
        if (value < 0)
            return Error.New("Цена (Price) меньше 0");

        return new Price(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}