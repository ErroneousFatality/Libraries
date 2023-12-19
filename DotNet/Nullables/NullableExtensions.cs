using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.Nullables;
public static class NullableExtensions
{
    public static bool TryGetValue<T>(this T? nullable, out T value)
        where T : struct
    {
        if (nullable.HasValue)
        {
            value = nullable.Value;
            return true;
        }
        value = default;
        return false;
    }

    public static bool TryGetValue<T>(this T? nullable, [NotNullWhen(true)] out T? value)
        where T : class
    {
        if (nullable == null)
        {
            value = null;
            return false;
        }
        value = nullable;
        return true;
    }
}
