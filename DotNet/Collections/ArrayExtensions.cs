using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AndrejKrizan.DotNet.Collections;

public static class ArrayExtensions
{
    extension<T>(T[] array)
    {
        #region ImmutableArray
        public ImmutableArray<T> ToImmutableArray()
            => ImmutableArray.Create(array);

        public ImmutableArray<T> AsImmutableArray()
            => Unsafe.As<T[], ImmutableArray<T>>(ref array);
        #endregion

        #region Fill
        public T[] Fill(T value)
        {
            Array.Fill(array, value);
            return array;
        }

        public T[] Fill(T value, int startIndex, int count)
        {
            Array.Fill(array, value, startIndex, count);
            return array;
        }

        public T[] Fill(Func<int, T> valueGenerator)
            => array.Fill(valueGenerator, 0, array.Length);

        public T[] Fill(Func<int, T> valueGenerator, int startIndex, int count)
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
}
