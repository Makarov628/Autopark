using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Autopark.Domain.Common.Models;
using LanguageExt;
using LanguageExt.Common;

namespace Autopark.Domain.Vehicle.ValueObjects;

public class RegistrationNumber : ValueObject
{
    public string Value { get; private set; }

    private RegistrationNumber(string value)
    {
        Value = value;
    }

    public static Fin<RegistrationNumber> Create(string value)
    {
        if(!Pattern.IsMatch(value))
            return Error.New("Некорректный регистрационный номер. Примеры правильных номеров: A123ABC, 123ABC, B1234AB, 1234AB12");

        return new RegistrationNumber(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static Regex Pattern => new Regex(@"(\w)?(\d{3,4})(\w{2,3})(\d{2})?", RegexOptions.Compiled);
}