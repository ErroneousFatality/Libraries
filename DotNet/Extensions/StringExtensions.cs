using System.Collections.Immutable;
using System.Text;

namespace AndrejKrizan.DotNet.Extensions;

public static class StringExtensions
{
    public static string ReplaceAt(this string str, int index, char ch)
        => string.Create(str.Length, str, (span, _str) =>
        {
            _str.AsSpan().CopyTo(span);
            span[index] = ch;
        });

    #region SplitToEnumerable
    public static IEnumerable<string> SplitToEnumerable(this string str, char separator, StringSplitOptions options = StringSplitOptions.None)
    {
        for (int startIndex = 0, finishIndex; startIndex < str.Length; startIndex = finishIndex + 1)
        {
            finishIndex = str.IndexOf(separator, startIndex);
            if (finishIndex == -1)
            {
                finishIndex = str.Length;
            }
            string substring = str[startIndex..finishIndex];
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                substring = substring.Trim();
            }
            if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && substring.Length == 0)
            {
                continue;
            }
            yield return substring;
        }
    }

    public static IEnumerable<string> SplitToEnumerable(this string str, string separator, 
        StringComparison comparison = StringComparison.CurrentCulture, 
        StringSplitOptions options = StringSplitOptions.None
    )
    {
        for (int startIndex = 0, finishIndex; startIndex < str.Length; startIndex = finishIndex + 1)
        {
            finishIndex = str.IndexOf(separator, startIndex, comparison);
            if (finishIndex == -1)
            {
                finishIndex = str.Length;
            }
            string substring = str[startIndex..finishIndex];
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                substring = substring.Trim();
            }
            if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && substring.Length == 0)
            {
                continue;
            }
            yield return substring;
        }
    }
    #endregion

    public static string? Quote(this string? str)
        => str == null ? str : '\"' + str + '\"';

    #region ToLowercasedFirstCharacter
    public static string ToLowercasedFirstCharacter(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToLower(str[0]));

    public static string ToLowercasedFirstCharacterInvariant(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToLowerInvariant(str[0]));
    #endregion LowercaseFirstCharacter

    #region ToUppercasedFirstCharacter
    public static string ToUppercasedFirstCharacter(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToUpper(str[0]));

    public static string ToUppercasedFirstCharacterInvariant(this string str)
        => str.Length < 1
            ? str
            : str.ReplaceAt(0, char.ToUpperInvariant(str[0]));

    #endregion

    #region AddPrefixIdempotent
    public static string AddPrefixIdempotent(this string str, char prefix)
        => str.StartsWith(prefix)
            ? str
            : prefix + str;
    public static string AddPrefixIdempotent(this string str, string prefix)
        => str.StartsWith(prefix)
            ? str
            : prefix + str;
    #endregion

    #region ContainsAny
    // TODO: optimize to run in O(n)
    public static bool ContainsAny(this string source, params string[] strings)
        => strings.Any(source.Contains);

    public static bool ContainsAny(this string source, StringComparison comparison, params string[] strings)
        => strings.Any(needle => source.Contains(needle, comparison));

    public static bool ContainsAny(this string source, params char[] chars)
        => source.IndexOfAny(chars) >= 0;
    #endregion

    #region ReplaceAll
    public static string ReplaceAll(this string source, IEnumerable<char> oldChars, char newChar)
        => source.ReplaceAll(oldChars.ToImmutableHashSet(), newChar);

    public static string ReplaceAll(this string source, IReadOnlySet<char> oldChars, char newChar)
    {
        IEnumerable<char> chars = source.Select(character
            => oldChars.Contains(character)
                ? newChar
                : character
        );
        string result = string.Concat(chars);
        return result;
    }

    public static string ReplaceAll(this string source, IReadOnlyDictionary<char, char> dictionary)
    {
        IEnumerable<char> chars = source.Select(character
            => dictionary.TryGetValue(character, out char newChar)
                ? newChar
                : character
        );
        string result = string.Concat(chars);
        return result;
    }
    #endregion

    public static string RemoveAll(this string source, IReadOnlySet<char> chars)
    {
        StringBuilder builder = new(source.Length);
        foreach(char character in source)
        {
            if (!chars.Contains(character))
            {
                builder.Append(character);
            }
        }
        string result = builder.ToString();
        return result;
    }
}
