using AndrejKrizan.DotNet.Extensions;

using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Collections.Immutable;

namespace AndrejKrizan.EntityFramework.Common.Comparers
{
    public class ImmutableArrayComparer<T> : ValueComparer<ImmutableArray<T>>
        where T : notnull
    {
        public ImmutableArrayComparer(IEqualityComparer<T>? comparer = null)
            : base(
                (left, right) => left.SequenceEqual(right, comparer),
                array => array.GetSequenceHashCode(),
                array => array
            )
        { }
    }
}
