namespace AndrejKrizan.DotNet.Functional;

public static class ObjectExtensions
{
    public static T Do<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }

    public static T Transform<T>(this T value, Func<T, T> function)
        => function(value);

    public static TResult Transform<T, TResult>(this T value, Func<T, TResult> function)
        => function(value);
}
