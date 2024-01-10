using System.Collections.Immutable;

using AndrejKrizan.DotNet.Collections;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Comparers;

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
