using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AndrejKrizan.DotNet.Collections;

public static class ImmutableArrayExtensions
{
    extension<T>(ImmutableArray<T> items)
    {
        public ImmutableArray<TResult> Convert<TResult>(Func<T, TResult> selector)
            => ImmutableArray.CreateRange(items, selector);

        public T[] AsArray()
            => Unsafe.As<ImmutableArray<T>, T[]>(ref items);
    }
}
