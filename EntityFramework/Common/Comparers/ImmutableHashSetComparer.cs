using System.Collections.Immutable;
using AndrejKrizan.DotNet.Collections;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Comparers;

public class ImmutableHashSetComparer<T> : ValueComparer<ImmutableHashSet<T>>
    where T : notnull
{
    public ImmutableHashSetComparer()
        : base(
            (left, right) => left == right || left != null && right != null && left.SetEquals(right),
            set => set.GetContentHashCode(),
            set => set
        )
    { }

    public ImmutableHashSetComparer(IEqualityComparer<T> comparer)
        : base(
            (left, right) => left == right || left != null && right != null && left.SetEquals(right, comparer),
            set => set.GetContentHashCode(),
            set => set
        )
    { }
}
