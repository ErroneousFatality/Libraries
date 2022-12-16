using System.Collections.Immutable;

using AndrejKrizan.Common.Extensions;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Comparers
{
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
    }
}
