using System.Text.RegularExpressions;
using Autopark.Domain.Common.Models;
using LanguageExt;
using LanguageExt.Common;

namespace Autopark.Domain.Common.ValueObjects;

public class CyrillicString : ValueObject
{
    public string Value { get; private set; }

    private CyrillicString(string value)
    {
        Value = value;
    }

    public static Fin<CyrillicString> Create(string value)
    {
        if(!Pattern.IsMatch(value))
            return Error.New("Используйте символы кириллицы и числа от 0 до 9");

        return new CyrillicString(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static Regex Pattern => new Regex(@"[а-яА-я0-9 ]", RegexOptions.Compiled);
}