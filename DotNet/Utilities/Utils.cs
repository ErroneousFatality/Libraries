using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.Utilities
{
    public static partial class Utils
    {
        public static void ListInitializeOrAdd<T>([NotNull] ref List<T>? list, T item)
        {
            if (list == null)
            {
                list = new List<T> { item };
            }
            else
            {
                list.Add(item);
            }
        }

        public static T Min<T>(T first, T second, IComparer<T> comparer)
            => comparer.Compare(first, second) <= 0 ? first : second;
        public static T Min<T>(T first, T second)
            => Min(first, second, Comparer<T>.Default);

        public static T Max<T>(T first, T second, IComparer<T> comparer)
            => comparer.Compare(first, second) >= 0 ? first : second;
        public static T Max<T>(T first, T second)
            => Max(first, second, Comparer<T>.Default);
    }
}
