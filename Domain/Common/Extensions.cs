
using Autopark.Domain.Common.Models;
using LanguageExt;
using LanguageExt.Common;

namespace Autopark.Domain.Common;

public static class Extensions
{
    public static string JoinStrings(this IEnumerable<string> strings, string separator)
    {
        return string.Join(separator, strings);
    }

    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

    public static Fin<T> ToFin<T>(this Either<Error, T> either) =>
        either.Match(Fin<T>.Succ, Fin<T>.Fail);

    public static Either<Error, ValueObject> ToValueObjectEither<T>(this Fin<T> fin) where T : ValueObject =>
        fin.Map<ValueObject>(x => x).ToEither();

    public static Fin<ValueObject> ToValueObjectFin<T>(this Fin<T> fin) where T : ValueObject =>
        fin.Map<ValueObject>(x => x);
}