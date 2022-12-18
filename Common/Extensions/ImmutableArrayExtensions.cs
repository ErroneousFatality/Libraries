using System.Collections.Immutable;

namespace AndrejKrizan.Common.Extensions
{
    public static class ImmutableArrayExtensions
    {
        public static ImmutableArray<TResult> Convert<T, TResult>(this ImmutableArray<T> items, Func<T, TResult> converter)
            => ImmutableArray.CreateRange(items, converter);
    }
}
