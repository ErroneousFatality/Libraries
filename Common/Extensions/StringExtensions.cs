namespace AndrejKrizan.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
            => string.IsNullOrWhiteSpace(str)
                ? str
                : str.ReplaceAt(0, char.ToLowerInvariant(str[0]));

        public static string EnsureStartsWith(this string str, char prefix)
            => str.StartsWith(prefix)
                ? str
                : prefix + str;
        public static string EnsureStartsWith(this string str, string prefix)
            => str.StartsWith(prefix)
                ? str
                : prefix + str;

        public static string ReplaceAt(this string str, int index, char ch)
            => string.Create(str.Length, str, (span, _str) =>
            {
                _str.AsSpan().CopyTo(span);
                span[index] = ch;
            });

        public static IEnumerable<string> SplitToEnumerable(this string strings, char separator)
        {
            int startIndex = 0;
            while (startIndex < strings.Length)
            {
                int finishIndex = strings.IndexOf(separator, startIndex);
                if (finishIndex == -1)
                {
                    finishIndex = strings.Length;
                }
                string @string = strings[startIndex..finishIndex];
                yield return @string;
                startIndex = finishIndex + 1;
            }
        }
    }
}
