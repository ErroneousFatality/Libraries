using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AndrejKrizan.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static ImmutableArray<T> AsImmutableArray<T>(this T[] array)
            => Unsafe.As<T[], ImmutableArray<T>>(ref array);
    }
}
