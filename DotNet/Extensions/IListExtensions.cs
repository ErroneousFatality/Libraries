namespace AndrejKrizan.DotNet.Extensions;
public static class IListExtensions
{
    public static IEnumerable<T> Take<T>(this IList<T> source, IEnumerable<int> indexes)
        => indexes.Order().Select(index => source[index]);
}
