using AndrejKrizan.DotNet.Extensions;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Comparers;

public class ArrayComparer<T> : ValueComparer<T[]>
    where T : notnull
{
    public ArrayComparer(IEqualityComparer<T>? comparer = null)
        : base(
            (left, right)
                => left == right
                || left != null && right != null && left.SequenceEqual(right, comparer),
            array => array.GetSequenceHashCode(),
            array => array.ToArray()
        )
    { }
}
