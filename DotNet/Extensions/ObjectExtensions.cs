namespace AndrejKrizan.DotNet.Extensions;

public static class ObjectExtensions
{
    #region Apply
    public static T Apply<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }

    public static T Apply<T>(this T value, Func<T, T> function)
        => function(value);

    public static TResult Apply<T, TResult>(this T value, Func<T, TResult> function)
        => function(value);
    #endregion
}
