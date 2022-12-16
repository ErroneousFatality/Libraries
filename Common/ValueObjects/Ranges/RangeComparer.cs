using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.Common.ValueObjects.Ranges
{
    public class RangeComparer<T> : IEqualityComparer<Range<T>>
        where T : struct
    {
        public bool Equals(Range<T>? x, Range<T>? y)
            => x == null && y == null
            || x != null && x.CompareTo(y) == 0;

        public int GetHashCode([DisallowNull] Range<T> range)
            => range.From.GetHashCode() ^ range.To.GetHashCode();
    }
}
