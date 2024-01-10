using AndrejKrizan.DotNet.Collections;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Comparers;

public class ListComparer<T> : ValueComparer<List<T>>
    where T : notnull
{
    public ListComparer(IEqualityComparer<T>? comparer = null)
        : base(
            (left, right)
                => left == right
                || left != null && right != null && left.SequenceEqual(right, comparer),
            array => array.GetSequenceHashCode(),
            array => array.ToList()
        )
    { }
}
