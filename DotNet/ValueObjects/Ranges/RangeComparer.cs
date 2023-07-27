using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.ValueObjects.Ranges;

public class RangeComparer<T> : IEqualityComparer<Range<T>>
    where T : struct
{
    public bool Equals(Range<T>? x, Range<T>? y)
        => x == y;

    public int GetHashCode([DisallowNull] Range<T> range)
        => range.GetHashCode();
}
