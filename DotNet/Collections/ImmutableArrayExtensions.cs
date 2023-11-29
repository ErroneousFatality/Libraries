using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Collections;

public static class ImmutableArrayExtensions
{
    public static ImmutableArray<TResult> Convert<T, TResult>(this ImmutableArray<T> items, Func<T, TResult> selector)
        => ImmutableArray.CreateRange(items, selector);
}
