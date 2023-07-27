using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.ValueObjects.Ranges;

public class NullableRangeComparer<T> : IEqualityComparer<NullableRange<T>>
    where T : struct
{
    public bool Equals(NullableRange<T>? x, NullableRange<T>? y)
        => x == null && y == null
        || x != null && x.CompareTo(y) == 0;

    public int GetHashCode([DisallowNull] NullableRange<T> range)
        => range.From.GetHashCode() ^ range.To.GetHashCode();
}
