using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.NullableRanges;

public class NullableRangeComparer<T> : IEqualityComparer<NullableRange<T>>
    where T : struct
{
    public bool Equals(NullableRange<T>? x, NullableRange<T>? y)
        => x == y;

    public int GetHashCode([DisallowNull] NullableRange<T> range)
        => range.GetHashCode();
}
