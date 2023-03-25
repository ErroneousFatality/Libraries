using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.Utilities
{
    public static partial class Utils
    {
        public static void ListInitializeOrAddTo<T>([NotNull] ref List<T>? list, T item)
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

        public static void ListInitializeOrAddTo<T>([NotNull] ref List<T>? list, T item, params T[] additionalItems)
            => ListInitializeOrAddTo(ref list, additionalItems.Prepend(item));

        public static void ListInitializeOrAddTo<T>([NotNull] ref List<T>? list, IEnumerable<T> items)
        {
            if (list == null)
            {
                list = new List<T>(items);
            }
            else
            {
                list.AddRange(items);
            }
        }
    }
}
