namespace AndrejKrizan.DotNet.Strings;
public static class IEnumerableStringExtensions
{
    #region StringJoin
    public static string StringJoin(this IEnumerable<string?> source, string? separator = ", ", bool quote = false)
    {
        if (quote)
        {
            source = source.Select(str => str.Quote());
        }
        return string.Join(separator, source);
    }

    public static string StringJoin(this IEnumerable<string?> source, char separator, bool quote = false)
    {
        if (quote)
        {
            source = source.Select(str => str.Quote());
        }
        return string.Join(separator, source);
    }


    public static string StringJoin<T>(this IEnumerable<T?> source, string? separator = ", ", bool quote = false)
        => source.Select(str => str?.ToString()).StringJoin(separator, quote);

    public static string StringJoin<T>(this IEnumerable<T?> source, char separator, bool quote = false)
        => source.Select(str => str?.ToString()).StringJoin(separator, quote);
    #endregion
}
