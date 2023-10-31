namespace AndrejKrizan.DotNet.Ranges;
public static class IEnumerableExtensions
{
    public static Range<T> GetRange<T>(this IEnumerable<T> source)
        where T : struct
        => source.GetRange(Comparer<T>.Default);

    public static Range<T> GetRange<T>(this IEnumerable<T> source, IComparer<T> comparer)
        where T : struct
    {
        T min = source.First();
        T max = min;

        foreach (T item in source.Skip(1))
        {
            if (comparer.Compare(item, min) < 0)
            {
                min = item;
            }
            else if (comparer.Compare(item, max) > 0)
            {
                max = item;
            }
        }
        return new Range<T>(from: min, to: max, validate: false);
    }


}
