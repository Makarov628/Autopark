
namespace Autopark.Domain.Common;

public static class Extensions
{
    public static string JoinStrings(this IEnumerable<string> strings, string separator)
    {
        return string.Join(separator, strings);
    }
}