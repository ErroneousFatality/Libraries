using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AndrejKrizan.DotNet.Collections;

public static class ArrayExtensions
{
    #region ImmutableArray
    public static ImmutableArray<T> ToImmutableArray<T>(this T[] array)
        => ImmutableArray.Create(array);

    public static ImmutableArray<T> AsImmutableArray<T>(this T[] array)
        => Unsafe.As<T[], ImmutableArray<T>>(ref array);
    #endregion

    #region Fill
    public static T[] Fill<T>(this T[] array, T value)
    {
        Array.Fill(array, value);
        return array;
    }

    public static T[] Fill<T>(this T[] array, T value, int startIndex, int count)
    {
        Array.Fill(array, value, startIndex, count);
        return array;
    }

    public static T[] Fill<T>(this T[] array, Func<int, T> valueGenerator)
        => array.Fill(valueGenerator, 0, array.Length);

    public static T[] Fill<T>(this T[] array, Func<int, T> valueGenerator, int startIndex, int count)
    {
        int endIndex = startIndex + count;
        for (int i = startIndex; i < endIndex; i++)
        {
            array[i] = valueGenerator(i);
        }
        return array;
    }
    #endregion
}
