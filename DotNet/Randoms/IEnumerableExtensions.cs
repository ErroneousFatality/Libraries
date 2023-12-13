using AndrejKrizan.DotNet.Collections;

namespace AndrejKrizan.DotNet.Randoms;
public static class IEnumerableExtensions
{
    #region TakeRandomly
    public static IEnumerable<T> TakeRandomly<T>(this IEnumerable<T> source, int count)
    {
        Random random = new();
        HashSet<int> indexes = random.NextSet(count, 0, source.Count() - 1);
        IEnumerable<T> subset = source.Take(indexes);
        return subset;
    }

    public static IEnumerable<T> TakeRandomly<T>(this IEnumerable<T> source)
    {
        Random random = new();
        int length = source.Count();
        int count = random.Next(0, length);
        HashSet<int> indexes = random.NextSet(count, 0, length - 1);
        IEnumerable<T> subset = source.Take(indexes);
        return subset;
    }
    #endregion
}
